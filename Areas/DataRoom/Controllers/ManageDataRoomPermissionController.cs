using DataRooms.UI.Areas.DataRoom.Models;
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

namespace DataRooms.UI.Areas.DataRoom.Controllers
{
    [SessionExpire]
    public class ManageDataRoomPermissionController : Controller
    {
        private DataRoomPermissionService _service { get; set; }
        private FolderPermissionService _folderService { get; set; }
        private FilePermissionService _fileService { get; set; }
        private LogService _logger { get; set; }
        SendEmail _emailSender;
        public ManageDataRoomPermissionController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new DataRoomPermissionService(token);
            _folderService = new FolderPermissionService(token);
            _fileService = new FilePermissionService(token);
            _logger = new LogService(token);
            _emailSender = new SendEmail();
        }


        [HttpGet]
        public ActionResult List(int dataroomid)
        {
            try
            {
                var model = new DataRoomCustomModel();
                model.DataRoomId = dataroomid;
                GetAllDataRoomPermissionsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult List(DataRoomCustomModel model)
        {
            try
            {
                GetAllDataRoomPermissionsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetAllDataRoomPermissionsAsync(DataRoomCustomModel model)
        {
            try
            {
                model.SortColumn = string.IsNullOrEmpty(model.SortColumn) ? "CreatedOn" : model.SortColumn;
                IPagedList<DataRooms.UI.Models.DataRoomPermission> datarooms = null;
                model.DataRoom = DataCache.DataRooms.First(x=>x.Id == model.DataRoomId);
                IEnumerable<DataRoomPermission> permissions = DataCache.DataRoomPermissions;
                datarooms = permissions.Where(x=>x.DataRoomId == model.DataRoomId).OrderByDescending
                            (m => m.UserName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                model.PagedDataRoomPermissions = datarooms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id,int dataroomid)
        {
            try
            {
                
                var dataroompermission = new DataRooms.UI.Models.DataRoomPermission();
                if (id > 0)
                    dataroompermission = DataCache.DataRoomPermissions.Single(x=>x.Id == id);
                else
                {
                    var dataroom = DataCache.DataRooms.First(x => x.Id == dataroomid);
                    dataroompermission.DataRoomId = dataroom.Id;
                    dataroompermission.DataRoomName = dataroom.DataRoomName;
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_editdataroompermission.cshtml", dataroompermission)
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
        public async Task<ActionResult> Edit(DataRooms.UI.Models.DataRoomPermission dataroompermission)
        {
            int activityid = 0;
            try
            {
                var permissiondetails = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == dataroompermission.DataRoomId && x.UserId == dataroompermission.UserId);
                if(permissiondetails.Count() > 0)
                {
                    dataroompermission.Id = permissiondetails.First().Id;
                }
                if (dataroompermission.Id > 0)
                {
                    var original = DataCache.DataRoomPermissions.Single(x=>x.Id == dataroompermission.Id);
                    var modified = original.Clone();
                    modified.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    modified.CompanyName = Convert.ToString(Session["CompanyName"]);
                    modified.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    modified.ModifierName = Convert.ToString(Session["UserName"]);
                    modified.ModifiedOn = DateTime.Now;
                    modified.HasFullControl = dataroompermission.HasFullControl;
                    modified.HasRead = dataroompermission.HasRead;
                    modified.HasWrite = dataroompermission.HasWrite;
                    modified.HasDelete = dataroompermission.HasDelete;
                    modified.IsActive = dataroompermission.IsActive;
                    await _service.UpdateDataRoomPermission(dataroompermission);
                    activityid = await _logger.LogActivity(modified.CompanyId,"SharBox Permission Modification", "SharBox - " + modified.DataRoomName + " permissions modified for user " + modified.UserName,dataroomid: modified.DataRoomId, dataroomname: modified.DataRoomName);
                    await _logger.LogDataDifference(activityid, original, dataroompermission, dataRoomId: original.Id);
                    await ManageFileandFolerPermissions(modified, "Save");
                    DataCache.RefreshSingleDataRoomPermission(modified);

                    string loggedInUser = Convert.ToString(Session["UserName"]);
                    _emailSender.SendEmailtoUser("UserPermission", userid: modified.UserId,objectType:"SharBox",objectId: modified.DataRoomId,loggedInUser: loggedInUser);

                    var dataroompermissions = GetIndividualDataRoomPermission(modified.DataRoomId);
                    JsonResult jr = Json(new
                    {
                        HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_singledataroompermissionlist.cshtml", dataroompermissions)
                    });
                    jr.MaxJsonLength = int.MaxValue;
                    jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return jr;
                }
                else
                {
                    dataroompermission.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    dataroompermission.CompanyName = Convert.ToString(Session["CompanyName"]);
                    dataroompermission.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    dataroompermission.CreatorName = Convert.ToString(Session["UserName"]);
                    dataroompermission.CreatedOn = DateTime.Now;
                    int id = await _service.SaveDataRoomPermission(dataroompermission);
                    dataroompermission.Id = id;
                    await _logger.LogActivity(dataroompermission.CompanyId,"SharBox Permission Creation", "SharBox - " + dataroompermission.DataRoomName + " permissions created for user " + dataroompermission.UserName, dataroomid: dataroompermission.DataRoomId, dataroomname: dataroompermission.DataRoomName);
                    await ManageFileandFolerPermissions(dataroompermission, "Save");
                    DataCache.RefreshSingleDataRoomPermission(dataroompermission);
                    string loggedInUser = Convert.ToString(Session["UserName"]);
                    _emailSender.SendEmailtoUser("UserPermission", userid: dataroompermission.UserId, objectType: "SharBox", objectId: dataroompermission.DataRoomId, loggedInUser: loggedInUser);
                    return Json("");
                }
                
                //new Thread(() => DataCache.RefreshSingleDataRoomPermission(dataroompermission)).Start();
                //TempData["Notification"] = "Data Room Permission Saved Successfully";
                //return RedirectToAction("List", "ManageDataRoomPermission", new { area = "DataRoom", dataroomid=dataroompermission.DataRoomId });
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
                var dataroompermission = DataCache.DataRoomPermissions.Single(x=>x.Id == id);
                await _service.DeleteDataRoomPermission(dataroompermission);
                await ManageFileandFolerPermissions(dataroompermission, "Delete");
                //new Thread(() => DataCache.RemoveDataRoomPermissionfromCache(dataroompermission)).Start();
                DataCache.RemoveDataRoomPermissionfromCache(dataroompermission);
                await _logger.LogActivity(dataroompermission.CompanyId,"SharBox Permission Deletion", "SharBox - " + dataroompermission.DataRoomName + " permissions deleted for user " + dataroompermission.UserName, dataroomid: dataroompermission.DataRoomId, dataroomname: dataroompermission.DataRoomName);
                string loggedInUser = Convert.ToString(Session["UserName"]);
                _emailSender.SendEmailtoUser("UserPermission", userid: dataroompermission.UserId, objectType: "SharBox", objectId: dataroompermission.DataRoomId, loggedInUser: loggedInUser);
                var dataroompermissions = GetIndividualDataRoomPermission(dataroompermission.DataRoomId);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_singledataroompermissionlist.cshtml", dataroompermissions)
                });
                jr.MaxJsonLength = int.MaxValue;
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jr;
                //TempData["Notification"] = "Data Room Permission Deleted Successfully";
                //return RedirectToAction("List", "ManageDataRoomPermission", new { area = "DataRoom",dataroomid = dataroompermission.DataRoomId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDataRoomPermissions(int dataroomid)
        {
            var dataroompermissions = GetIndividualDataRoomPermission(dataroomid);
            JsonResult jr = Json(new
            {
                HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_dataroompermissionlist.cshtml", dataroompermissions)
            });
            jr.MaxJsonLength = int.MaxValue;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }

        public IEnumerable<DataRoomPermission> GetIndividualDataRoomPermission(int dataroomid)
        {
            try
            {
                var dataroompermissions = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == dataroomid);
                if(dataroompermissions!=null && dataroompermissions.Count() > 0)
                {
                    return dataroompermissions.ToList();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return new List<DataRoomPermission>();
        }

        public async Task ManageFileandFolerPermissions(DataRooms.UI.Models.DataRoomPermission dataroompermission,string flag)
        {
            try
            {
                // Remove All Permissions for the user
                var folderpermissions = DataCache.FolderPermissions.Where(x => x.DataRoomId == dataroompermission.DataRoomId && x.UserId == dataroompermission.UserId);
                if(folderpermissions!=null && folderpermissions.Count() > 0)
                {
                    await _folderService.DeleteRangeFolderPermissions(folderpermissions);
                    new Thread(() => DataCache.RefreshFolderPermissions()).Start();
                }
                var filepermissions = DataCache.FilePermissions.Where(x => x.DataRoomId == dataroompermission.DataRoomId && x.UserId == dataroompermission.UserId);
                if (filepermissions != null && filepermissions.Count() > 0)
                {
                    await _fileService.DeleteRangeFilePermission(filepermissions);
                    new Thread(() => DataCache.RefreshFilePermissions()).Start();
                }

                if(flag == "Save")
                {
                    // Save new permissions
                    var folders = DataCache.Folders.Where(x => x.IsActive == true && x.DataRoomId == dataroompermission.DataRoomId);
                    if (folders != null && folders.Count() > 0)
                    {
                        foreach (var folder in folders)
                        {
                            FolderPermission permission = new FolderPermission();
                            permission.DataRoomId = dataroompermission.DataRoomId;
                            permission.DataRoomName = dataroompermission.DataRoomName;
                            permission.FolderId = folder.Id;
                            permission.FolderName = folder.FolderName;
                            permission.UserId = dataroompermission.UserId;
                            permission.UserName = dataroompermission.UserName;
                            permission.IsActive = true;
                            permission.HasFullControl = dataroompermission.HasFullControl;
                            permission.HasRead = dataroompermission.HasRead;
                            permission.HasWrite = dataroompermission.HasWrite;
                            permission.HasDelete = dataroompermission.HasDelete;
                            permission.CompanyId = dataroompermission.CompanyId;
                            permission.CompanyName = dataroompermission.CompanyName;
                            permission.CreatedBy = dataroompermission.CreatedBy;
                            permission.CreatorName = dataroompermission.CreatorName;
                            permission.CreatedOn = dataroompermission.CreatedOn;
                            permission.Id = await _folderService.SaveFolderPermission(permission);
                            new Thread(() => DataCache.RefreshSingleFolderPermission(permission)).Start();
                        }
                    }

                    var files = DataCache.Files.Where(x => x.IsActive == true && x.DataRoomId == dataroompermission.DataRoomId);
                    if (files != null && files.Count() > 0)
                    {
                        foreach (var file in files)
                        {
                            FilePermission permission = new FilePermission();
                            permission.DataRoomId = dataroompermission.DataRoomId;
                            permission.DataRoomName = dataroompermission.DataRoomName;
                            permission.UserId = dataroompermission.UserId;
                            permission.UserName = dataroompermission.UserName;
                            permission.FileId = file.Id;
                            permission.FileName = file.FileName;
                            permission.FolderId = file.FolderId;
                            permission.FolderName = file.FolderName;
                            permission.IsActive = true;
                            permission.HasFullControl = dataroompermission.HasFullControl;
                            permission.HasRead = dataroompermission.HasRead;
                            permission.HasWrite = dataroompermission.HasWrite;
                            permission.HasDelete = dataroompermission.HasDelete;
                            permission.CompanyId = dataroompermission.CompanyId;
                            permission.CompanyName = dataroompermission.CompanyName;
                            permission.CreatedBy = dataroompermission.CreatedBy;
                            permission.CreatorName = dataroompermission.CreatorName;
                            permission.CreatedOn = dataroompermission.CreatedOn;
                            permission.Id = await _fileService.SaveFilePermission(permission);
                            new Thread(() => DataCache.RefreshSingleFilePermission(permission)).Start();
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}