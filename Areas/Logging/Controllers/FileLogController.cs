using DataRooms.UI.Areas.Logging.Models;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Logging.Controllers
{
    [SessionExpire]
    public class FileLogController : Controller
    {
        private LoggerApiService _service { get; set; }
        private XMLLogService _xmlLogService { get; set; }
        int companyId = 0;
        public FileLogController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new LoggerApiService(token);
            companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            var company = DataCache.Companies.Single(x => x.Id == companyId);
            _xmlLogService = new XMLLogService(company.LogsStoragePath);
        }

        //[HttpGet]
        //public ActionResult List()
        //{
        //    try
        //    {
        //        var model = new ActivityLogModel();
        //        GetAllActivityLogsAsync(model);
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet]
        public ActionResult List(int? page, string search = "", string pagesize = "")
        {
            try
            {
                var model = new ActivityLogModel();
                //GetAllActivityLogsAsync(model);
                if (!string.IsNullOrEmpty(pagesize))
                {
                    model.RecordsPerPage = Convert.ToInt32(pagesize);
                }
                IPagedList<ActivityLog> logs = null;
                IEnumerable<ActivityLog> activityLogs = _xmlLogService.GetActivityLogs();

                activityLogs = activityLogs.Where(x => x.ActivityCategory.ToLower().Contains(search.ToLower())
                                                       || x.ActivityDescription.ToLower().Contains(search.ToLower())
                                                       || x.DataRoomName.ToLower().Contains(search.ToLower())
                                                       || x.CreatorName.ToLower().Contains(search.ToLower()));


                logs = activityLogs.Where(x => x.FileId > 0).OrderByDescending
                                    (m => m.Id).ToPagedList(page ?? 1, model.RecordsPerPage);
                model.PagedActivityLogs = logs;
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
                IEnumerable<ActivityLog> activityLogs = _xmlLogService.GetActivityLogs();
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    activityLogs = activityLogs.Where(x => x.ActivityCategory.ToLower().Contains(model.SearchString.ToLower())
                                                       || x.ActivityDescription.ToLower().Contains(model.SearchString.ToLower())
                                                       || x.FileName.ToLower().Contains(model.SearchString.ToLower())
                                                       || x.CreatorName.ToLower().Contains(model.SearchString.ToLower()));
                }
                if (activityLogs == null || activityLogs.Count() == 0)
                {
                    activityLogs = new List<ActivityLog>();
                }
                switch (model.SortColumn)
                {
                    case "CreatedOn":
                        if (model.SortOrder.Equals("desc"))
                            logs = activityLogs.Where(x => x.FileId > 0).OrderByDescending
                                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = activityLogs.Where(x => x.FileId > 0).OrderBy
                                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "DataRoomName":
                        if (model.SortOrder.Equals("desc"))
                            logs = activityLogs.Where(x => x.FileId > 0).OrderByDescending
                                    (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = activityLogs.Where(x => x.FileId > 0).OrderBy
                                    (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "FolderName":
                        if (model.SortOrder.Equals("desc"))
                            logs = activityLogs.Where(x => x.FileId > 0).OrderByDescending
                                    (m => m.FolderName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = activityLogs.Where(x => x.FileId > 0).OrderBy
                                    (m => m.FolderName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "FileName":
                        if (model.SortOrder.Equals("desc"))
                            logs = activityLogs.Where(x => x.FileId > 0).OrderByDescending
                                    (m => m.FileName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = activityLogs.Where(x => x.FileId > 0).OrderBy
                                    (m => m.FileName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "UserName":
                        if (model.SortOrder.Equals("desc"))
                            logs = activityLogs.Where(x => x.FileId > 0).OrderByDescending
                                    (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = activityLogs.Where(x => x.FileId > 0).OrderBy
                                    (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "ActivityCategory":
                        if (model.SortOrder.Equals("desc"))
                            logs = activityLogs.Where(x => x.FileId > 0).OrderByDescending
                                    (m => m.ActivityCategory).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            logs = activityLogs.Where(x => x.FileId > 0).OrderBy
                                    (m => m.ActivityCategory).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "Default":
                        logs = activityLogs.Where(x => x.FileId > 0).OrderByDescending
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
    }
}