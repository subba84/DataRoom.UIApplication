
using DataRooms.UI.WebApiHelpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.Code
{
    public class TaskManager
    {
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        TaskService _service;
        public TaskManager(string token)
        {
            _service = new TaskService(token);
        }
        public async Task CreateTask(Models.File file)
        {
            try
            {
                logger.Debug("Entered into Task Creation when file submits..");
                var dataroom = DataCache.DataRooms.First(x=>x.Id == file.DataRoomId);
                var foderdetails = DataCache.Folders.Where(x => x.Id == file.FolderId);
                Models.Folder folder = new Models.Folder();
                if (foderdetails != null && foderdetails.Count() > 0)
                {
                    folder = foderdetails.First();
                }
                var workflow = DataCache.WorkFlows.First(x=>x.Id == folder.WorkFlowId && x.IsActive == true);
                if(file.Status == "Submitted")
                {
                    var taskstobecreated = DataCache.DataRoomWorkFlowUsers.Where(x => x.IsActive == true && x.WorkFlowId == workflow.Id && x.DataRoomId == dataroom.Id  && (x.RoleId == AppRole.Reviewer));
                    logger.Debug("tasks to be created.." + taskstobecreated.Count());
                    if (taskstobecreated != null && taskstobecreated.Count() > 0)
                    {                        
                        foreach (var user in taskstobecreated)
                        {
                            if((user.RoleId == AppRole.Reviewer && workflow.IsReviewRequired == true))
                            {
                                var taskDetails = DataCache.ToDoTasks.Where(x => x.FileId == file.Id && x.UserRoleId == user.RoleId && x.UserId == user.UserId);
                                if (taskDetails == null || taskDetails.Count() == 0)
                                {
                                    Models.ToDoTask task = new Models.ToDoTask();
                                    task.DataRoomId = dataroom.Id;
                                    task.DataRoomName = dataroom.DataRoomName;
                                    task.FolderId = file.FolderId;
                                    task.FolderName = file.FolderName;
                                    task.FileId = file.Id;
                                    task.FileName = file.FileName;
                                    task.TaskCategory = "Review";
                                    task.UserId = user.UserId;
                                    task.UserName = user.UserName;
                                    task.UserRoleId = user.RoleId;
                                    task.CreatedBy = file.CreatedBy;
                                    task.CreatorName = file.CreatorName;
                                    task.CreatedOn = file.CreatedOn;
                                    logger.Debug("TaskCategory.." + task.TaskCategory);
                                    task.Id = await _service.SaveTask(task);
                                    new Thread(() => DataCache.RefreshSingleToDoTask(task)).Start();
                                }
                            }
                        }
                    }
                }
                //else if(workflow.IsApprovalRequired == true && file.Status == "Reviewed")
                //{
                //    var approvers = DataCache.DataRoomWorkFlowUsers.Where(x => x.IsActive == true && x.DataRoomId == dataroom.Id && x.WorkFlowId == workflow.Id && x.RoleId == AppRole.Approver);
                //    if (approvers != null && approvers.Count() > 0)
                //    {
                //        foreach (var approver in approvers)
                //        {
                //            Models.ToDoTask task = new Models.ToDoTask();
                //            task.DataRoomId = dataroom.Id;
                //            task.DataRoomName = dataroom.DataRoomName;
                //            task.FolderId = file.FolderId;
                //            task.FolderName = file.FolderName;
                //            task.FileId = file.Id;
                //            task.FileName = file.FileName;
                //            task.TaskCategory = "Approval";
                //            task.UserId = approver.UserId;
                //            task.UserName = approver.UserName;
                //            task.UserRoleId = AppRole.Approver;
                //            task.CreatedBy = file.CreatedBy;
                //            task.CreatorName = file.CreatorName;
                //            task.CreatedOn = file.CreatedOn;
                //            task.Id = await _service.SaveTask(task);
                //            new Thread(() => DataCache.RefreshSingleToDoTask(task)).Start();
                //        }
                //    }
                //}
            }
            catch(Exception ex)
            {
                logger.Error("Exception in Task Creation.." + ex.Message + "----" + ex.StackTrace);
                throw ex;
            }
            logger.Debug("Exit into Task Creation when file submits..");
        }

        public async Task CreateApprovalTask(Models.File file)
        {
            try
            {
                var dataroom = DataCache.DataRooms.First(x => x.Id == file.DataRoomId);
                var foderdetails = DataCache.Folders.Where(x => x.Id == file.FolderId);
                Models.Folder folder = new Models.Folder();
                if (foderdetails != null && foderdetails.Count() > 0)
                {
                    folder = foderdetails.First();
                }
                var workflow = DataCache.WorkFlows.First(x => x.Id == folder.WorkFlowId && x.IsActive == true);
                if (file.Status == "Reviewed")
                {
                    var taskstobecreated = DataCache.DataRoomWorkFlowUsers.Where(x => x.IsActive == true && x.WorkFlowId == workflow.Id && x.DataRoomId == dataroom.Id && x.RoleId == AppRole.Approver);                    
                    if (taskstobecreated != null && taskstobecreated.Count() > 0)
                    {

                        foreach (var user in taskstobecreated)
                        {
                            if (user.RoleId == AppRole.Approver && workflow.IsApprovalRequired == true)
                            {
                                var taskDetails = DataCache.ToDoTasks.Where(x => x.FileId == file.Id && x.UserRoleId == user.RoleId && x.UserId == user.UserId);
                                if(taskDetails == null || taskDetails.Count() == 0)
                                {
                                    Models.ToDoTask task = new Models.ToDoTask();
                                    task.DataRoomId = dataroom.Id;
                                    task.DataRoomName = dataroom.DataRoomName;
                                    task.FolderId = file.FolderId;
                                    task.FolderName = file.FolderName;
                                    task.FileId = file.Id;
                                    task.FileName = file.FileName;
                                    task.TaskCategory = "Approval";
                                    task.UserId = user.UserId;
                                    task.UserName = user.UserName;
                                    task.UserRoleId = user.RoleId;
                                    task.CreatedBy = file.CreatedBy;
                                    task.CreatorName = file.CreatorName;
                                    task.CreatedOn = file.CreatedOn;
                                    task.Id = await _service.SaveTask(task);
                                    new Thread(() => DataCache.RefreshSingleToDoTask(task)).Start();
                                }
                            }
                        }
                    }
                }               
            }
            catch (Exception ex)
            {
                logger.Error("Exception in Task Creation.." + ex.Message + "----" + ex.StackTrace);
                throw ex;
            }
            logger.Debug("Exit into Task Creation when file submits..");
        }

        public async Task DeleteTask(int id)
        {
            try
            {
                Models.ToDoTask task = DataCache.ToDoTasks.First(x => x.Id == id);
                await _service.DeleteTask(task);
                new Thread(() => DataCache.RemoveToDoTaskfromCache(task)).Start();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}