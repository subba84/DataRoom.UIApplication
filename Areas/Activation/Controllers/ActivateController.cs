using DataRooms.UI.Code;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Activation.Controllers
{
    public class ActivateController : Controller
    {
        ActivationService _service = new ActivationService();
        public ActionResult Index()
        {
            
            return View();
        }

        public async Task<ActionResult> Activate(string flag = "")
        {
            try
            {
                ViewBag.Flag = flag;
                HostDetails hostInfo = DataCache.HostInformation == null ? await _service.GetHostInformation() : DataCache.HostInformation;
                //List<Module> modules = await _service.GetModulefromApi(hostInfo);
                string secretkey = ConfigurationManager.AppSettings["SecretKey"];
                //if (modules != null && modules.Count > 0)
                //{
                //    foreach (var module in modules)
                //    {
                //        module.ModuleName = EncryptionHelper.Decrypt(module.ModuleName, secretkey);
                //        module.ModuleCount = EncryptionHelper.Decrypt(module.ModuleCount, secretkey);
                //        module.FromDate = EncryptionHelper.Decrypt(module.FromDate, secretkey);
                //        module.ToDate = EncryptionHelper.Decrypt(module.ToDate, secretkey);
                //        module.FromDate = FormatDatePart(module.FromDate);
                //        module.ToDate = FormatDatePart(module.ToDate);
                //    }
                //}

                List<Module> modules = new List<Module>();
                Module module = new Module();
                module.ModuleName = "SharBox";
                module.ModuleCount = "1";
                module.LicenseStatus = "Active";
                module.FromDate = "09-16-2022";
                module.ToDate = "11-16-2022";
                modules.Add(module);
                if (modules != null && modules.Count > 0 && modules.Where(x => x.LicenseStatus == "Active").Count() > 0)
                {
                    modules = modules.Where(x => x.ModuleName == "SharBox").ToList();
                    Module mod = modules.First();
                    LicenseInfo licenseInfo = new LicenseInfo();
                    licenseInfo.Column1 = EncryptionHelper.Encrypt(mod.ModuleName, secretkey);
                    licenseInfo.Column2 = EncryptionHelper.Encrypt(mod.ModuleCount, secretkey);
                    licenseInfo.Column3 = EncryptionHelper.Encrypt(mod.LicenseStatus, secretkey);
                    licenseInfo.Column4 = EncryptionHelper.Encrypt(mod.FromDate, secretkey);
                    licenseInfo.Column5 = EncryptionHelper.Encrypt(mod.ToDate, secretkey);
                    await _service.SaveLicenseInfo(licenseInfo);
                    DataCache.RefreshModule();
                    WebConfigHelper.UpdateKeyValue();
                    return View(modules);
                }
                else
                {
                    if (flag == "re-activate")
                    {
                        // Need to build this view
                        return RedirectToAction("GetAvailableModules");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Authenticate", new { area = "Login" });
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public string FormatDatePart(string date)
        {
            try
            {
                DateTime dtFinaldate; string sDateTime;
                // Remove Time Component
                string[] timeparts = date.Split(' ');
                date = timeparts[0];
                try 
                {
                    string[] sDate = date.Split('/');
                    sDateTime = sDate[1] + '-' + sDate[0] + '-' + sDate[2];
                    dtFinaldate = Convert.ToDateTime(sDateTime);

                }
                catch (Exception e)
                {
                    try
                    {
                        string[] sDate = date.Split('-');
                        sDateTime = sDate[1] + '-' + sDate[0] + '-' + sDate[2];
                        dtFinaldate = Convert.ToDateTime(sDateTime);
                    }
                    catch (Exception e1)
                    {
                        string[] sDate = date.Split('/');
                        sDateTime = sDate[1] + '/' + sDate[0] + '/' + sDate[2];
                        dtFinaldate = Convert.ToDateTime(sDateTime);
                    }
                }
                return dtFinaldate.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(date.ToString() + "," + ex);
            }
        }

        public async Task<ActionResult> GetAvailableModules()
        {
            List<Module> modules = new List<Module>();
            try
            {
                HostDetails hostInfo = DataCache.HostInformation == null ? await _service.GetHostInformation() : DataCache.HostInformation;
                modules = await _service.GetModulefromApi(hostInfo);
                string secretkey = ConfigurationManager.AppSettings["SecretKey"];
                if (modules != null && modules.Count > 0)
                {
                    foreach (var module in modules)
                    {
                        module.ModuleName = EncryptionHelper.Decrypt(module.ModuleName, secretkey);
                        module.ModuleCount = EncryptionHelper.Decrypt(module.ModuleCount, secretkey);
                        module.FromDate = EncryptionHelper.Decrypt(module.FromDate, secretkey);
                        module.ToDate = EncryptionHelper.Decrypt(module.ToDate, secretkey);
                        module.FromDate = FormatDatePart(module.FromDate);
                        module.ToDate = FormatDatePart(module.ToDate);
                    }
                }
                if (modules != null && modules.Count > 0 && modules.Where(x => x.LicenseStatus == "Active").Count() > 0)
                {
                    var moduleinfo = modules.Where(x => x.ModuleName == "SharBox");
                    if(moduleinfo!=null && moduleinfo.Count() > 0)
                    {
                        return View(moduleinfo.ToList());
                    }
                    return View(new List<Module>());
                }
                else
                {
                    return View(new List<Module>());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(modules);
        }
    }
}