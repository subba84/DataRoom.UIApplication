
using Amazon.S3.Model;
using Castle.Core.Smtp;
using DataRooms.UI.Areas.DataRoom.Models;
using DataRooms.UI.Areas.Explorer.Model;
using DataRooms.UI.Code;
using DataRooms.UI.Code.Email;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using iTextSharp.text;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using static iTextSharp.text.pdf.AcroFields;

namespace DataRooms.UI.Areas.DataRoom.Controllers
{
    public class ManageItemTrackerController : Controller
    {
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        private ItemTrackerService _itemTrackerService;
        FileEncryption encryption;
        LogService _logger;
        PermissionManager _permissionManager;
        string authToken;
        SendEmail _emailSender;
        public ManageItemTrackerController()
        {
            authToken = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _logger = new LogService(authToken);
            encryption = new FileEncryption();
            _emailSender = new SendEmail();
            _permissionManager = new PermissionManager(authToken);
            _itemTrackerService = new ItemTrackerService(authToken);
        }

        public JsonResult ItemTrackerCreation(int dataroomid,int folderid)
        {
            try
            {
                Guid itemTrackerGuid = Guid.NewGuid();
                ItemTrackerModel model = new ItemTrackerModel();
                model.ItemTrackerGuid = itemTrackerGuid.ToString();
                var dataroom = DataCache.DataRooms.First(x => x.Id == dataroomid);
                model.DataRoomId = dataroom.Id;
                model.DataRoomName = dataroom.DataRoomName;
                var folder = DataCache.Folders.Where(x => x.Id == folderid);
                if(folder!=null && folder.Count() > 0)
                {
                    model.FolderId = folder.First().Id;
                    model.FolderName = folder.First().FolderName;
                }                
                model.DataRoomItemTrackerControls = new List<ItemTrackerControl>();
                model.ItemTrackerPermissions = new List<ItemTrackerPermission>();
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Explorer\Views\Shared\_itemtrackercreation.cshtml", model)
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
        public async Task<JsonResult> SaveItemTrackerControl(ItemTrackerModel model)
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            var dataroom = DataCache.DataRooms.First(x => x.Id == model.DataRoomId);
            var company = DataCache.Companies.First(x => x.Id == dataroom.CompanyId);
            // Parent Control Data
            var parentData = model.ControlData.Where(x => string.IsNullOrEmpty(x.ControlReferenceData));
            if(parentData!=null && parentData.Count() > 0)
            {
                Guid parentGuid = Guid.NewGuid();
                Guid level1guid = Guid.NewGuid();
                Guid level2guid = Guid.NewGuid();
                Guid level3guid = Guid.NewGuid();
                foreach (var parentItem in parentData.ToList())
                {
                    ItemTrackerControl level1ItemTrackerControl = new ItemTrackerControl();
                    level1ItemTrackerControl.ControlType = model.ControlType;
                    level1ItemTrackerControl.ControlName = parentItem.ControlName;
                    level1ItemTrackerControl.DataRoomId = model.DataRoomId;
                    level1ItemTrackerControl.DataRoomName = model.DataRoomName;
                    level1ItemTrackerControl.FolderId = model.FolderId;
                    level1ItemTrackerControl.FolderName = model.FolderName;
                    level1ItemTrackerControl.ItemTrackerId = model.ItemTrackerId;
                    //level1ItemTrackerControl.ItemTrackerName = model.DataRoomName;
                    level1ItemTrackerControl.IsActive = true;
                    level1ItemTrackerControl.CreatedBy = loggedInUser;
                    level1ItemTrackerControl.CreatedByName = loggedInUserName;
                    level1ItemTrackerControl.CreatedOn = DateTime.Now;
                    level1ItemTrackerControl.IsMandatory = model.IsMandatory;
                    level1ItemTrackerControl.ControlMasterData = parentItem.ControlData;
                    level1ItemTrackerControl.ControlReferenceName = parentItem.ControlReferenceData;
                    level1ItemTrackerControl.ControlGuid = level1guid.ToString();
                    level1ItemTrackerControl.ParentGuid = parentGuid.ToString();
                    level1ItemTrackerControl.ItemTrackerGuid = model.ItemTrackerGuid;
                    level1ItemTrackerControl.Id = await _itemTrackerService.SaveItemTrackerControl(level1ItemTrackerControl);
                    //await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Control Creation", "Control " + parentItem.ControlName + " Created in Item Tracker " + model.ItemTrackerName, dataroom.Id, 0, 0, dataroom.DataRoomName, null, null);
                    DataCache.RefreshSingleItemTrackerControl(level1ItemTrackerControl);
                    if(!string.IsNullOrEmpty(parentItem.ControlData))
                    {
                        
                        var level2Data = model.ControlData.Where(x => !string.IsNullOrEmpty(x.ControlData) && x.ControlReferenceData == parentItem.ControlData);
                        if (level2Data != null && level2Data.Count() > 0)
                        {
                            foreach (var level2Item in level2Data.ToList())
                            {
                                ItemTrackerControl level2ItemTrackerControl = new ItemTrackerControl();                                
                                level2ItemTrackerControl.ControlType = model.ControlType;
                                level2ItemTrackerControl.ControlName = level2Item.ControlName;
                                level2ItemTrackerControl.DataRoomId = model.DataRoomId;
                                level2ItemTrackerControl.DataRoomName = model.DataRoomName;
                                level2ItemTrackerControl.FolderId = model.FolderId;
                                level2ItemTrackerControl.FolderName = model.FolderName;
                                level2ItemTrackerControl.ItemTrackerId = model.ItemTrackerId;
                                level2ItemTrackerControl.IsActive = true;
                                level2ItemTrackerControl.CreatedBy = loggedInUser;
                                level2ItemTrackerControl.CreatedByName = loggedInUserName;
                                level2ItemTrackerControl.CreatedOn = DateTime.Now;
                                level2ItemTrackerControl.IsMandatory = model.IsMandatory;
                                level2ItemTrackerControl.ControlMasterData = level2Item.ControlData;
                                level2ItemTrackerControl.ControlReferenceId = level1ItemTrackerControl.Id;
                                level2ItemTrackerControl.ControlReferenceName = level2Item.ControlReferenceData;
                                level2ItemTrackerControl.ControlGuid = level2guid.ToString();
                                level2ItemTrackerControl.ParentGuid = parentGuid.ToString();
                                level2ItemTrackerControl.ItemTrackerGuid = model.ItemTrackerGuid;
                                level2ItemTrackerControl.Id = await _itemTrackerService.SaveItemTrackerControl(level2ItemTrackerControl);
                                //await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Control Creation", "Control " + level2Item.ControlName + " Created in Item Tracker " + model.ItemTrackerName, dataroom.Id, 0, 0, dataroom.DataRoomName, null, null);
                                DataCache.RefreshSingleItemTrackerControl(level2ItemTrackerControl);
                                
                                var level3Data = model.ControlData.Where(x => !string.IsNullOrEmpty(x.ControlData) &&
                                x.ControlReferenceData == level2Item.ControlData);
                                if (level3Data != null && level3Data.Count() > 0)
                                {
                                    foreach (var level3Item in level3Data.ToList())
                                    {
                                        ItemTrackerControl level3ItemTrackerControl = new ItemTrackerControl();
                                        level3ItemTrackerControl.ControlType = model.ControlType;
                                        level3ItemTrackerControl.ControlName = level3Item.ControlName;
                                        level3ItemTrackerControl.DataRoomId = model.DataRoomId;
                                        level3ItemTrackerControl.DataRoomName = model.DataRoomName;
                                        level3ItemTrackerControl.FolderId = model.FolderId;
                                        level3ItemTrackerControl.FolderName = model.FolderName;
                                        level3ItemTrackerControl.ItemTrackerId = model.ItemTrackerId;
                                        level3ItemTrackerControl.IsActive = true;
                                        level3ItemTrackerControl.CreatedBy = loggedInUser;
                                        level3ItemTrackerControl.CreatedByName = loggedInUserName;
                                        level3ItemTrackerControl.CreatedOn = DateTime.Now;
                                        level3ItemTrackerControl.IsMandatory = model.IsMandatory;
                                        level3ItemTrackerControl.ControlMasterData = level3Item.ControlData;
                                        level3ItemTrackerControl.ControlReferenceId = level2ItemTrackerControl.Id;
                                        level3ItemTrackerControl.ControlReferenceName = level3Item.ControlReferenceData;
                                        level3ItemTrackerControl.ControlGuid = level3guid.ToString();
                                        level3ItemTrackerControl.ParentGuid = parentGuid.ToString();
                                        level3ItemTrackerControl.ItemTrackerGuid = model.ItemTrackerGuid;
                                        level3ItemTrackerControl.Id = await _itemTrackerService.SaveItemTrackerControl(level3ItemTrackerControl);
                                        //await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Control Creation", "Control " + level3Item.ControlName + " Created in Item Tracker " + model.ItemTrackerName, dataroom.Id, 0, 0, dataroom.DataRoomName, null, null);
                                        DataCache.RefreshSingleItemTrackerControl(level3ItemTrackerControl);
                                    }
                                }
                            }
                        }
                    }
                    
                }
            }
            return Json("");
        }

        public JsonResult GetItemTrackerControlsbasedonDataRoomFolderTracker(int dataroomid, int folderid, int itemtrackerid,string guid="")
        {
            try
            {
                List<ItemTrackerControl> model = new List<ItemTrackerControl>();
                var dataroomitemtrackercontrols = DataCache.ItemTrackerControls.Where(x => x.DataRoomId == dataroomid && x.IsActive == true && x.FolderId == folderid && x.ItemTrackerGuid == guid);
                if(dataroomitemtrackercontrols!=null && dataroomitemtrackercontrols.Count() > 0)
                {
                    model = dataroomitemtrackercontrols.ToList();
                }
                else
                {
                    dataroomitemtrackercontrols = DataCache.ItemTrackerControls.Where(x => x.DataRoomId == dataroomid && x.IsActive == true && x.FolderId == folderid && x.ItemTrackerId == itemtrackerid);
                    if (dataroomitemtrackercontrols != null && dataroomitemtrackercontrols.Count() > 0)
                    {
                        model = dataroomitemtrackercontrols.ToList();
                    }
                }
                JsonResult jr = Json(new
                {
                    //HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_itemtrackerformpart.cshtml", model)
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_itemtrackercontrollist.cshtml", model)
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

        public JsonResult GetItemTrackerEditFormPartbasedonDataRoom(int dataroomid,int folderid,int itemtrackerid,string rowguid=null)
        {
            try
            {
                ItemTrackerModel model = new ItemTrackerModel();
                model.DataRoomId = dataroomid;
                model.FolderId = folderid;
                model.ItemTrackerId = itemtrackerid;
                model.DataRoomItemTrackerData = new List<ItemTrackerData>();
                model.DataRoomItemTrackerControls = new List<ItemTrackerControl>();                
                var dataroomitemtrackercontrols = DataCache.ItemTrackerControls.Where(x => x.DataRoomId == dataroomid && x.FolderId == folderid && x.ItemTrackerId == itemtrackerid && x.IsActive == true);
                if (dataroomitemtrackercontrols != null && dataroomitemtrackercontrols.Count() > 0)
                {
                    model.DataRoomItemTrackerControls = dataroomitemtrackercontrols.ToList();
                }
                model.DataRoomItemTrackerData = new List<ItemTrackerData>();
                if (!string.IsNullOrEmpty(rowguid) && rowguid!="undefined")
                {
                    var dataroomitemtrackerdata = DataCache.ItemTrackerData.Where(x => x.DataRoomId == dataroomid && x.IsActive == true && x.FolderId == folderid && x.ItemTrackerId == itemtrackerid);
                    if (dataroomitemtrackerdata != null && dataroomitemtrackerdata.Count() > 0)
                    {
                        model.DataRoomItemTrackerData = dataroomitemtrackerdata.ToList();
                        if (model.DataRoomItemTrackerData != null && model.DataRoomItemTrackerData.Count() > 0)
                        {
                            var rowDetails = model.DataRoomItemTrackerData.Where(x => x.RowGuid == rowguid);
                            if (rowDetails != null && rowDetails.Count() > 0)
                            {
                                model.DataRoomItemTrackerData = rowDetails.ToList();
                            }
                        }
                    }
                }
                
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_itemtrackerformpart.cshtml", model)                    
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

        public async Task<JsonResult> DeleteItemTrackerControl(string guid)
        {
            try
            {
                var itemTrackerControls = DataCache.ItemTrackerControls.Where(x => x.ParentGuid == guid);
                if(itemTrackerControls!=null && itemTrackerControls.Count() > 0)
                {
                    int dataroomid = itemTrackerControls.First().DataRoomId;
                    int itemtrackerid = itemTrackerControls.First().ItemTrackerId;
                    var dataroom = DataCache.DataRooms.First(x => x.Id == dataroomid);
                    var itemtracker = DataCache.ItemTrackerMetaData.First(x => x.Id == itemtrackerid);
                    foreach(var item in itemTrackerControls.ToList())
                    {
                        await _itemTrackerService.DeleteItemTrackerControl(item);
                        await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Control Deletion", "Control " + item.ControlName + " got deleted in Item Tracker " + itemtracker.ItemTrackerName, dataroom.Id, 0, itemtracker.Id, dataroom.DataRoomName, null, itemtracker.ItemTrackerName);
                        DataCache.RemoveItemTrackerControlfromCache(item);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("");
        }

        public async Task<JsonResult> SaveItemTrackerData(IEnumerable<ItemTrackerData> model, List<HttpPostedFileBase> files)
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            List<ItemTrackerData> oldData=new List<ItemTrackerData>();
            try
            {
                if (!string.IsNullOrEmpty(model.First().RowGuid))
                {
                    oldData = DataCache.ItemTrackerData.Where(x => x.RowGuid == model.First().RowGuid).ToList();
                    var serializedData = JsonConvert.SerializeObject(oldData);
                    oldData = JsonConvert.DeserializeObject<List<ItemTrackerData>>(serializedData);
                }
                string itemTrackerPath = string.Empty;
                List<string> filepaths = new List<string>();
                var dataroom = DataCache.DataRooms.First(x => x.Id == model.First().DataRoomId);
                //var folder = DataCache.Folders.First(x => x.Id == model.First().FolderId);
                //var itemtracker = DataCache.ItemTrackerMetaData.First(x => x.Id == model.First().ItemTrackerId);
                itemTrackerPath = dataroom.RelativePath + "/ItemTracker";
                if (!System.IO.Directory.Exists(itemTrackerPath))
                {
                    System.IO.Directory.CreateDirectory(itemTrackerPath);
                }
                if (files != null && files.Count() > 0)
                {
                    foreach (var item in files)
                    {
                        if (item != null)
                        {
                            Guid guid = Guid.NewGuid();
                            var directoryPath = Server.MapPath("~/") + "Temp/" + guid.ToString();
                            if (!System.IO.Directory.Exists(directoryPath))
                            {
                                System.IO.Directory.CreateDirectory(directoryPath);
                            }
                            var filepath = directoryPath + "/" + item.FileName;
                            item.SaveAs(filepath);
                            var encryptedPath = Server.MapPath("~/") + "Temp/EncryptionTemp/" + item.FileName;
                            encryption.EncryptFile(filepath, encryptedPath);
                            string newfilepath = itemTrackerPath + "/" + item.FileName;
                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);

                                if (System.IO.File.Exists(newfilepath))
                                {
                                    newfilepath = itemTrackerPath + "/" + System.IO.Path.GetFileNameWithoutExtension(item.FileName) + "_copy" + System.IO.Path.GetExtension(item.FileName);
                                }
                                System.IO.File.Move(encryptedPath, newfilepath);
                            }
                            filepaths.Add(newfilepath);
                        }
                    }
                }

                if (model != null && model.Count() > 0)
                {
                    var itemtrackermetadata = DataCache.ItemTrackerMetaData.First(x => x.Id == model.First().ItemTrackerId);
                    var folder = itemtrackermetadata.FolderId > 0 ? DataCache.Folders.First(x => x.Id == itemtrackermetadata.FolderId) : new Folder();
                    Guid rowGuid = Guid.NewGuid();
                    foreach (var item in model)
                    {
                        var itemTrackerData = new ItemTrackerData();
                        var rowData = DataCache.ItemTrackerData.Where(x => x.RowGuid == model.First().RowGuid && x.ControlGuid == item.ControlGuid);
                        if(rowData!=null && rowData.Count() > 0)
                        {
                            itemTrackerData = rowData.First();
                        }
                        
                        if (!string.IsNullOrEmpty(model.First().RowGuid) && itemTrackerData.Id > 0)
                        {                            
                            itemTrackerData.RowGuid = model.First().RowGuid;
                            itemTrackerData.ModifiedBy = loggedInUser;
                            itemTrackerData.ModifierName = loggedInUserName;
                            itemTrackerData.ModifiedOn = DateTime.Now;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(model.First().RowGuid))
                            {
                                itemTrackerData.RowGuid = model.First().RowGuid;
                            }
                            else
                            {
                                itemTrackerData.RowGuid = rowGuid.ToString();
                            }
                            itemTrackerData.ControlGuid = item.ControlGuid;
                            itemTrackerData.CreatedBy = loggedInUser;
                            itemTrackerData.CreatedByName = loggedInUserName;
                            itemTrackerData.CreatedOn = DateTime.Now;
                            itemTrackerData.DataRoomId = dataroom.Id;
                            itemTrackerData.DataRoomName = dataroom.DataRoomName;
                            itemTrackerData.FolderId = model.First().FolderId;
                            itemTrackerData.FolderName = model.First().FolderName;
                            itemTrackerData.ItemTrackerId = model.First().ItemTrackerId;
                            itemTrackerData.ItemTrackerName = model.First().ItemTrackerName;
                            itemTrackerData.ControlName = item.ControlName;
                            itemTrackerData.ControlTypeId = item.ControlTypeId;
                            itemTrackerData.IsActive = true;
                        }
                        switch (item.ControlTypeId)
                        {
                            case ControlType.TextBox:
                                itemTrackerData.ControlTypeName = "TextBox";
                                itemTrackerData.ControlDataId = 0;
                                itemTrackerData.ControlDataName = item.ControlDataName;
                                break;
                            case ControlType.Dropdown:
                                itemTrackerData.ControlTypeName = "Dropdown";
                                itemTrackerData.ControlDataId = item.ControlDataId;
                                if (item.ControlDataId > 0)
                                    itemTrackerData.ControlDataName = DataCache.ItemTrackerControls.Where(x => x.DataRoomId == dataroom.Id && x.Id == item.ControlDataId).First().ControlMasterData;
                                item.ControlDataName = itemTrackerData.ControlDataName;
                                break;
                            case ControlType.TwoLevelDropDown:
                                itemTrackerData.ControlTypeName = "2 Level Dropdown";
                                itemTrackerData.ControlDataId = item.ControlDataId;
                                if(item.ControlDataId > 0)
                                itemTrackerData.ControlDataName = DataCache.ItemTrackerControls.Where(x => x.DataRoomId == dataroom.Id && x.Id == item.ControlDataId).First().ControlMasterData;
                                item.ControlDataName = itemTrackerData.ControlDataName;
                                break;
                            case ControlType.ThreeLevelDropdown:
                                itemTrackerData.ControlTypeName = "3 Level Dropdown";
                                itemTrackerData.ControlDataId = item.ControlDataId;
                                if (item.ControlDataId > 0)
                                    itemTrackerData.ControlDataName = DataCache.ItemTrackerControls.Where(x => x.DataRoomId == dataroom.Id && x.Id == item.ControlDataId).First().ControlMasterData;
                                item.ControlDataName = itemTrackerData.ControlDataName;
                                break;
                            case ControlType.FileUpload:
                                itemTrackerData.ControlTypeName = "File Upload";
                                itemTrackerData.ControlDataId = 0;
                                itemTrackerData.ControlDataName = filepaths.Count > 0 ? filepaths[0] : "";
                                item.ControlDataName = itemTrackerData.ControlDataName;
                                if(filepaths.Count > 0)
                                filepaths.Remove(filepaths[0]);
                                break;
                            case ControlType.DateControl:
                                itemTrackerData.ControlTypeName = "Date";
                                itemTrackerData.ControlDataId = 0;
                                itemTrackerData.ControlDataName = item.ControlDataName;
                                break;
                        }

                        if (!string.IsNullOrEmpty(model.First().RowGuid) && itemTrackerData.Id > 0)
                            await _itemTrackerService.UpdateItemTrackerData(itemTrackerData);                        
                        else
                            await _itemTrackerService.SaveItemTrackerData(itemTrackerData);
                    }
                    await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Data", "Item Tracker Data Added/Updated by " + loggedInUserName, dataroom.Id,folderid:folder.Id,foldername:folder.FolderName, fileid: itemtrackermetadata.Id, filename: itemtrackermetadata.ItemTrackerName, dataroomname: dataroom.DataRoomName);
                }
                // Save Item Tracker history
                if (!string.IsNullOrEmpty(model.First().RowGuid))
                {
                    await SaveItemTrackerHistory(model, oldData);
                }
                new Thread(() => DataCache.RefreshItemTrackerData()).Start();

                _emailSender.SendEmailtoUser("ItemTrackerOperation", itemtrackerid: model.First().ItemTrackerId, statusFlag: "ItemTrackerDataAdd", loggedInUser: loggedInUserName);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " -- " + ex.StackTrace);
            }
            return Json("Item Tracker Saved Successfully");
        }

        public JsonResult GetItemTrackerDatabasedonDataRoom(int dataroomid,int folderid,int itemtrackerid)
        {
            try
            {
                ItemTrackerModel model = new ItemTrackerModel();
                model.DataRoomItemTrackerControls = new List<ItemTrackerControl>();
                model.DataRoomItemTrackerData = new List<ItemTrackerData>();
                var dataroomitemtrackercontrols = DataCache.ItemTrackerControls.Where(x => x.DataRoomId == dataroomid && x.IsActive == true && x.FolderId == folderid && x.ItemTrackerId == itemtrackerid);
                if (dataroomitemtrackercontrols != null && dataroomitemtrackercontrols.Count() > 0)
                {
                    model.DataRoomItemTrackerControls = dataroomitemtrackercontrols.ToList();
                }
                var dataroomitemtrackerdata = DataCache.ItemTrackerData.Where(x => x.DataRoomId == dataroomid && x.IsActive == true && x.FolderId == folderid && x.ItemTrackerId == itemtrackerid);
                if (dataroomitemtrackerdata != null && dataroomitemtrackerdata.Count() > 0)
                {
                    model.DataRoomItemTrackerData = dataroomitemtrackerdata.ToList();
                }

                JsonResult jr = Json(new
                {                    
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_itemtracker.cshtml", model)
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

        public JsonResult GetItemTrackerDatabasedonDataRoomFolderItemTracker(int dataroomid, int folderid, int itemtrackerid)
        {
            try
            {
                ItemTrackerModel model = new ItemTrackerModel();
                model.DataRoomItemTrackerControls = new List<ItemTrackerControl>();
                model.DataRoomItemTrackerData = new List<ItemTrackerData>();
                var dataroomitemtrackercontrols = DataCache.ItemTrackerControls.Where(x => x.DataRoomId == dataroomid && x.IsActive == true && x.FolderId == folderid && x.ItemTrackerId == itemtrackerid);
                if (dataroomitemtrackercontrols != null && dataroomitemtrackercontrols.Count() > 0)
                {
                    model.DataRoomItemTrackerControls = dataroomitemtrackercontrols.ToList();
                }
                var dataroomitemtrackerdata = DataCache.ItemTrackerData.Where(x => x.DataRoomId == dataroomid && x.IsActive == true && x.FolderId == folderid && x.ItemTrackerId == itemtrackerid);
                if (dataroomitemtrackerdata != null && dataroomitemtrackerdata.Count() > 0)
                {
                    model.DataRoomItemTrackerData = dataroomitemtrackerdata.ToList();
                }

                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_ItemTrackerDataListPart.cshtml", model)
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

        public JsonResult GetMasterDatabasedonParent(int parentid)
        {
            try
            {
                var data = DataCache.ItemTrackerControls.Where(x=>x.ControlReferenceId == parentid);
                if(data!=null && data.Count() > 0)
                {
                    return Json(data.ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> DeleteItemTracker(int itemtrackerid)
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            try
            {
                var itemtracker = DataCache.ItemTrackerMetaData.Where(x => x.Id == itemtrackerid);
                var dataroom = new DataRooms.UI.Models.DataRoom();
                if(itemtracker!=null && itemtracker.Count() > 0)
                {
                    var folder = itemtracker.First().FolderId > 0 ? DataCache.Folders.First(x => x.Id == itemtracker.First().FolderId) : new Folder();
                    dataroom = DataCache.DataRooms.First(x => x.Id == itemtracker.First().DataRoomId);
                    await _itemTrackerService.DeleteItemTracker(itemtracker.First());
                    await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Deletion", "Item Tracker - " + itemtracker.First().ItemTrackerName + " Deleted by " + loggedInUserName, dataroom.Id,fileid:itemtracker.First().Id,filename:itemtracker.First().ItemTrackerName, folderid: folder.Id, foldername: folder.FolderName, dataroomname: dataroom.DataRoomName);
                    _emailSender.SendEmailtoUser("ItemTrackerOperation", itemtrackerid: itemtrackerid, statusFlag: "ItemTrackerDelete",loggedInUser:loggedInUserName);
                    DataCache.RemoveItemTrackerMetaDatafromCache(itemtracker.First());
                }
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("Item Tracker Deleted Successfully");
        }

        public async Task<JsonResult> DeleteItemTrackerData(string rowguid)
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            try
            {
                var dataroom = new DataRooms.UI.Models.DataRoom();
                var itemTrackerData = DataCache.ItemTrackerData.Where(x => x.RowGuid == rowguid);
                if(itemTrackerData!=null && itemTrackerData.Count() > 0)
                {
                    dataroom = DataCache.DataRooms.First(x=>x.Id == itemTrackerData.First().DataRoomId);
                    var itemtracker = DataCache.ItemTrackerMetaData.First(x => x.Id == itemTrackerData.First().Id);
                    var folder = itemtracker.FolderId > 0 ? DataCache.Folders.First(x => x.Id == itemtracker.FolderId) : new Folder();
                    foreach (var item in itemTrackerData.ToList())
                    {
                        await _itemTrackerService.DeleteItemTrackerData(item);
                    }

                    await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Data Deletion", "Item Tracker Data Deleted by " + loggedInUserName, dataroom.Id, fileid: itemtracker.Id, filename: itemtracker.ItemTrackerName, folderid: folder.Id, foldername: folder.FolderName, dataroomname: dataroom.DataRoomName);
                    _emailSender.SendEmailtoUser("ItemTrackerOperation", itemtrackerid: itemtracker.Id, statusFlag: "ItemTrackerDataDelete", loggedInUser: loggedInUserName);
                }
                
                new Thread(() => DataCache.RefreshItemTrackerData()).Start();
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("Item Tracker Data Deleted Successfully");
        }

        public void DownloadItemTrackerFile(string guid)
        {
            try
            {
                var controlData = DataCache.ItemTrackerData.Where(x => x.ControlGuid == guid);
                if (controlData != null && controlData.Count() > 0)
                {
                    int dataroomid = controlData.First().DataRoomId;
                    var dataroom = DataCache.DataRooms.First(x => x.Id == dataroomid);
                    string encryptedFilePath = controlData.First().ControlDataName;
                    FileManager fileManager = new FileManager(string.Empty, dataroom.CompanyId);
                    string filename = System.IO.Path.GetFileName(encryptedFilePath);    
                    byte[] filebyteArray = fileManager.GetFileByteArray(encryptedFilePath);
                    if (filebyteArray != null)
                    {
                        string tempfilepath = Server.MapPath("~/Temp/") + filename;
                        System.IO.File.WriteAllBytes(tempfilepath, filebyteArray);
                        var decryptedfilepath = Server.MapPath("~/Temp/EncryptionTemp/") + filename;
                        encryption.DecryptFile(tempfilepath, decryptedfilepath);
                        HttpResponse response = System.Web.HttpContext.Current.Response;
                        response.Clear();
                        response.ClearContent();
                        response.ClearHeaders();
                        response.Buffer = true;
                        response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                        byte[] data = System.IO.File.ReadAllBytes(decryptedfilepath);
                        response.BinaryWrite(data);
                        response.End();
                        //await _logger.LogActivity(file.CompanyId, "File Download", "File - " + filename + " has been downloaded from " + file.FolderName + " under sharbox - " + file.DataRoomName, dataroomid: file.DataRoomId, dataroomname: file.DataRoomName, folderid: file.Id, foldername: file.FolderName, fileid: file.Id, filename: file.FileName);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> CreateItemTrackerMetaData(ItemTrackerModel model)
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            try
            {
                var dataroom = DataCache.DataRooms.First(x => x.Id == model.DataRoomId);
                var metadata = new ItemTrackerMetaData();
                metadata.ItemTrackerGuid = model.ItemTrackerGuid;
                metadata.ItemTrackerName = model.ItemTrackerName;
                metadata.FolderId = model.FolderId;
                metadata.FolderName = model.FolderName;
                metadata.DataRoomId = model.DataRoomId;
                metadata.DataRoomName = model.DataRoomName;
                metadata.IsActive = true;
                if (model.Id > 0)
                {
                    metadata.ModifiedBy = loggedInUser;
                    metadata.ModifierName = loggedInUserName;
                    metadata.ModifiedOn = DateTime.Now;
                    await _itemTrackerService.UpdateItemTracker(metadata);
                }
                else
                {
                    metadata.CreatedBy = loggedInUser;
                    metadata.CreatorName = loggedInUserName;
                    metadata.CreatedOn = DateTime.Now;
                    metadata.Id = await _itemTrackerService.SaveItemTracker(metadata);
                }
                //await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Creation", "Control " + metadata.ItemTrackerName + " Created in Item Tracker " + metadata.ItemTrackerName, dataroom.Id, 0, 0, dataroom.DataRoomName, null, null);
                DataCache.RefreshSingleItemTrackerMetaData(metadata);

                if (model.ItemTrackerPermissions == null)
                    model.ItemTrackerPermissions = new List<ItemTrackerPermission>();

                if (!model.ItemTrackerPermissions.Select(x => x.UserId).Contains(loggedInUser))
                {
                    // Admin Permissions
                    ItemTrackerPermission itemTrackerPermission = new ItemTrackerPermission();
                    itemTrackerPermission.DataRoomId = model.DataRoomId;
                    itemTrackerPermission.DataRoomName = model.DataRoomName;
                    itemTrackerPermission.FolderId = model.FolderId;
                    itemTrackerPermission.FolderName = model.FolderName;
                    itemTrackerPermission.ItemTrackerId = metadata.Id;
                    itemTrackerPermission.ItemTrackerName = model.ItemTrackerName;
                    itemTrackerPermission.IsActive = true;
                    itemTrackerPermission.HasFullControl = true;
                    itemTrackerPermission.UserId = loggedInUser;
                    itemTrackerPermission.UserName = loggedInUserName;
                    itemTrackerPermission.CreatedBy = loggedInUser;
                    itemTrackerPermission.CreatorName = loggedInUserName;
                    itemTrackerPermission.CreatedOn = DateTime.Now;
                    model.ItemTrackerPermissions.Add(itemTrackerPermission);
                }

                if (model.ItemTrackerPermissions!=null && model.ItemTrackerPermissions.Count > 0)
                {
                    foreach(var permission in model.ItemTrackerPermissions)
                    {
                        if(permission.IsActive != false)
                        {
                            permission.DataRoomId = model.DataRoomId;
                            permission.DataRoomName = model.DataRoomName;
                            permission.FolderId = model.FolderId;
                            permission.FolderName = model.FolderName;
                            permission.ItemTrackerId = metadata.Id;
                            permission.ItemTrackerName = metadata.ItemTrackerName;
                            permission.IsActive = true;
                            permission.CreatedBy = loggedInUser;
                            permission.CreatorName = loggedInUserName;
                            permission.CreatedOn = DateTime.Now;
                            permission.Id = await _itemTrackerService.SaveItemTrackerPermission(permission);
                            DataCache.RefreshSingleItemTrackerPermission(permission);
                        }
                    }
                }

                var itemtrackercontrols = DataCache.ItemTrackerControls.Where(x => x.ItemTrackerGuid == model.ItemTrackerGuid);
                if(itemtrackercontrols!=null && itemtrackercontrols.Count() > 0)
                {
                    foreach(var control in itemtrackercontrols.ToList())
                    {
                        control.ItemTrackerId = metadata.Id;
                        await _itemTrackerService.UpdateItemTrackerControl(control);
                        DataCache.RefreshSingleItemTrackerControl(control);
                    }
                }
                var folder = model.FolderId > 0 ? DataCache.Folders.First(x => x.Id == model.FolderId) : new Folder();
                await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Creation", "Item Tracker - " + model.ItemTrackerName + " Created by " + loggedInUserName, dataroom.Id,fileid: metadata.Id,filename:metadata.ItemTrackerName, folderid: folder.Id, foldername: folder.FolderName, dataroomname: dataroom.DataRoomName);

                _emailSender.SendEmailtoUser("ItemTrackerOperation", itemtrackerid: metadata.Id, statusFlag: "ItemTrackerAdded");
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("Success");
        }

        public async Task<JsonResult> DeleteItemTrackerMetaData(int id)
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            try
            {
                var dataroom = new DataRooms.UI.Models.DataRoom();
                var itemTrackerMetaData = DataCache.ItemTrackerMetaData.Where(x => x.Id == id);
                

                if (itemTrackerMetaData != null && itemTrackerMetaData.Count() > 0)
                {
                    var folder = itemTrackerMetaData.First().FolderId > 0 ? DataCache.Folders.First(x => x.Id == itemTrackerMetaData.First().FolderId) : new Folder();
                    await _itemTrackerService.DeleteItemTracker(itemTrackerMetaData.First());
                    _emailSender.SendEmailtoUser("ItemTrackerOperation", itemtrackerid: itemTrackerMetaData.First().Id, statusFlag: "ItemTrackerDeleted");
                    dataroom = DataCache.DataRooms.First(x => x.Id == itemTrackerMetaData.First().DataRoomId);
                    foreach (var item in itemTrackerMetaData.ToList())
                    {
                        await _itemTrackerService.DeleteItemTracker(item);
                    }
                    await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Deletion", "Item Tracker - "+ itemTrackerMetaData.First().ItemTrackerName + " Deleted by " + loggedInUserName, dataroom.Id,fileid: itemTrackerMetaData.First().Id,filename: itemTrackerMetaData.First().ItemTrackerName, folderid: folder.Id, foldername: folder.FolderName, dataroomname: dataroom.DataRoomName);
                }

                new Thread(() => DataCache.RefreshItemTrackerData()).Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Item Tracker Deleted Successfully");
        }

        public async Task SaveItemTrackerHistory(IEnumerable<ItemTrackerData> newData, IEnumerable<ItemTrackerData> originalData)
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            List<DeltaModel> deltaList = new List<DeltaModel>();
            try
            {
                var guid = newData.First().RowGuid;
                //var originalData = DataCache.ItemTrackerData.Where(x => x.RowGuid == guid);
                if(originalData!=null && originalData.Count() > 0)
                {
                    IEnumerable<ItemTrackerData> oldData = originalData.ToList();
                    List<string> oldControlGuids = oldData.Select(x => x.ControlGuid).Distinct().ToList();
                    List<string> newControlGuids = newData.Select(x => x.ControlGuid).Distinct().ToList();
                    foreach(var oldControl in oldControlGuids)
                    {
                        var oldControlExistsinNewData = newData.Where(x => x.ControlGuid == oldControl);
                        if(oldControlExistsinNewData!=null && oldControlExistsinNewData.Count() > 0)
                        {
                            var oldControlData = oldData.Where(x => x.ControlGuid == oldControl);
                            if (oldControlData != null && oldControlData.Count() > 0)
                            {
                                var newControlData = newData.Where(x => x.ControlGuid == oldControl);
                                if (newControlData != null && newControlData.Count() > 0)
                                {
                                    if (oldControlData.First().ControlDataName != newControlData.First().ControlDataName)
                                    {
                                        DeltaModel model = new DeltaModel();
                                        model.ColumnName = oldControlData.First().ControlName;
                                        model.OldValue = oldControlData.First().ControlDataName;
                                        model.NewValue = newControlData.First().ControlDataName;
                                        deltaList.Add(model);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var oldControlData = oldData.Where(x => x.ControlGuid == oldControl);
                            if (oldControlData != null && oldControlData.Count() > 0)
                            {
                                DeltaModel model = new DeltaModel();
                                model.ColumnName = oldControlData.First().ControlName;
                                model.OldValue = oldControlData.First().ControlDataName;
                                model.NewValue = "--";
                                deltaList.Add(model);
                            }
                        }
                    }


                    foreach (var newControl in newControlGuids)
                    {
                        var newControlExistsinOldData = oldData.Where(x => x.ControlGuid == newControl);
                        if (newControlExistsinOldData != null && newControlExistsinOldData.Count() > 0)
                        {
                            var newControlData = newData.Where(x => x.ControlGuid == newControl);
                            if (newControlData != null && newControlData.Count() > 0)
                            {
                                var oldControlData = oldData.Where(x => x.ControlGuid == newControl);
                                if (oldControlData != null && oldControlData.Count() > 0)
                                {
                                    if (oldControlData.First().ControlDataName != newControlData.First().ControlDataName && !deltaList.Select(x=>x.ColumnName).Contains(newControlData.First().ControlName))
                                    {
                                        DeltaModel model = new DeltaModel();
                                        model.ColumnName = oldControlData.First().ControlName;
                                        model.OldValue = oldControlData.First().ControlDataName;
                                        model.NewValue = newControlData.First().ControlDataName;
                                        deltaList.Add(model);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var newControlData = newData.Where(x => x.ControlGuid == newControl);
                            if (newControlData != null && newControlData.Count() > 0)
                            {
                                DeltaModel model = new DeltaModel();
                                model.ColumnName = newControlData.First().ControlName;
                                model.OldValue = "--";
                                model.NewValue = newControlData.First().ControlDataName;
                                deltaList.Add(model);
                            }
                        }
                    }
                }

                if(deltaList!=null && deltaList.Count() > 0)
                {
                    foreach(var item in deltaList)
                    {
                        var model = new ItemTrackerHistory();
                        model.ColumnName = item.ColumnName;
                        model.OldValue = item.OldValue;
                        model.NewValue = item.NewValue;
                        model.CreatedBy = loggedInUser;
                        model.CreatedByName = loggedInUserName;
                        model.CreatedOn = DateTime.Now;
                        model.ItemTrackerRowGuid = newData.First().RowGuid;
                        model.ItemTrackerId = newData.First().ItemTrackerId;
                        model.ItemTrackerName = newData.First().ItemTrackerName;
                        await _itemTrackerService.SaveItemTrackerHistory(model);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<JsonResult> GetItemTrackerHistory(int itemtrackerid,string rowguid)
        {
            try
            {
                IEnumerable<ItemTrackerHistory> history = await _itemTrackerService.GetItemTrackerHistory(itemtrackerid, rowguid);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_itemtrackerhistory.cshtml", history)
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

        public async Task DownloadItemTrackerData(int itemtrackerid,string reportType="PDF")
        {
            int loggedInUser = Convert.ToInt32(Session["UserId"]);
            string loggedInUserName = Convert.ToString(Session["UserName"]);
            
            try
            {
                var itemTrackerMetaData = DataCache.ItemTrackerMetaData.First(x=>x.Id == itemtrackerid);
                var folder = itemTrackerMetaData.FolderId > 0 ? DataCache.Folders.First(x => x.Id == itemTrackerMetaData.FolderId) : new Folder();
                var dataroom = DataCache.DataRooms.First(x => x.Id == itemTrackerMetaData.DataRoomId);
                FileConverter converter = new FileConverter();
                string viewContent = string.Empty;
                string reportPath = string.Empty;
                if (reportType == "PDF")
                {
                    viewContent = converter.GetHtmlContent(itemtrackerid);
                    reportPath = converter.SaveReport("PDF", viewContent, itemTrackerMetaData.ItemTrackerName);
                }
                else
                {
                    viewContent = converter.GetPlainContent(itemtrackerid);
                    reportPath = converter.SaveReport("Excel", viewContent, itemTrackerMetaData.ItemTrackerName);
                }                 
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                if (reportType == "PDF")
                {
                    response.AddHeader("Content-Disposition", "attachment;filename=" + itemTrackerMetaData.ItemTrackerName + ".pdf");
                }
                else
                {
                    response.AddHeader("Content-Disposition", "attachment;filename=" + itemTrackerMetaData.ItemTrackerName + ".csv");
                }                    
                byte[] data = System.IO.File.ReadAllBytes(reportPath);
                await _logger.LogActivity(dataroom.CompanyId, "Item Tracker Data Download", "Item Tracker - " + itemTrackerMetaData.ItemTrackerName + " Data Downloaded by " + loggedInUserName, dataroom.Id, fileid: itemTrackerMetaData.Id, filename: itemTrackerMetaData.ItemTrackerName, folderid: folder.Id, foldername: folder.FolderName, dataroomname: dataroom.DataRoomName);
                response.BinaryWrite(data);
                response.End();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        
    }
}