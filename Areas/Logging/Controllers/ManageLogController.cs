using DataRooms.UI.Areas.Logging.Models;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Logging.Controllers
{
    [SessionExpire]
    public class ManageLogController : Controller
    {
        private LoggerApiService _service { get; set; }
        private XMLLogService _xmlLogService { get; set; }
        int companyId = 0;
        public ManageLogController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new LoggerApiService(token);
            companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            var company = DataCache.Companies.Single(x => x.Id == companyId);
            _xmlLogService = new XMLLogService(company.LogsStoragePath);
        }


        [HttpGet]
        public ActionResult List()
        {
            try
            {
                var model = new ActivityLogModel();
                GetAllActivityLogsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult List(ActivityLogModel model)
        {
            try
            {
                GetAllActivityLogsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetAllActivityLogsAsync(ActivityLogModel model)
        {
            try
            {
                model.SortColumn = string.IsNullOrEmpty(model.SortColumn) ? "CreatedOn" : model.SortColumn;
                IPagedList<ActivityLog> logs = null;
                switch (model.SortColumn)
                {
                    case "CreatedOn":
                        if (model.SortOrder.Equals("desc"))
                            logs = (_xmlLogService.GetActivityLogs()).OrderByDescending
                                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = (_xmlLogService.GetActivityLogs()).OrderBy
                                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "UserName":
                        if (model.SortOrder.Equals("desc"))
                            logs = (_xmlLogService.GetActivityLogs()).OrderByDescending
                                    (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = (_xmlLogService.GetActivityLogs()).OrderBy
                                    (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "ActivityCategory":
                        if (model.SortOrder.Equals("desc"))
                            logs = (_xmlLogService.GetActivityLogs()).OrderByDescending
                                    (m => m.ActivityCategory).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = (_xmlLogService.GetActivityLogs()).OrderBy
                                    (m => m.ActivityCategory).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "Default":
                        logs = (_xmlLogService.GetActivityLogs()).OrderByDescending
                            (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                }

                model.PagedActivityLogs = logs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDataLogsbyActivityLogId(int activitylogid)
        {
            try
            {
                IEnumerable<DataLog> logs = _xmlLogService.GetDataLogsbyActivityId(activitylogid);// await _service.GetDataLogs(activitylogid,"empty");
                ActivityLogModel model = new ActivityLogModel();
                if(logs!=null && logs.Count() > 0)
                {
                    model.DataLogs = new List<DataLogObject>();
                    foreach (var log in logs)
                    {
                        DataLogObject dataLogObject = new DataLogObject();
                        dataLogObject.TableName = log.TableName;
                        dataLogObject.OriginalData = GetObjectfromJson(log.TableName,log.OriginalData);
                        dataLogObject.ModifiedData = GetObjectfromJson(log.TableName,log.ModifiedData);
                        //dataLogObject.Changes = GetObjectfromJson(log.TableName,log.Changes);
                        model.DataLogs.Add(dataLogObject);
                    }
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Logging\Views\Shared\_datalogs.cshtml", model)
                });
                jr.MaxJsonLength = int.MaxValue;
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jr;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Dictionary<string, string> GetObjectfromJson(string tablename,string jsonData)
        {
            Type t = Type.GetType("DataRooms.UI.Models." + tablename);
            var originalData = JsonConvert.DeserializeObject(jsonData, t);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            var properties = t.GetProperties();
            if(properties!=null && properties.Count() > 0)
            {
                foreach(var property in properties)
                {
                    dictionary.Add(property.Name, GetPropValue(originalData, property.Name));
                }
            }
            return dictionary;
        }

        private static string FormatJson(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        public static string GetPropValue(object src, string propName)
        {
            return Convert.ToString(src.GetType().GetProperty(propName).GetValue(src, null));
        }
    }
}