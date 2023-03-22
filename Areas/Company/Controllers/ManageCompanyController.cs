using DataRooms.UI.Areas.Company.Model;
using DataRooms.UI.Code;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.WebApiHelpers;
using Newtonsoft.Json;
using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Company.Controllers
{
    [SessionExpire]
    public class ManageCompanyController : Controller
    {
        LogService _logger;
        CompanyService _companyService;
        private readonly UserService _service;
        FileManager _fileManager;
        string authToken;
        string secretkey = ConfigurationManager.AppSettings["SecretKey"];
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        public ManageCompanyController()
        {
            authToken = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _logger = new LogService(authToken);
            _service = new UserService(authToken);
            _companyService = new CompanyService(authToken);
        }
        
        [HttpGet]
        public ActionResult List()
        {
            CustomCompanyModel model = new CustomCompanyModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(CustomCompanyModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(CustomCompanyModel model)
        {
            try
            {
                IEnumerable<DataRooms.UI.Models.Company> companies = null;
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    companies = DataCache.Companies.Where(x => x.CompanyName.ToLower().Contains(model.SearchString.ToLower()));
                    if(companies != null && companies.Count() > 0)
                    {
                        model.PagedCompanies = companies.ToPagedList(model.CurrentPage,model.RecordsPerPage);
                    }
                }
                else
                {
                    companies = DataCache.Companies;
                    if (companies != null && companies.Count() > 0)
                    {
                        model.PagedCompanies = companies.ToPagedList(model.CurrentPage, model.RecordsPerPage);
                    }
                    else
                    {
                        model.PagedCompanies = new List<DataRooms.UI.Models.Company>().ToPagedList(model.CurrentPage, model.RecordsPerPage);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                CustomCompanyModel model = new CustomCompanyModel();
                model.CompanyDetails = new DataRooms.UI.Models.Company();
                model.ADDetails = new Models.ADInfo();
                if (id != null && id > 0)
                    model.CompanyDetails = DataCache.Companies.Where(x => x.Id == id).First();
                var adDetails = DataCache.ADConfiguration.Where(x => x.CompanyId == id);
                if(adDetails!=null && adDetails.Count() > 0)
                {
                    model.ADDetails = adDetails.First();
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Company\Views\Shared\_editcompany.cshtml", model)
                });
                jr.MaxJsonLength = int.MaxValue;
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(DataRooms.UI.Models.Company company, DataRooms.UI.Models.ADInfo aDInfo)
        {
            int activitylogid = 0;
            try
            {
                logger.Debug(JsonConvert.SerializeObject(company));
                if (company.Id > 0)
                {
                    var oldcompany = DataCache.Companies.Where(x => x.Id == company.Id).First();
                    company.ModifiedBy = Convert.ToInt32(Session["Id"]);
                    company.ModifierName = Convert.ToString(Session["Name"]);
                    company.ModifiedOn = DateTime.Now;
                    company.RelativePath = oldcompany.RelativePath;
                    company.CreatedBy = oldcompany.CreatedBy;
                    company.CreatorName = oldcompany.CreatorName;
                    company.CreatedOn = oldcompany.CreatedOn;
                    await _companyService.UpdateCompany(company);
                    //new Thread(() => DataCache.RefreshSingleCompany(company)).Start();
                    DataCache.RefreshSingleCompany(company);
                    activitylogid = await _logger.LogActivity(company.Id,"Company Modification", "Company - " + company.CompanyName + " was modified by " + company.ModifierName);
                    await _logger.LogDataDifference(activitylogid, oldcompany, company);
                }
                else
                {
                    company.CreatedBy = Convert.ToInt32(Session["Id"]);
                    company.CreatorName = Convert.ToString(Session["Name"]);
                    company.CreatedOn = DateTime.Now;                    
                    Guid guid = Guid.NewGuid();
                    company.Guid = guid.ToString();                    
                    company.Id = await _companyService.SaveCompany(company);
                    // Update Path for the company
                    company.RelativePath = company.StoragePath + "/" + company.Id;
                    logger.Debug("before updation -- " + company.RelativePath);
                    await _companyService.UpdateCompany(company);
                    logger.Debug("after updation -- " + company.RelativePath);
                    //new Thread(() => DataCache.RefreshSingleCompany(company)).Start();
                    DataCache.RefreshSingleCompany(company);
                    //DataCache.RefreshCompanies();
                    _fileManager = new FileManager(authToken, company.Id);
                    _fileManager.CreateCompanyFolderinWorkSpace(company);
                    if (company.IsLogsRequired == true)
                    {
                        DataCache.CreateLogFolder(company.LogsStoragePath);
                    }
                    activitylogid = await _logger.LogActivity(company.Id, "Company Creation", "Company - " + company.CompanyName + " is created by " + company.ModifierName);
                    await _logger.LogDataDifference(activitylogid, new Models.Company(), company);
                }

                aDInfo.CompanyId = company.Id;
                aDInfo.CompanyName = company.CompanyName;

                aDInfo.IsADSync = EncryptionHelper.Encrypt(aDInfo.IsADSync, secretkey);
                aDInfo.DomainName = EncryptionHelper.Encrypt(aDInfo.DomainName, secretkey);
                aDInfo.IPAddress = EncryptionHelper.Encrypt(aDInfo.IPAddress, secretkey);
                aDInfo.CompanyName = EncryptionHelper.Encrypt(aDInfo.CompanyName, secretkey);
                if (aDInfo.Id == 0)
                {
                    await _service.SaveADInfo(aDInfo);
                }
                else
                {
                    await _service.UpdateADInfo(aDInfo);
                }
                DataCache.RefreshADConfiguration();
                TempData["Notification"] = "Company saved successfully";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<ActionResult> Delete(int id)
        {
            int activitylogid = 0;
            try
            {
                var oldcompany = DataCache.Companies.Where(x => x.Id == id).First();
                var company = oldcompany.Clone();
                company.DeletedBy = Convert.ToInt32(Session["Id"]);
                company.DeletorName = Convert.ToString(Session["Name"]);
                company.DeletedOn = DateTime.Now;
                _fileManager = new FileManager(authToken, company.Id);
                _fileManager.DeleteCompanyFolderinWorkSpace(company);
                if (company.IsLogsRequired == true)
                {
                    _fileManager.DeleteFolderfromWorkSpace(company.LogsStoragePath + "/Logs");
                }
                activitylogid = await _logger.LogActivity(company.Id, "Company Deletion", "Company - " + company.CompanyName + " was deleted by " + company.ModifierName);
                await _logger.LogDataDifference(activitylogid, oldcompany, company);
                new Thread(() => DataCache.RemoveCompanyfromCache(company)).Start();
                await _companyService.DeleteCompany(company);
                TempData["Notification"] = "Company deleted successfully";
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}