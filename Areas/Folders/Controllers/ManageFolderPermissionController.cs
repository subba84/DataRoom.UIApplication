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

namespace DataRooms.UI.Areas.Folders.Controllers
{
    [SessionExpire]
    public class ManageFolderPermissionController : Controller
    {
        private FolderPermissionService _service { get; set; }
        private FilePermissionService _fileService { get; set; }
        private LogService _logger { get; set; }
        PermissionManager _permissionManager;
        SendEmail _emailSender;
        public ManageFolderPermissionController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new FolderPermissionService(token);
            _fileService = new FilePermissionService(token);
            _logger = new LogService(token);
            _permissionManager = new PermissionManager(token);
            _emailSender = new SendEmail();
        }

        [HttpGet]
        public JsonResult GetDataRooms(string searchString)
        {
            try
            {
                var datarooms = DataCache.DataRooms.Where(x => x.DataRoomName.ToLower().Contains(searchString.ToLower()));
                return Json(datarooms, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetFoldersbasedonDataRoom(string searchString,int dataroomid)
        {
            try
            {
                var folders = DataCache.Folders.Where(x => x.DataRoomId == dataroomid && x.FolderName.ToLower().Contains(searchString.ToLower()));
                return Json(folders, JsonRequestBehavior.AllowGet);
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
                //var dataroomusers = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == dataroomid).Select(x => x.UserId).ToList();
                int companyId = Convert.ToInt32(Session["CompanyId"]);
                var users = DataCache.Users.Where(x => x.CompanyId == companyId && x.FullName.ToLower().Contains(searchString.ToLower()));
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult List(int folderid)
        {
            try
            {
                var model = new FolderCustomModel();
                model.FolderId = folderid;
                GetAllFolderPermissionsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult List(FolderCustomModel model)
        {
            try
            {
                GetAllFolderPermissionsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetAllFolderPermissionsAsync(FolderCustomModel model)
        {
            try
            {
                model.Folder = DataCache.Folders.FirstOrDefault(x => x.Id == model.FolderId);
                model.SortColumn = string.IsNullOrEmpty(model.SortColumn) ? "CreatedOn" : model.SortColumn;
                IPagedList<FolderPermission> folderpermissions = null;
                IEnumerable<FolderPermission> permissions = DataCache.FolderPermissions.Where(x=>x.FolderId == model.FolderId);
                if(permissions!=null && permissions.Count() > 0)
                {
                    switch (model.SortColumn)
                    {
                        case "CreatedOn":
                            if (model.SortOrder.Equals("desc"))
                                folderpermissions = permissions.OrderByDescending
                                        (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            else
                                folderpermissions = permissions.OrderBy
                                        (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            break;
                        case "DataRoomName":
                            if (model.SortOrder.Equals("desc"))
                                folderpermissions = permissions.OrderByDescending
                                        (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            else
                                folderpermissions = permissions.OrderBy
                                        (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            break;
                        case "UserName":
                            if (model.SortOrder.Equals("desc"))
                                folderpermissions = permissions.OrderByDescending
                                        (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            else
                                folderpermissions = permissions.OrderBy
                                        (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            break;
                        case "Default":
                            folderpermissions = permissions.OrderByDescending
                                (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                            break;
                    }
                }
                else
                {
                    permissions = new List<FolderPermission>();
                }
                

                model.PagedFolderPermissions = folderpermissions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id,int folderid)
        {
            try
            {
                var folderpermission = new DataRooms.UI.Models.FolderPermission();
                if (id > 0)
                    folderpermission = DataCache.FolderPermissions.Single(x => x.Id == id);
                else
                {
                    var folder = DataCache.Folders.Single(x => x.Id == folderid);
                    folderpermission.FolderId = folder.Id;
                    folderpermission.FolderName = folder.FolderName;
                    folderpermission.DataRoomId = folder.DataRoomId;
                    folderpermission.DataRoomName = folder.DataRoomName;
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Folders\Views\Shared\_editfolderpermission.cshtml", folderpermission)
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
        public async Task<ActionResult> Edit(DataRooms.UI.Models.FolderPermission folderPermission)
        {
            int activityid = 0;
            try
            {
                var permissiondetails = DataCache.FolderPermissions.Where(x => x.FolderId == folderPermission.FolderId && x.UserId == folderPermission.UserId);
                if (permissiondetails.Count() > 0)
                {
                    folderPermission.Id = permissiondetails.First().Id;
                }
                if (folderPermission.Id > 0)
                {
                    var original = DataCache.FolderPermissions.Single(x => x.Id == folderPermission.Id);
                    var modified = original.Clone();
                    modified.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    modified.ModifierName = Convert.ToString(Session["UserName"]);
                    modified.ModifiedOn = DateTime.Now;
                    modified.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    modified.CompanyName = Convert.ToString(Session["CompanyName"]);
                    modified.IsActive = folderPermission.IsActive;
                    modified.HasFullControl = folderPermission.HasFullControl;
                    modified.HasRead = folderPermission.HasRead;
                    modified.HasWrite = folderPermission.HasWrite;
                    modified.HasDelete = folderPermission.HasDelete;
                    await _service.UpdateFolderPermission(folderPermission);
                    activityid = await _logger.LogActivity(folderPermission.CompanyId,"Folder Permission Modification", "Folder - " + modified.FolderName + " permissions modified for user " + modified.UserName, dataroomid: modified.DataRoomId, dataroomname: modified.DataRoomName);
                    await _logger.LogDataDifference(activityid, original, modified, dataRoomId: original.DataRoomId);
                    await ManageFilePermissions(modified, "Save");
                    DataCache.RefreshSingleFolderPermission(modified);
                    string loggedInUser = Convert.ToString(Session["UserName"]);
                    _emailSender.SendEmailtoUser("UserPermission", userid: modified.UserId, objectType: "Folder", objectId: modified.FolderId, loggedInUser: loggedInUser);
                    IEnumerable<FolderPermission> folderPermissions = GetIndividualFolderPermissions(modified.FolderId);
                    JsonResult jr = Json(new
                    {
                        HTML = this.RenderPartialView(@"~\Areas\Folders\Views\Shared\_singlefolderpermissionlist.cshtml", folderPermissions)
                    });
                    jr.MaxJsonLength = int.MaxValue;
                    jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return jr;
                }
                else
                {
                    folderPermission.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    folderPermission.CreatorName = Convert.ToString(Session["UserName"]);
                    folderPermission.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    folderPermission.CompanyName = Convert.ToString(Session["CompanyName"]);
                    folderPermission.CreatedOn = DateTime.Now;
                    int id = await _service.SaveFolderPermission(folderPermission);
                    folderPermission.Id = id;
                    await _logger.LogActivity(folderPermission.CompanyId,"Folder Permission Creation", "Folder - " + folderPermission.DataRoomName + " permissions created for user " + folderPermission.UserName, dataroomid: folderPermission.DataRoomId, dataroomname: folderPermission.DataRoomName);
                    await ManageFilePermissions(folderPermission, "Save");
                    DataCache.RefreshSingleFolderPermission(folderPermission);
                    await _permissionManager.ApplyPermissionstoFolderHierarchy(folderPermission.FolderId, folderPermission.UserId, folderPermission.UserName);
                    string loggedInUser = Convert.ToString(Session["UserName"]);
                    _emailSender.SendEmailtoUser("UserPermission", userid: folderPermission.UserId, objectType: "Folder", objectId: folderPermission.FolderId, loggedInUser: loggedInUser);
                    return Json("Success");
                }
                
                //TempData["Notification"] = "Folder Permission Saved Successfully";
                //return RedirectToAction("List", "ManageFolderPermission", new { area = "Folders",folderid=folderPermission.FolderId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var folderpermission = DataCache.FolderPermissions.Single(x => x.Id == id);
                await _service.DeleteFolderPermission(folderpermission);
                DataCache.RemovFolderPermissionfromCache(folderpermission);
                await ManageFilePermissions(folderpermission, "Delete");
                await _logger.LogActivity(folderpermission.CompanyId,"Folder Permission Deletion", "Folder - " + folderpermission.FolderName + " permissions deleted for user " + folderpermission.UserName, dataroomid: folderpermission.DataRoomId, dataroomname: folderpermission.DataRoomName);
                string loggedInUser = Convert.ToString(Session["UserName"]);
                _emailSender.SendEmailtoUser("UserPermission", userid: folderpermission.UserId, objectType: "Folder", objectId: folderpermission.FolderId, loggedInUser: loggedInUser);
                TempData["Notification"] = "Folder Permission Deleted Successfully";
                //return RedirectToAction("List", "ManageFolderPermission", new { area = "Folders", folderid = folderpermission.FolderId });
                IEnumerable<FolderPermission> folderPermissions = GetIndividualFolderPermissions(folderpermission.FolderId);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Folders\Views\Shared\_singlefolderpermissionlist.cshtml", folderPermissions)
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

        public async Task ManageFilePermissions(DataRooms.UI.Models.FolderPermission folderPermission, string flag)
        {
            try
            {
                // Remove All Permissions for the user                
                var filepermissions = DataCache.FilePermissions.Where(x => x.FolderId == folderPermission.FolderId && x.UserId == folderPermission.UserId);
                if (filepermissions != null && filepermissions.Count() > 0)
                {
                    await _fileService.DeleteRangeFilePermission(filepermissions);
                    new Thread(() => DataCache.RefreshFilePermissions()).Start();
                }

                if (flag == "Save")
                {
                    // Save new permissions
                    var files = DataCache.Files.Where(x => x.IsActive == true && x.FolderId == folderPermission.DataRoomId);
                    if (files != null && files.Count() > 0)
                    {
                        foreach (var file in files)
                        {
                            FilePermission permission = new FilePermission();
                            permission.DataRoomId = folderPermission.DataRoomId;
                            permission.DataRoomName = folderPermission.DataRoomName;
                            permission.UserId = folderPermission.UserId;
                            permission.UserName = folderPermission.UserName;
                            permission.FileId = file.Id;
                            permission.FileName = file.FileName;
                            permission.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                            permission.CompanyName = Convert.ToString(Session["CompanyName"]);
                            permission.FolderId = file.FolderId;
                            permission.FolderName = file.FolderName;
                            permission.IsActive = true;
                            permission.HasFullControl = folderPermission.HasFullControl;
                            permission.HasRead = folderPermission.HasRead;
                            permission.HasWrite = folderPermission.HasWrite;
                            permission.HasDelete = folderPermission.HasDelete;
                            permission.CreatedBy = folderPermission.CreatedBy;
                            permission.CreatorName = folderPermission.CreatorName;
                            permission.CreatedOn = folderPermission.CreatedOn;
                            permission.Id = await _fileService.SaveFilePermission(permission);
                            new Thread(() => DataCache.RefreshSingleFilePermission(permission)).Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetFolderPermissions(int folderid)
        {
            try
            {
                IEnumerable<FolderPermission> folderPermissions = GetIndividualFolderPermissions(folderid);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Folders\Views\Shared\_folderpermissionslist.cshtml", folderPermissions)
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



        public IEnumerable<FolderPermission> GetIndividualFolderPermissions(int folderid)
        {
            try
            {
                int companyId = Convert.ToInt32(Session["CompanyId"]);
                var folderPermissions = DataCache.FolderPermissions.Where(x => x.FolderId == folderid && x.CompanyId == companyId);
                if(folderPermissions!= null && folderPermissions.Count() > 0)
                {
                    return folderPermissions.ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return new List<FolderPermission>();
        }
    }
}