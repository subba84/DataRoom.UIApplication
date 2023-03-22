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
    public class ManageApprovalController : Controller
    {
        int loggedInUser = 0; string loggedInUserName = string.Empty;
        private AuditLogService _auditLogService { get; set; }
        private FileService fileService { get; set; }
        TaskManager _taskManager;
        private LogService _logger { get; set; }
        SendEmail _emailSender;
        public ManageApprovalController()
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
                IEnumerable<ToDoTask> tasks = DataCache.ToDoTasks.Where(x => x.TaskCategory == "Approval" && x.UserId == loggedInUser);
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    tasks = tasks.Where(x => x.FileName.ToLower().Contains(model.SearchString.ToLower())
                    || x.FolderName.ToLower().Contains(model.SearchString.ToLower())
                    || x.DataRoomName.ToLower().Contains(model.SearchString.ToLower())
                    || x.FileId.ToString().Contains(model.SearchString.ToLower())
                    || x.Id.ToString().Contains(model.SearchString.ToLower()));
                    if (tasks != null && tasks.Count() > 0)
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int fileid, int taskid)
        {
            try
            {
                CustomReviewModel model = new CustomReviewModel();
                model.TaskId = taskid;
                model.File = DataCache.Files.First(x => x.Id == fileid);
                model.AuditHistory = await _auditLogService.GetAuditLogs(fileid);
                if (model.AuditHistory != null && model.AuditHistory.Count() > 0)
                {
                    model.AuditHistory = model.AuditHistory.OrderByDescending(x => x.Id);
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~/Areas/TaskApproval/Views/Shared/_approvalfile.cshtml", model)
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
        public async Task<ActionResult> Edit(File file, int TaskId)
        {
            try
            {
                File fl = DataCache.Files.First(x => x.Id == file.Id);
                Models.DataRoom room = DataCache.DataRooms.First(x => x.Id == fl.DataRoomId);
                var foderdetails = DataCache.Folders.Where(x => x.Id == fl.FolderId);
                Models.Folder folder = new Folder();
                if (foderdetails != null && foderdetails.Count() > 0)
                {
                    folder = foderdetails.First();
                }
                WorkFlowMaster workflow = DataCache.WorkFlows.First(x => x.Id == folder.WorkFlowId);
                var tasks = DataCache.ToDoTasks.Where(x => x.FileId == fl.Id);
                if ((workflow.IsSingleApproverRequired == true || workflow.IsSignleApprovalSufficient == true) && file.Status == "Approved")
                {
                    fl.Status = "Approved";
                    fl.ModifiedBy = loggedInUser;
                    fl.ModifierName = loggedInUserName;
                    fl.ModifiedOn = DateTime.Now;
                    fl.IsApproved = true;
                    fl.ApprovedBy = loggedInUser;
                    fl.ApproverName = loggedInUserName;
                    fl.ApprovedOn = DateTime.Now;
                    await fileService.UpdateFile(fl);
                    new Task(() => DataCache.RefreshSingleFile(fl)).Start();
                }
                else if ((workflow.IsMultipleApproversRequired || workflow.IsSignleApprovalSufficient == false) && tasks.Count() == 1 && file.Status == "Approved")
                {
                    fl.Status = "Approved";
                    fl.ModifiedBy = loggedInUser;
                    fl.ModifierName = loggedInUserName;
                    fl.ModifiedOn = DateTime.Now;
                    fl.IsApproved = true;
                    fl.ApprovedBy = loggedInUser;
                    fl.ApproverName = loggedInUserName;
                    fl.ApprovedOn = DateTime.Now;
                    await fileService.UpdateFile(fl);
                    new Task(() => DataCache.RefreshSingleFile(fl)).Start();
                }
                else if (file.Status == "Rejected")
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
                if(file.Status == "Rejected")
                {
                    var todoTasks = DataCache.ToDoTasks.Where(x => x.FileId == file.Id);
                    if (todoTasks != null && todoTasks.Count() > 0)
                    {
                        foreach (var task in todoTasks.ToList())
                        {
                            await _taskManager.DeleteTask(task.Id);
                        }
                    }
                    var todotasks = DataCache.ToDoTasks.Where(x => x.FileId == fl.Id);
                    if (todotasks != null && todotasks.Count() > 0)
                    {
                        todotasks = todotasks.OrderBy(x => x.Id);
                        _emailSender.SendEmailtoUser("WorkFlow", userid: todotasks.First().UserId, fileid: fl.Id, statusFlag: "Rejected");
                    }
                    else
                    {
                        _emailSender.SendEmailtoUser("WorkFlow", userid: fl.CreatedBy, fileid: fl.Id, statusFlag: "Rejected");
                    }
                }
                else
                {
                    await _taskManager.DeleteTask(TaskId);
                }

                await _auditLogService.SaveAuditLog(log);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (file.Status == "Rejected")
            {
                TempData["Notification"] = "File - " + file.FileName + " have been rejected";
            }
            else
            {
                TempData["Notification"] = "File - " + file.FileName + " have been approved successfully";
            }
            return RedirectToAction("List");
        }
    }
}