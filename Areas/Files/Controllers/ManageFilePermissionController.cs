using DataRooms.UI.Areas.Files.Models;
using DataRooms.UI.Areas.Folders.Model;
using DataRooms.UI.Code;
using DataRooms.UI.Code.Email;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Files.Controllers
{
    [SessionExpire]
    public class ManageFilePermissionController : Controller
    {
        private FilePermissionService _service { get; set; }
        private LogService _logger { get; set; }
        private PermissionManager _permissionManager;
        SendEmail _emailSender;
        public ManageFilePermissionController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new FilePermissionService(token);
            _logger = new LogService(token);
            _permissionManager = new PermissionManager(token);
            _emailSender = new SendEmail();
        }

        //[HttpGet]
        //public JsonResult GetDataRooms(string searchString)
        //{
        //    try
        //    {
        //        var datarooms = DataCache.DataRooms.Where(x => x.DataRoomName.ToLower().Contains(searchString.ToLower()));
        //        return Json(datarooms, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //[HttpGet]
        //public JsonResult GetFoldersbasedonDataRoom(string searchString, int dataroomid)
        //{
        //    try
        //    {
        //        var folders = DataCache.Folders.Where(x => x.DataRoomId == dataroomid && x.FolderName.ToLower().Contains(searchString.ToLower()));
        //        return Json(folders, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet]
        public JsonResult GetFilesbasedonDataRoomandFolder(string searchString, int dataroomid,int folderid)
        {
            try
            {
                var files = DataCache.Files.Where(x => x.DataRoomId == dataroomid && x.FolderId == folderid && x.FileName.ToLower().Contains(searchString.ToLower()));
                return Json(files, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetUsersBasedonDataRoom(string searchString, int dataroomid)
        {
            try
            {
                var dataroomusers = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == dataroomid).Select(x => x.UserId).ToList();
                var users = DataCache.Users.Where(x => dataroomusers.Contains(x.Id) && x.FullName.ToLower().Contains(searchString.ToLower()));
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult List(int fileid)
        {
            try
            {
                var model = new FileCustomModel();
                model.FileId = fileid;
                GetAllFilePermissionsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult List(FileCustomModel model)
        {
            try
            {
                GetAllFilePermissionsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetAllFilePermissionsAsync(FileCustomModel model)
        {
            try
            {
                model.File = DataCache.Files.First(x=>x.Id == model.FileId);
                model.SortColumn = string.IsNullOrEmpty(model.SortColumn) ? "CreatedOn" : model.SortColumn;
                IPagedList<FilePermission> filepermissions = null;
                IEnumerable<FilePermission> permissions = DataCache.FilePermissions.Where(x=>x.FileId == model.FileId);
                if(permissions!=null && permissions.Count() > 0)
                {
                    switch (model.SortColumn)
                    {
                        case "CreatedOn":
                            if (model.SortOrder.Equals("desc"))
                                filepermissions = permissions.OrderByDescending
                                        (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            else
                                filepermissions = permissions.OrderBy
                                        (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            break;
                        case "DataRoomName":
                            if (model.SortOrder.Equals("desc"))
                                filepermissions = permissions.OrderByDescending
                                        (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            else
                                filepermissions = permissions.OrderBy
                                        (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            break;
                        case "UserName":
                            if (model.SortOrder.Equals("desc"))
                                filepermissions = permissions.OrderByDescending
                                        (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            else
                                filepermissions = permissions.OrderBy
                                        (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            break;
                        case "Default":
                            filepermissions = permissions.OrderByDescending
                                (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            break;
                    }
                }
                else
                {
                    permissions = new List<FilePermission>();
                }
                model.PagedFilePermissions = filepermissions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id,int fileid)
        {
            try
            {
                var filepermission = new DataRooms.UI.Models.FilePermission();
                if (id > 0)
                    filepermission = DataCache.FilePermissions.Single(x => x.Id == id);
                else
                {
                    var file = DataCache.Files.First(x=>x.Id == fileid);
                    filepermission.FolderId = file.FolderId;
                    filepermission.FolderName = file.FolderName;
                    filepermission.DataRoomId = file.DataRoomId;
                    filepermission.DataRoomName = file.DataRoomName;
                    filepermission.FileId = file.Id;
                    filepermission.FileName = file.FileName;
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Files\Views\Shared\_editfilepermission.cshtml", filepermission)
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
        public async Task<ActionResult> Edit(DataRooms.UI.Models.FilePermission filePermission)
        {
            int activityid = 0;
            try
            {
                var permissiondetails = DataCache.FilePermissions.Where(x => x.FileId == filePermission.FileId && x.UserId == filePermission.UserId);
                if (permissiondetails.Count() > 0)
                {
                    filePermission.Id = permissiondetails.First().Id;
                }
                if (filePermission.Id > 0)
                {
                    var original = DataCache.FilePermissions.Single(x => x.Id == filePermission.Id);
                    var modified = original.Clone();

                    modified.IsActive = filePermission.IsActive;
                    modified.HasFullControl = filePermission.HasFullControl;
                    modified.HasRead = filePermission.HasRead;
                    modified.HasWrite = filePermission.HasWrite;
                    modified.HasDelete = filePermission.HasDelete;



                    modified.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    modified.ModifierName = Convert.ToString(Session["UserName"]);
                    modified.ModifiedOn = DateTime.Now;
                    await _service.UpdateFilePermission(modified);
                    activityid = await _logger.LogActivity(modified.CompanyId,"File Permission Modification", "File - " + modified.FileName + " permissions modified for user " + modified.UserName, dataroomid: modified.DataRoomId, dataroomname: modified.DataRoomName);
                    await _logger.LogDataDifference(activityid, original, modified, dataRoomId: original.DataRoomId);
                    //new Thread(() => DataCache.RefreshSingleFilePermission(modified)).Start();
                    DataCache.RefreshSingleFilePermission(modified);
                    await _permissionManager.ApplyPermissionstoFolderHierarchy(filePermission.FolderId, filePermission.UserId, filePermission.UserName);
                    string loggedInUser = Convert.ToString(Session["UserName"]);
                    _emailSender.SendEmailtoUser("UserPermission", userid: modified.UserId, objectType: "File", objectId: modified.FileId, loggedInUser: loggedInUser);
                    List<FilePermission> permissions = GetIndividualFilePermissions(modified.FileId);
                    JsonResult jr = Json(new
                    {
                        HTML = this.RenderPartialView(@"~\Areas\Files\Views\Shared\_singlefilepermissionlist.cshtml", permissions)
                    });
                    jr.MaxJsonLength = int.MaxValue;
                    jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return jr;
                }
                else
                {
                    filePermission.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    filePermission.CreatorName = Convert.ToString(Session["UserName"]);
                    filePermission.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    filePermission.CompanyName = Convert.ToString(Session["CompanyName"]);
                    filePermission.CreatedOn = DateTime.Now;
                    int id = await _service.SaveFilePermission(filePermission);
                    filePermission.Id = id;
                    await _logger.LogActivity(filePermission.CompanyId,"File Permission Creation", "File - " + filePermission.FileName + " permissions created for user " + filePermission.UserName, dataroomid: filePermission.DataRoomId, dataroomname: filePermission.DataRoomName);
                    //new Thread(() => DataCache.RefreshSingleFilePermission(filePermission)).Start();
                    DataCache.RefreshSingleFilePermission(filePermission);
                    await _permissionManager.ApplyPermissionstoFolderHierarchy(filePermission.FolderId, filePermission.UserId, filePermission.UserName);
                    string loggedInUser = Convert.ToString(Session["UserName"]);
                    _emailSender.SendEmailtoUser("UserPermission", userid: filePermission.UserId, objectType: "File", objectId: filePermission.FileId, loggedInUser: loggedInUser);
                    return Json("File Permission Saved Successfully");
                }

                
                //TempData["Notification"] = "File Permission Saved Successfully";
                //return RedirectToAction("List", "ManageFilePermission", new { area = "Files", fileid = filePermission.FileId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var filepermission = DataCache.FilePermissions.Single(x => x.Id == id);
                await _service.DeleteFilePermission(filepermission);
                //new Thread(() => DataCache.RemoveFilePermissionfromCache(filepermission)).Start();
                DataCache.RemoveFilePermissionfromCache(filepermission);
                await _logger.LogActivity(filepermission.CompanyId,"File Permission Deletion", "File - " + filepermission.FileName + " permissions deleted for user " + filepermission.UserName, dataroomid: filepermission.DataRoomId, dataroomname: filepermission.DataRoomName);

                string loggedInUser = Convert.ToString(Session["UserName"]);
                _emailSender.SendEmailtoUser("UserPermission", userid: filepermission.UserId, objectType: "File", objectId: filepermission.FileId, loggedInUser: loggedInUser);


                //TempData["Notification"] = "File Permission Deleted Successfully";
                //return RedirectToAction("List", "ManageFilePermission", new { area = "Files",fileid = filepermission.FileId });
                List<FilePermission> permissions = GetIndividualFilePermissions(filepermission.FileId);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Files\Views\Shared\_singlefilepermissionlist.cshtml", permissions)
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

        [HttpGet]
        public JsonResult GetFilePermissions(int fileid)
        {
            try
            {
                List<FilePermission> permissions = GetIndividualFilePermissions(fileid);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Files\Views\Shared\_filepermissionslist.cshtml", permissions)
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

        public List<FilePermission> GetIndividualFilePermissions(int fileid)
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            List<FilePermission> filePermissions = DataCache.FilePermissions.Where(x => x.CompanyId == companyId && x.FileId == fileid).ToList();
            return filePermissions;
        }
    }
}