using Castle.Core.Smtp;
using DataRooms.UI.Code.Email;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.DataRoom.Controllers
{
    public class ManageItemTrackerPermissionController : Controller
    {
        ItemTrackerService _itemTrackerService { get; set; }
        private LogService _logger { get; set; }
        SendEmail _emailSender;
        public ManageItemTrackerPermissionController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _logger = new LogService(token);
            _itemTrackerService = new ItemTrackerService(token);
            _emailSender = new SendEmail();
        }
        public JsonResult EditItemTrackerPermission(int itemtrackerid)
        {
            try
            {
                ItemTrackerMetaData metaData = new ItemTrackerMetaData();
                ItemTrackerPermission permission = new ItemTrackerPermission();
                var itemTrackerMetaData = DataCache.ItemTrackerMetaData.Where(x => x.Id == itemtrackerid); 
                if(itemTrackerMetaData!=null && itemTrackerMetaData.Count() > 0)
                {
                    metaData = itemTrackerMetaData.First();
                    permission.DataRoomId = metaData.DataRoomId;
                    permission.DataRoomName = metaData.DataRoomName;
                    permission.FolderId = metaData.FolderId;
                    permission.FolderName = metaData.FolderName;
                    permission.ItemTrackerId = metaData.Id;
                    permission.ItemTrackerName = metaData.ItemTrackerName;
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_edititemtrackerpermission.cshtml", permission)
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

        public JsonResult GetItemtrackerPermissions(int itemtrackerid)
        {
            try
            {
                List<ItemTrackerPermission> itemTrackerPermissions = new List<ItemTrackerPermission>();
                var itemTrackerPermissionDetails = DataCache.ItemTrackerPermissions.Where(x => x.ItemTrackerId == itemtrackerid);    
                if(itemTrackerPermissionDetails != null && itemTrackerPermissionDetails.Count() > 0)
                {
                    itemTrackerPermissions = itemTrackerPermissionDetails.ToList();
                }
                else
                {
                    var itemTrackerMetaData = DataCache.ItemTrackerMetaData.First(x => x.Id == itemtrackerid);
                    itemTrackerPermissions.Add(new ItemTrackerPermission {ItemTrackerId = itemTrackerMetaData.Id,
                        ItemTrackerName = itemTrackerMetaData.ItemTrackerName,
                        DataRoomId = itemTrackerMetaData.DataRoomId,
                    DataRoomName = itemTrackerMetaData.DataRoomName,
                    FolderId = itemTrackerMetaData.FolderId,
                    FolderName = itemTrackerMetaData.FolderName,
                    });
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_itemtrackerpermissionslist.cshtml", itemTrackerPermissions)
                });
                jr.MaxJsonLength = int.MaxValue;
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jr;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("");
        }

        public async Task<JsonResult> SaveItemTrackerPermission(ItemTrackerPermission model)
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            try
            {
                int itemtrackerid = 0;
                if(model.Id > 0)
                {                    
                    var itemtrackerpermission = DataCache.ItemTrackerPermissions.First(x => x.Id == model.Id);
                    itemtrackerid = itemtrackerpermission.ItemTrackerId;
                    itemtrackerpermission.HasFullControl = model.HasFullControl;
                    itemtrackerpermission.HasWrite = model.HasWrite;
                    itemtrackerpermission.HasRead = model.HasRead;
                    itemtrackerpermission.HasDelete = model.HasDelete;
                    itemtrackerpermission.IsActive = model.IsActive;
                    itemtrackerpermission.ModifiedBy = loggedInUser;
                    itemtrackerpermission.ModifierName = loggedInUserName;
                    itemtrackerpermission.ModifiedOn = DateTime.Now;
                    await _itemTrackerService.UpdateItemTrackerPermission(itemtrackerpermission);
                    DataCache.RefreshSingleItemTrackerPermission(itemtrackerpermission);
                }
                else
                {
                    model.CreatedBy = loggedInUser;
                    model.CreatorName = loggedInUserName;
                    model.CreatedOn = DateTime.Now;
                    model.Id = await _itemTrackerService.SaveItemTrackerPermission(model);
                    itemtrackerid = model.ItemTrackerId;
                    DataCache.RefreshSingleItemTrackerPermission(model);
                }
                _emailSender.SendEmailtoUser("UserPermission", userid: model.UserId, objectType: "ItemTracker", objectId: model.ItemTrackerId, loggedInUser: loggedInUserName);
                List<ItemTrackerPermission> permissions = new List<ItemTrackerPermission>();
                var itemTrackerPermissionDetails = DataCache.ItemTrackerPermissions.Where(x => x.ItemTrackerId == itemtrackerid);
                if (itemTrackerPermissionDetails != null && itemTrackerPermissionDetails.Count() > 0)
                {
                    permissions = itemTrackerPermissionDetails.ToList();
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_itemtrackerpermissiongrid.cshtml", permissions)
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

        public async Task<JsonResult> DeleteItemTrackerPermission(int id)
        {
            try
            {
                int itemtrackerid = 0;
                var itemTrackerPermission = DataCache.ItemTrackerPermissions.Where(x => x.Id == id);
                if(itemTrackerPermission!=null && itemTrackerPermission.Count() > 0)
                {
                    var permission = itemTrackerPermission.First();
                    itemtrackerid = permission.ItemTrackerId;
                   await _itemTrackerService.DeleteItemTrackerPermission(permission);
                   DataCache.RemovItemTrackerPermissionfromCache(permission);
                    var dataroom = DataCache.DataRooms.First(x => x.Id == permission.DataRoomId);
                    _emailSender.SendEmailtoUser("UserPermission", userid: permission.UserId, objectType: "ItemTracker", objectId: permission.ItemTrackerId, loggedInUser: Convert.ToString(Session["UserName"]));
                    await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Permission Deletion", "ItemTracker - " + permission.ItemTrackerName + " permissions deleted for user " + permission.UserName, dataroomid: permission.DataRoomId, dataroomname: permission.DataRoomName);
                }
                List<ItemTrackerPermission> permissions = new List<ItemTrackerPermission>();
                var itemTrackerPermissionDetails = DataCache.ItemTrackerPermissions.Where(x => x.ItemTrackerId == itemtrackerid);
                if (itemTrackerPermissionDetails != null && itemTrackerPermissionDetails.Count() > 0)
                {
                    permissions = itemTrackerPermissionDetails.ToList();
                }
                
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_itemtrackerpermissiongrid.cshtml", permissions)
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
    }
}