using DataRooms.UI.Areas.TaskApproval.Model;
using DataRooms.UI.Code;
using DataRooms.UI.Code.Email;
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

namespace DataRooms.UI.Areas.TaskApproval.Controllers
{
    [SessionExpire]
    public class ManageReviewController : Controller
    {
        int loggedInUser = 0;string loggedInUserName = string.Empty;
        private AuditLogService _auditLogService { get; set; }
        private FileService fileService { get; set; }
        TaskManager _taskManager;
        private LogService _logger { get; set; }
        SendEmail _emailSender;
        public ManageReviewController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            loggedInUser = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
            loggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
            _auditLogService = new AuditLogService(token);
            fileService = new FileService(token);
            _logger = new LogService(token);
            _taskManager = new TaskManager(token);
            _emailSender = new SendEmail();
        }

        [HttpGet]
        public ActionResult List()
        {
            CustomReviewModel model = new CustomReviewModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(CustomReviewModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(CustomReviewModel model)
        {
            try
            {
                IEnumerable<ToDoTask> tasks = DataCache.ToDoTasks.Where(x=>x.TaskCategory == "Review" && x.UserId == loggedInUser);
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    tasks = tasks.Where(x => x.FileName.ToLower().Contains(model.SearchString.ToLower())
                    || x.FolderName.ToLower().Contains(model.SearchString.ToLower())
                    || x.DataRoomName.ToLower().Contains(model.SearchString.ToLower())
                    || x.FileId.ToString().Contains(model.SearchString.ToLower())
                    || x.Id.ToString().Contains(model.SearchString.ToLower()));
                    if(tasks!=null && tasks.Count() > 0)
                    {
                        model.PagedTaks = tasks.ToPagedList(model.CurrentPage, model.RecordsPerPage);
                    }
                    else
                    {
                        model.PagedTaks = new List<ToDoTask>().ToPagedList(model.CurrentPage, model.RecordsPerPage);
                    }
                }
                else
                {
                    if (tasks != null && tasks.Count() > 0)
                    {
                        model.PagedTaks = tasks.ToPagedList(model.CurrentPage, model.RecordsPerPage);
                    }
                    else
                    {
                        model.PagedTaks = new List<ToDoTask>().ToPagedList(model.CurrentPage, model.RecordsPerPage);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int fileid,int taskid)
        {
            try
            {
                CustomReviewModel model = new CustomReviewModel();
                model.TaskId = taskid;
                model.File = DataCache.Files.First(x=>x.Id == fileid);
                model.AuditHistory = await _auditLogService.GetAuditLogs(fileid);
                if(model.AuditHistory!=null && model.AuditHistory.Count() > 0)
                {
                    model.AuditHistory = model.AuditHistory.OrderByDescending(x => x.Id);
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~/Areas/TaskApproval/Views/Shared/_reviewfile.cshtml", model)
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

        [HttpPost]
        public async Task<ActionResult> Edit(File file,int TaskId)
        {
            try
            {
                File fl = DataCache.Files.First(x=>x.Id == file.Id);
                Models.DataRoom room = DataCache.DataRooms.First(x=>x.Id == fl.DataRoomId);
                var foderdetails = DataCache.Folders.Where(x => x.Id == fl.FolderId);
                Models.Folder folder = new Folder();
                if (foderdetails != null && foderdetails.Count() > 0)
                {
                    folder = foderdetails.First();
                }
                WorkFlowMaster workflow = DataCache.WorkFlows.First(x=>x.Id == folder.WorkFlowId);
                var tasks = DataCache.ToDoTasks.Where(x => x.FileId == fl.Id && x.TaskCategory == "Review");
                if((workflow.IsSingleReviewerRequired == true || workflow.IsSingleReviewSufficient == true) && file.Status != "Rejected")
                {
                    fl.Status = "Reviewed";
                    fl.ModifiedBy = loggedInUser;
                    fl.ModifierName = loggedInUserName;
                    fl.ModifiedOn = DateTime.Now;
                    await fileService.UpdateFile(fl);
                    new Task(() => DataCache.RefreshSingleFile(fl)).Start();
                }
                else if ((workflow.IsMultipleReviewersRequired || workflow.IsSingleReviewSufficient == false) && tasks.Count() == 1 && file.Status != "Rejected")
                {
                    fl.Status = "Reviewed";
                    fl.ModifiedBy = loggedInUser;
                    fl.ModifierName = loggedInUserName;
                    fl.ModifiedOn = DateTime.Now;
                    await fileService.UpdateFile(fl);
                    new Task(() => DataCache.RefreshSingleFile(fl)).Start();
                }
                else if(file.Status == "Rejected")
                {
                    fl.Status = "Rejected";
                    fl.ModifiedBy = loggedInUser;
                    fl.ModifierName = loggedInUserName;
                    fl.ModifiedOn = DateTime.Now;
                    await fileService.UpdateFile(fl);
                    new Task(() => DataCache.RefreshSingleFile(fl)).Start();
                }

                
                AuditLog log = new AuditLog();
                log.AuditorId = loggedInUser;
                log.AuditorName = loggedInUserName;
                log.FileId = file.Id;
                log.FileName = file.FileName;
                log.Status = file.Status;
                log.AuditOn = DateTime.Now;
                log.Comments = file.FileDescription;
                if(fl.Status == "Rejected")
                {
                    var todoTasks = DataCache.ToDoTasks.Where(x => x.FileId == file.Id);
                    if(todoTasks!=null && todoTasks.Count() > 0)
                    {
                        foreach(var task in todoTasks.ToList())
                        {
                            await _taskManager.DeleteTask(task.Id);
                        }
                    }

                    _emailSender.SendEmailtoUser("WorkFlow", userid: fl.CreatedBy, fileid: fl.Id, statusFlag: "Rejected");
                }
                else
                {
                    await _taskManager.DeleteTask(TaskId);
                    // Create tasks for approvers...
                    await _taskManager.CreateApprovalTask(fl);
                    var todotasks = DataCache.ToDoTasks.Where(x => x.FileId == fl.Id);
                    if (todotasks != null && todotasks.Count() > 0)
                    {
                        todotasks = todotasks.OrderBy(x => x.Id);
                        _emailSender.SendEmailtoUser("WorkFlow", userid: todotasks.First().UserId, fileid: fl.Id, statusFlag: "Reviewed");
                    }
                    else
                    {
                        _emailSender.SendEmailtoUser("WorkFlow", userid: fl.CreatedBy, fileid: fl.Id, statusFlag: "Reviewed");
                    }
                }
                await _auditLogService.SaveAuditLog(log);

                if(file.Status == "Reviewed" && workflow.IsApprovalRequired == false)
                {
                    fl.Status = "Approved";
                    fl.ModifiedBy = loggedInUser;
                    fl.ModifierName = loggedInUserName;
                    fl.ModifiedOn = DateTime.Now;
                    await fileService.UpdateFile(fl);
                    new Task(() => DataCache.RefreshSingleFile(fl)).Start();

                    AuditLog auditlog = new AuditLog();
                    auditlog.AuditorId = loggedInUser;
                    auditlog.AuditorName = loggedInUserName;
                    auditlog.FileId = file.Id;
                    auditlog.FileName = file.FileName;
                    auditlog.Status = file.Status;
                    auditlog.AuditOn = DateTime.Now;
                    auditlog.Comments = file.FileDescription;
                    await _auditLogService.SaveAuditLog(auditlog);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            if(file.Status == "Rejected")
            {
                TempData["Notification"] = "File - " + file.FileName + " have been rejected";
            }
            else
            {
                TempData["Notification"] = "File - " + file.FileName + " have been reviewed successfully";
            }
            
            return RedirectToAction("List");
        }
    }
}