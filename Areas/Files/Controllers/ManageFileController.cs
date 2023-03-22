
using DataRooms.UI.Areas.Files.Models;
using DataRooms.UI.Code;
using DataRooms.UI.Code.Email;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
//using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Files.Controllers
{
    [SessionExpire]
    public class ManageFileController : Controller
    {
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        private FileService _service { get; set; }
        private FilePermissionService _filePermissionService { get; set; }
        private AuditLogService _auditLogService { get; set; }
        private DataRoomService _dataroomService { get; set; }
        PermissionManager _permissionManager;
        TaskManager _taskManager;
        FileManager _fileManager;
        FileEncryption encryption;
        SendEmail _emailSender;
        private LogService _logger { get; set; }
        public string _workspacepath = string.Empty;
        int companyId = 0;
        string companyName = "";
        int loggedInUser = 0;
        string loggedInUserName = "";
        public ManageFileController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new FileService(token);
            _dataroomService = new DataRoomService(token);
            _logger = new LogService(token);
            _auditLogService = new AuditLogService(token);
            _filePermissionService = new FilePermissionService(token);
            _workspacepath = ConfigurationManager.AppSettings["WorkspacePath"];
            companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            companyName = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);
            loggedInUser = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
            loggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
            _permissionManager = new PermissionManager(token);
            _taskManager = new TaskManager(token);
            _fileManager = new FileManager(token, companyId);
            encryption = new FileEncryption();
            _emailSender = new SendEmail();
        }


        //[HttpGet]
        //public async Task<ActionResult> List()
        //{
        //    try
        //    {
        //        IEnumerable<File> files = await GetFilesAsync();
        //        return View(files);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult> List(string searchString)
        //{
        //    try
        //    {
        //        IEnumerable<File> files = await GetFilesAsync();
        //        return View(files);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public ActionResult SaveFilesinTemp(string guid)
        {
            if (Request.Files != null && Request.Files.Count > 0)
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase uploadedFile = Request.Files[fileName];
                    var directoryPath = Server.MapPath("~/") + "Temp/" + guid;
                    if (!System.IO.Directory.Exists(directoryPath))
                    {
                        System.IO.Directory.CreateDirectory(directoryPath);
                    }                    
                    var filepath = directoryPath + "/" + uploadedFile.FileName;
                    uploadedFile.SaveAs(filepath);
                    
                    //var encryptedPath = Server.MapPath("~/") + "Temp/EncryptionTemp/" + uploadedFile.FileName;
                    //encryption.EncryptFile(filepath, encryptedPath);
                    //if (System.IO.File.Exists(filepath))
                    //{
                    //    System.IO.File.Delete(filepath);
                    //    System.IO.File.Move(encryptedPath, filepath);
                    //}
                }
            }
            return Json("");
        }



        [HttpGet]
        public JsonResult CheckforWorkFlowUsers(int folderid)
        {
            try
            {
                var folderdetails = DataCache.Folders.Where(x => x.Id == folderid);
                Folder folder = new Folder();
                if (folderdetails != null && folderdetails.Count() > 0)
                {
                    folder = folderdetails.First();
                    if (folder.WorkFlowId > 0)
                    {
                        var workflowusers = DataCache.DataRoomWorkFlowUsers.Where(x => x.WorkFlowId == folder.WorkFlowId && x.DataRoomId == folder.DataRoomId && x.IsActive == true);
                        if (workflowusers == null || workflowusers.Count() == 0)
                        {
                            return Json("Work Flow Users are not found",JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(/*File file*/)
        {            
            string fileNameforSaveDisplay = string.Empty;
            logger.Debug("File Move Starts.." + DateTime.Now);
            int activitylogid = 0;
            File file = new File();
            file.DataRoomId = Convert.ToInt32(Request.Form["DataRoomId"]);
            file.DataRoomName = Request.Form["DataRoomName"];
            file.FolderId = Convert.ToInt32(Request.Form["FolderId"]);
            file.FolderName = Request.Form["FolderName"];
            file.FileDescription = Request.Form["FileDescription"];
            file.Guid = Request.Form["Guid"];
            file.Status = Request.Form["Status"];
            try
            {
                var folderdetails = DataCache.Folders.Where(x => x.Id == file.FolderId);
                Folder folder = new Folder();
                bool isLoggedInUserApprover = false;
                if(folderdetails != null && folderdetails.Count() > 0)
                {
                    folder = folderdetails.First();
                    if (folder.WorkFlowId > 0)
                    {
                        var workflowusers = DataCache.DataRoomWorkFlowUsers.Where(x => x.WorkFlowId == folder.WorkFlowId && x.DataRoomId == folder.DataRoomId && x.IsActive == true);
                        if (workflowusers == null || workflowusers.Count() == 0)
                        {
                            return Json("Work Flow Users are not found");
                        }
                        else
                        {
                            // Check logged in user is approver or not for this file..
                            var approvers = workflowusers.Where(x => x.RoleId == AppRole.Approver && x.UserId == loggedInUser);
                            if(approvers !=null && approvers.Count() > 0)
                            {
                                isLoggedInUserApprover = true;
                            }
                        }
                    }
                }
                var directoryPath = Server.MapPath("~/") + "Temp/" + file.Guid;
                if (System.IO.Directory.Exists(directoryPath))
                {
                    string[] filePaths = System.IO.Directory.GetFiles(directoryPath);
                    if(filePaths!=null && filePaths.Count() > 0)
                    {
                        foreach(var filepath in filePaths)
                        {
                            var encryptedPath = Server.MapPath("~/") + "Temp/EncryptionTemp/" + System.IO.Path.GetFileName(filepath);
                            encryption.EncryptFile(filepath, encryptedPath);
                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                                System.IO.File.Move(encryptedPath, filepath);
                            }



                            System.IO.FileInfo flinfo = new System.IO.FileInfo(filepath);
                            File fl = new File();
                            Guid guid = Guid.NewGuid();
                            fl.Guid = guid.ToString();
                            fl.IsWorkFlowRequired = file.IsWorkFlowRequired;
                            fl.IsActive = true;
                            fl.ContentType = flinfo.Extension;
                            fl.FileSize = flinfo.Length.ToString();
                            fl.DataRoomId = file.DataRoomId;
                            fl.DataRoomName = file.DataRoomName;
                            fl.FolderId = file.FolderId;
                            fl.FolderName = file.FolderName;
                            fl.FileName = flinfo.Name;
                            fileNameforSaveDisplay = fl.FileName;
                            fl.FileDescription = file.FileDescription;                            
                            fl.CreatedBy = loggedInUser;
                            fl.CreatorName = loggedInUserName;
                            fl.CreatedOn = DateTime.Now;
                            fl.Status = file.Status;
                            if(folder.WorkFlowId > 0)
                            {
                                fl.IsWorkFlowRequired = true;
                            }
                            else
                            {
                                fl.IsWorkFlowRequired = false;
                            }
                            fl.FileVersion = "1.0";
                            fl.CompanyId = companyId;
                            fl.CompanyName = companyName;
                            if (folder.WorkFlowId > 0)
                            {
                                fl.Status = "Submitted";
                                if (isLoggedInUserApprover)
                                {
                                    fl.Status = "Approved";
                                }
                            }
                            if (isLoggedInUserApprover)
                            {
                                fl.IsApproved = true;
                                fl.ApprovedBy = Convert.ToInt32(Session["UserId"]);
                                fl.ApprovedOn = DateTime.Now;
                            }
                            else
                            {
                                fl.IsApproved = null;
                                fl.ApprovedBy = null;
                                fl.ApprovedOn = null;
                            }
                            int fileid = await _service.UploadFile(fl);
                            fl.Id = fileid;
                            string relativePath = _fileManager.CopyFilefromTemptoWorkSpace(filepath, file.FolderId, file.DataRoomId, fl.Id.ToString());
                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                                GC.Collect();
                            }
                            fl.RelativePath = relativePath;
                            // Formatting File Path
                            fl.RelativePath = fl.RelativePath.Replace(@"\\", @"\");
                            fl.RelativePath = fl.RelativePath.Replace(@"//", @"\");
                            fl.RelativePath = fl.RelativePath.Replace(@"/", @"\");
                            await _service.UpdateFile(fl);
                            new Thread(() => DataCache.RefreshSingleFile(fl)).Start();
                            activitylogid = await _logger.LogActivity(fl.CompanyId,"File Upload", "File - " + fl.FileName + " has been uploaded in " + fl.FolderName + " under sharbox - " + fl.DataRoomName, dataroomid: fl.DataRoomId, dataroomname: fl.DataRoomName, folderid: folder.Id, foldername: folder.FolderName, fileid: fl.Id, filename: fl.FileName);
                            
                            await (_permissionManager.ApplyCreatorPermissionstoFile(fl));
                            if (fl.FolderId > 0)
                                await (_permissionManager.ApplyFolderPermissionstoFile(fl));
                            else
                                await (_permissionManager.ApplyDataRoomPermissionstoFile(fl));
                            if (folder.WorkFlowId > 0)
                            {
                                AuditLog log = new AuditLog();
                                log.AuditorId = loggedInUser;
                                log.AuditorName = loggedInUserName;
                                log.FileId = fl.Id;
                                log.FileName = fl.FileName;
                                log.Status = "Submitted";
                                if (isLoggedInUserApprover)
                                {
                                    log.Status = "Approved";
                                }
                                log.AuditOn = DateTime.Now;
                                log.Comments = fl.FileDescription;
                                await _auditLogService.SaveAuditLog(log);
                                if (!isLoggedInUserApprover)
                                    await _taskManager.CreateTask(fl);
                            }

                            if (Session["SelectedFiles"] == null)
                            {
                                List<DataRooms.UI.Models.File> files = new List<File>();
                                files.Add(fl);
                                Session["SelectedFiles"] = files;
                            }
                            else
                            {
                                List<DataRooms.UI.Models.File> files = Session["SelectedFiles"] as List<DataRooms.UI.Models.File>;
                                files.Add(fl);
                                Session["SelectedFiles"] = files;
                            }
                            break;
                        }
                    }
                }
                try
                {
                    if (System.IO.Directory.Exists(directoryPath))
                    {
                        int count = System.IO.Directory.GetFiles(directoryPath).Length;
                        if(count == 0)
                        {
                            System.IO.Directory.Delete(directoryPath, true);
                            GC.Collect();

                            await _fileManager.UpdateDataRoomSize(activitylogid, file.DataRoomId, 0, true);

                            if (Session["SelectedFiles"] != null)
                            {
                                List<DataRooms.UI.Models.File> files = Session["SelectedFiles"] as List<DataRooms.UI.Models.File>;
                                if (files != null && files.Count() > 0)
                                {
                                    var fileItem = files[0];
                                    if (fileItem.IsWorkFlowRequired == false || fileItem.IsWorkFlowRequired == null)
                                    {
                                        new Thread(() => _emailSender.SendEmailtoUser("FileOperation", fileid: fileItem.Id, statusFlag: "BulkDocumentsAdded", files: files)).Start();
                                    }
                                    else
                                    {
                                        var tasks = DataCache.ToDoTasks.Where(x => x.FileId == fileItem.Id);
                                        if (tasks != null && tasks.Count() > 0)
                                        {
                                            tasks = tasks.OrderBy(x => x.Id);
                                            new Thread(() => _emailSender.SendEmailtoUser("WorkFlow", userid: tasks.First().UserId, fileid: fileItem.Id, statusFlag: "BulkDocumentsSubmitted", files: files)).Start();
                                        }
                                    }

                                }

                                Session["SelectedFiles"] = null;
                            }
                        }
                        //System.IO.Directory.Delete(directoryPath, true);
                        //GC.Collect();
                    }
                }
                catch(Exception e)
                {
                    // throw e;
                }
                
            }
            catch (Exception ex)
            {
                logger.Error("Exception Occurs..." + ex.Message + "...." + ex.StackTrace);
                throw ex;
            }
            logger.Debug("File Move Ends.." + DateTime.Now);
            return Json(fileNameforSaveDisplay);
        }

        public JsonResult SaveFiletoServer()
        {
            return Json("Success");
        }

        public JsonResult GetUploadedFilesCount(string guid,int totalfilecount)
        {
            try
            {
                var directoryPath = Server.MapPath("~/") + "Temp/" + guid;
                if (System.IO.Directory.Exists(directoryPath))
                {
                    string[] filePaths = System.IO.Directory.GetFiles(directoryPath);
                    if(filePaths!=null && filePaths.Count() > 0)
                    {
                        return Json(filePaths.Count(), JsonRequestBehavior.AllowGet);
                    }
                    return Json(0,JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        
        public async Task<ActionResult> ViewFile(int fileid)
        {
            try
            {
                FileCustomModel model = new FileCustomModel();
                model.File = DataCache.Files.First(x => x.Id == fileid);
                model.AuditLogs = await _auditLogService.GetAuditLogs(fileid);
                model.WaitingWith = await _service.GetWaitingWith(fileid);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~/Areas/Files/Views/Shared/_viewfile.cshtml", model)
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

        [HttpGet]
        public async Task<ActionResult> EditFileforSubmission(int fileid)
        {
            try
            {
                FileCustomModel model = new FileCustomModel();
                model.File = DataCache.Files.First(x => x.Id == fileid);
                model.AuditLogs = await _auditLogService.GetAuditLogs(fileid);
                model.WaitingWith = await _service.GetWaitingWith(fileid);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~/Areas/Files/Views/Shared/_editfile.cshtml", model)
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
        public async Task<ActionResult> CheckInFile()
        {
            int activitylogid = 0;
            try
            {
                File file = new File();
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    foreach (string fileName in Request.Files)
                    {
                        logger.Debug("File CheckIn Started...");
                        HttpPostedFileBase uploadedFile = Request.Files[fileName];
                        var fileid = Convert.ToInt32(Request.Form["FileId"]);
                        logger.Debug("File Id..." + fileid);
                        file = DataCache.Files.First(x => x.Id == fileid);
                        var modified = file.Clone();
                        var folder = new Folder();
                        var folderDetails = DataCache.Folders.Where(x => x.Id == file.FolderId);
                        if(folderDetails!=null && folderDetails.Count() > 0)
                        {
                            folder = folderDetails.First();
                        }
                        bool isLoggedInUserApprover = false;
                        logger.Debug("Work FLow Checking...");
                        var workflowapproverusers = DataCache.DataRoomWorkFlowUsers.Where(x => x.WorkFlowId == folder.WorkFlowId && x.DataRoomId == folder.DataRoomId && x.UserId == loggedInUser && x.RoleId == AppRole.Approver && x.IsActive == true);
                        if (workflowapproverusers != null && workflowapproverusers.Count() > 0)
                        {
                            isLoggedInUserApprover = true;
                        }
                        if (folder.WorkFlowId > 0)
                        {
                            modified.Status = "Submitted";
                            if (isLoggedInUserApprover == true)
                            {
                                modified.Status = "Approved";
                            }
                            modified.IsWorkFlowRequired = true;
                        }
                        else
                        {
                            modified.Status = "Submitted";
                        }
                        modified.IsCheckIn = true;
                        modified.CheckInBy = Convert.ToInt32(Session["UserId"]);
                        modified.CheckInByName = Convert.ToString(Session["UserName"]);
                        modified.CheckInOn = DateTime.Now;
                        modified.IsCheckOut = null;
                        modified.CheckOutBy = null;
                        modified.CheckOutByName = null;
                        modified.CheckOutOn = null;
                        double originalFileVersion = string.IsNullOrEmpty(file.FileVersion) ? 1 : Convert.ToDouble(file.FileVersion);
                        originalFileVersion = originalFileVersion + 1.0;
                        string filepath = _fileManager.SaveFiletoWorkSpace(uploadedFile, modified.FolderId, Convert.ToInt32(modified.DataRoomId), file.Id + "_" + originalFileVersion);
                        //System.IO.FileInfo fileinfo = new System.IO.FileInfo(filepath);
                        modified.ContentType = System.IO.Path.GetExtension(uploadedFile.FileName);
                        modified.FileSize = uploadedFile.ContentLength.ToString();
                        modified.RelativePath = filepath;
                        modified.FileName = uploadedFile.FileName;
                        modified.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                        modified.ModifierName = Convert.ToString(Session["UserName"]);
                        modified.ModifiedOn = DateTime.Now;
                        if (isLoggedInUserApprover)
                        {
                            modified.IsApproved = true;
                            modified.ApprovedBy = Convert.ToInt32(Session["UserId"]);
                            modified.ApprovedOn = DateTime.Now;
                        }
                        else
                        {
                            modified.IsApproved = null;
                            modified.ApprovedBy = null;
                            modified.ApprovedOn = null;
                        }
                        
                        modified.FileVersion = Convert.ToString(originalFileVersion) + ".0";
                        logger.Debug("File Version Saving Started...");
                        await SaveFileVersion(file);
                        logger.Debug("File Version Saving Completed...");
                        await _service.UpdateFile(modified);
                        logger.Debug("File Update Completed...");
                        new Thread(() => DataCache.RefreshSingleFile(modified)).Start();
                        logger.Debug("Data Cache Refresh...");
                        if (modified.IsWorkFlowRequired == true)
                        {
                            var todotasks = DataCache.ToDoTasks.Where(x => x.FileId == modified.Id);
                            if (todotasks != null && todotasks.Count() > 0)
                            {
                                foreach (var task in todotasks)
                                {
                                    await _taskManager.DeleteTask(task.Id);
                                }
                            }
                            if(!isLoggedInUserApprover)
                            await _taskManager.CreateTask(modified);
                        }
                        activitylogid = await _logger.LogActivity(modified.CompanyId,"File Check-In", "File - " + modified.FileName + " has been checked in " + modified.FolderName + " under sharbox - " + modified.DataRoomName, dataroomid: modified.DataRoomId, dataroomname: modified.DataRoomName, folderid: modified.Id, foldername: modified.FolderName, fileid: modified.Id, filename: modified.FileName);
                        await _logger.LogDataDifference(activitylogid, modified, file, dataRoomId: file.DataRoomId);
                        await _fileManager.UpdateDataRoomSize(activitylogid, file.DataRoomId, Convert.ToInt32(file.FileSize), true);
                        if (modified.IsWorkFlowRequired == false || modified.IsWorkFlowRequired == null)
                        {
                            logger.Debug("Email Send...");
                            _emailSender.SendEmailtoUser("FileOperation", fileid: modified.Id, statusFlag: "DocumentCheckIn");
                        }
                        else
                        {
                            var tasks = DataCache.ToDoTasks.Where(x => x.FileId == modified.Id);
                            if (tasks != null && tasks.Count() > 0)
                            {
                                tasks = tasks.OrderBy(x => x.Id);
                                _emailSender.SendEmailtoUser("WorkFlow", userid: tasks.First().UserId, fileid: modified.Id, statusFlag: "Submitted");
                            }
                        }
                    }
                }
                TempData["Notification"] = "File Saved Successfully";
                return Json(file.FolderId);
            }
            catch (Exception ex)
            {
                logger.Error("Error in File Check In..." + ex.Message + "....." + ex.StackTrace,ex);
                throw ex;
            }
        }

        public async Task SaveFileVersion(File file)
        {
            try
            {
                FileVersion fileVersion = new FileVersion();
                fileVersion.FileId = file.Id;
                fileVersion.FileName = file.FileName;
                fileVersion.DataRoomId = file.DataRoomId;
                fileVersion.DataRoomName = file.DataRoomName;
                fileVersion.FolderId = file.FolderId;
                fileVersion.FolderName = file.FolderName;
                fileVersion.IsActive = file.IsActive;
                fileVersion.IsArchived = file.IsArchived;
                fileVersion.IsDeleted = file.IsDeleted;
                fileVersion.CreatedBy = file.CreatedBy;
                fileVersion.CreatedOn = file.CreatedOn;
                fileVersion.CreatorName = file.CreatorName;
                fileVersion.ModifiedBy = file.ModifiedBy;
                fileVersion.ModifierName = file.ModifierName;
                fileVersion.ModifiedOn = file.ModifiedOn;
                fileVersion.ArchivedOn = file.ArchivedOn;
                fileVersion.FileSize = file.FileSize;
                fileVersion.ContentType = file.ContentType;
                fileVersion.RelativePath = file.RelativePath;
                fileVersion.FileVersionNumber = file.FileVersion;
                fileVersion.CompanyId = file.CompanyId;
                fileVersion.CompanyName = file.CompanyName;
                fileVersion.Guid = file.Guid;
                await _logger.LogActivity(file.CompanyId,"File Version Creation", "File Version - " + file.FileVersion + " created for file - " + file.FileName, file.DataRoomId, file.FolderId, file.Id, file.DataRoomName, file.FolderName, file.FileName);
                await _service.SaveFileVersion(fileVersion);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditFileforSubmission(File fl,HttpPostedFileBase uploadedfile)
        {
            int activityLogId = 0;
            try
            {
                var file = DataCache.Files.First(x => x.Id == fl.Id);
                file.FileDescription = fl.FileDescription;
                file.Status = fl.Status;
                var dataroom = DataCache.DataRooms.First(x => x.Id == file.DataRoomId);
                var folderdetails = DataCache.Folders.Where(x => x.Id == file.FolderId);
                if(folderdetails!=null && folderdetails.Count() > 0)
                {
                    var folder = folderdetails.First();
                    if (folder.WorkFlowId > 0 && file.Status != "Save")
                    {
                        var workflowusers = DataCache.DataRoomWorkFlowUsers.Where(x => x.DataRoomId == folder.DataRoomId && x.IsActive == true);
                        if (workflowusers == null || workflowusers.Count() == 0)
                        {
                            return Json("Work Flow Users are not found");
                        }
                    }
                }
                
                if (file.Status == "Submitted")
                {
                    if (uploadedfile != null)
                    {
                        string relativePath = _fileManager.SaveFiletoWorkSpace(uploadedfile, file.FolderId, file.DataRoomId);
                        System.IO.FileInfo flinfo = new System.IO.FileInfo(_workspacepath + "/" + relativePath);
                        file.IsActive = true;
                        file.ContentType = flinfo.Extension;
                        file.FileSize = flinfo.Length.ToString();
                        file.FileName = flinfo.Name;
                        file.RelativePath = relativePath;
                    }                    
                    file.ModifiedBy = loggedInUser;
                    file.ModifierName = loggedInUserName;
                    file.ModifiedOn = DateTime.Now;
                    file.FileVersion = "1";                    
                    await _service.UpdateFile(file);
                    new Thread(() => DataCache.RefreshSingleFile(file)).Start();
                    activityLogId = await _logger.LogActivity(file.CompanyId,"Edit File", "File - " + file.FileName + " has been modified", dataroomid: file.DataRoomId, dataroomname: file.DataRoomName, folderid: file.Id, foldername: file.FolderName, fileid: file.Id, filename: file.FileName);
                    await _fileManager.UpdateDataRoomSize(activityLogId, file.DataRoomId, Convert.ToInt32(file.FileSize), true);
                    if (file.IsWorkFlowRequired == true && fl.Status == "Submitted")
                    {
                        AuditLog log = new AuditLog();
                        log.AuditorId = loggedInUser;
                        log.AuditorName = loggedInUserName;
                        log.FileId = fl.Id;
                        log.FileName = fl.FileName;
                        log.Status = "Submitted";
                        log.AuditOn = DateTime.Now;
                        log.Comments = fl.FileDescription;
                        await _auditLogService.SaveAuditLog(log);
                        await _taskManager.CreateTask(file);
                    }
                }
                else
                {
                     await _service.UpdateFile(file);
                }
                return Json("");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        

        public void DeleteFilefromWorkSpace(string relativepath)
        {
            try
            {
                string filepath = System.IO.Path.Combine(_workspacepath, relativepath);
                if (System.IO.Directory.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetFileSize(string filepath)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = new System.IO.FileInfo(filepath).Length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var file = DataCache.Files.Single(x => x.Id == id);                
                file.IsActive = false;
                file.DeletedBy = Convert.ToInt32(Session["UserId"]);
                file.DeletorName = Convert.ToString(Session["UserName"]);
                file.DeletedOn = DateTime.Now;
                await _service.UpdateFile(file);
                new Thread(() => DataCache.RefreshSingleFile(file)).Start();
                //DeleteFilefromWorkSpace(file.RelativePath);
                await _logger.LogActivity(file.CompanyId,"File Delete", "File - " + file.FileName + " has been deleted in " + file.FolderName + " under sharbox - " + file.DataRoomName, dataroomid: file.DataRoomId, dataroomname: file.DataRoomName, folderid: file.Id, foldername: file.FolderName, fileid: file.Id, filename: file.FileName);
                _emailSender.SendEmailtoUser("FileOperation", fileid: file.Id, statusFlag: "DocumentDelete");
                TempData["Notification"] = "File Deleted Successfully";
                return RedirectToAction("Index", "FileExplorer", new { area = "Explorer", dataroomid = file.DataRoomId, folderid = file.FolderId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult CheckforFileContent(int fileid)
        {
            try
            {
                var file = DataCache.Files.Single(x => x.Id == fileid);
                string fullfilepath = file.RelativePath;
                byte[] filebyteArray = _fileManager.GetFileByteArray(fullfilepath);
                return Json(filebyteArray != null ? "File Found" : "File Not Found",JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("File Not Found",JsonRequestBehavior.AllowGet);
            }
        }
        public async Task DownloadFile(int fileid)
        {
            try
            {
                var file = DataCache.Files.Single(x => x.Id == fileid);
                string fullfilepath = file.RelativePath;
                //string[] fileparts = fullfilepath.Split('\\');
                //fileparts[fileparts.Length - 1] = file.FileName;
                byte[] filebyteArray = _fileManager.GetFileByteArray(fullfilepath);//string.Join(@"\", fileparts)
                if (filebyteArray != null)
                {
                    string tempfilepath = Server.MapPath("~/Temp/") + file.FileName;
                    System.IO.File.WriteAllBytes(tempfilepath, filebyteArray);
                    var decryptedfilepath = Server.MapPath("~/Temp/EncryptionTemp/") + file.FileName;
                    encryption.DecryptFile(tempfilepath, decryptedfilepath);
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=" + file.FileName);
                    byte[] data = System.IO.File.ReadAllBytes(decryptedfilepath);
                    response.BinaryWrite(data);
                    response.End();
                    await _logger.LogActivity(file.CompanyId,"File Download", "File - " + file.FileName + " has been downloaded from " + file.FolderName + " under sharbox - " + file.DataRoomName, dataroomid: file.DataRoomId, dataroomname: file.DataRoomName, folderid: file.Id, foldername: file.FolderName, fileid: file.Id, filename: file.FileName);
                    
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<JsonResult> CheckforFileVersionContent(int fileid, int fileversionid)
        {
            try
            {
                bool isFileExists = false;
                var fileversions = await _service.GetFileVersions(fileid);
                if (fileversions != null)
                {
                    if (fileversions.Select(x => x.Id).Contains(fileversionid))
                    {
                        var file = fileversions.First(x => x.Id == fileversionid);
                        string fullfilepath = file.RelativePath;
                        byte[] filebyteArray = _fileManager.GetFileByteArray(fullfilepath);
                        if (filebyteArray != null)
                            isFileExists = true;
                    }
                }
                return Json(isFileExists == true ? "File Found" : "File Not Found", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("File Not Found", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task DownloadFileVersion(int fileid,int fileversionid)
        {
            try
            {
                var fileversions = await _service.GetFileVersions(fileid);
                if (fileversions != null)
                {
                    if (fileversions.Select(x => x.Id).Contains(fileversionid))
                    {
                        var file = fileversions.First(x => x.Id == fileversionid);
                        string fullfilepath = file.RelativePath;
                        byte[] filebyteArray = _fileManager.GetFileByteArray(fullfilepath);
                        if (filebyteArray != null)
                        {
                            string tempfilepath = Server.MapPath("~/Temp/") + file.FileName;
                            System.IO.File.WriteAllBytes(tempfilepath, filebyteArray);
                            var decryptedfilepath = Server.MapPath("~/Temp/EncryptionTemp/") + file.FileName;
                            encryption.DecryptFile(tempfilepath, decryptedfilepath);
                            HttpResponse response = System.Web.HttpContext.Current.Response;
                            response.Clear();
                            response.ClearContent();
                            response.ClearHeaders();
                            response.Buffer = true;
                            response.AddHeader("Content-Disposition", "attachment;filename=" + file.FileName);
                            byte[] data = System.IO.File.ReadAllBytes(decryptedfilepath);
                            response.BinaryWrite(data);
                            response.End();
                            await _logger.LogActivity(file.CompanyId,"File Download", "File - " + file.FileName + " has been downloaded from " + file.FolderName + " under sharbox - " + file.DataRoomName, dataroomid: file.DataRoomId, dataroomname: file.DataRoomName, folderid: file.Id, foldername: file.FolderName, fileid: file.Id, filename: file.FileName);
                   
                        }
                        else
                        {
                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> CheckOutFile(int fileid)
        {
            int activitylogid = 0;
            try
            {
                File file = DataCache.Files.Single(x => x.Id == fileid);
                if(file.IsCheckOut == true && (file.IsCheckIn == false || file.IsCheckIn == null))
                {
                    return Json("File - " + file.FileName + " have been checked out by " + file.CheckOutByName + " on " + file.CheckOutOn.ToApplicationFormat());
                }
                var modified = file.Clone();
                modified.IsCheckOut = true;
                modified.CheckOutBy = Convert.ToInt32(Session["UserId"]);
                modified.CheckOutByName = Convert.ToString(Session["UserName"]);
                modified.CheckOutOn = DateTime.Now;
                modified.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                modified.ModifierName = Convert.ToString(Session["UserName"]);
                modified.ModifiedOn = DateTime.Now;
                await _service.UpdateFile(modified);
                new Thread(() => DataCache.RefreshSingleFile(modified)).Start();
                activitylogid = await _logger.LogActivity(modified.CompanyId,"File Upload", "File - " + modified.FileName + " has been uploaded in " + modified.FolderName + " under sharbox - " + modified.DataRoomName, dataroomid: modified.DataRoomId, dataroomname: modified.DataRoomName, folderid: modified.Id, foldername: modified.FolderName, fileid: modified.Id, filename: modified.FileName);
                await _logger.LogDataDifference(activitylogid, file, modified, dataRoomId: file.DataRoomId);
                _emailSender.SendEmailtoUser("FileOperation", fileid: modified.Id, statusFlag: "DocumentCheckOut");
                return Json(modified.FolderId);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> ShareFile(int fileid, string emailids)
        {
            try
            {
                File file = new File();
                DataRooms.UI.Models.Company company = new DataRooms.UI.Models.Company();
                DataRooms.UI.Models.DataRoom dataroom = new DataRooms.UI.Models.DataRoom();
                DataRooms.UI.Models.Folder folder = new DataRooms.UI.Models.Folder();
                var fileDetails = DataCache.Files.Where(x => x.Id == fileid);
                if(fileDetails != null && fileDetails.Count() > 0)
                {
                    file = fileDetails.First();
                    var companyDetails = DataCache.Companies.Where(x => x.Id == file.CompanyId);
                    if(companyDetails!=null && companyDetails.Count() > 0)
                    {
                        company = companyDetails.First();
                    }
                    var dataroomDetails = DataCache.DataRooms.Where(x => x.Id == file.DataRoomId);
                    if(dataroomDetails!=null && dataroomDetails.Count() > 0)
                    {
                        dataroom = dataroomDetails.First();
                    }
                    var folderDetails = DataCache.Folders.Where(x => x.Id == file.FolderId);
                    if (folderDetails != null && folderDetails.Count() > 0)
                    {
                        folder = folderDetails.First();
                    }
                }
                if (!string.IsNullOrEmpty(emailids))
                {
                    List<string> emails = emailids.Split(',').ToList();
                    foreach(var email in emails)
                    {
                        _emailSender.SendEmailtoUser("FileShare", fileid,loggedInUser:loggedInUserName, toEmail: email);
                        await _logger.LogActivity(company.Id, "File Share", "File - " + file.FileName + " shared to " + email + " by " + loggedInUserName, dataroomid: dataroom.Id, folderid: folder.Id, fileid: file.Id, dataroomname: dataroom.DataRoomName, foldername: folder.FolderName, filename: file.FileName);
                    }
                }
                return Json("File Shared Successfully",JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("Error occured in file sharing", JsonRequestBehavior.AllowGet);
            }
        }

        
    }
}