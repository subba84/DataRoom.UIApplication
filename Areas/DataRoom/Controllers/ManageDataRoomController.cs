using DataRooms.UI.Areas.DataRoom.Models;
using DataRooms.UI.Code;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.DataRoom.Controllers
{
    [SessionExpire]
    public class ManageDataRoomController : Controller
    {
        private DataRoomService _service { get; set; }
        private FolderService _folderService { get; set; }
        private FileService _fileService { get; set; }
        private LogService _logger { get; set; }
        private DataRoomPermissionService _dataRoomPermissionService { get; set; }
        public PermissionManager _permissionManager;
        public FileManager _fileManager;
        public string _workspacepath = string.Empty;

        public ManageDataRoomController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new DataRoomService(token);
            _folderService = new FolderService(token);
            _fileService = new FileService(token);
            _logger = new LogService(token);
            _permissionManager = new PermissionManager(token);
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            _fileManager = new FileManager(token, companyId);
            _dataRoomPermissionService = new DataRoomPermissionService(token);
            _workspacepath = ConfigurationManager.AppSettings["WorkspacePath"];
        }

        #region DataRooms for Auto Complete
        /// <summary>
        /// Data Rooms for Auto Complete in Data Room Permission Page
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public ActionResult GetAllDataRooms(string searchString)
        {
            try
            {
                IEnumerable<DataRooms.UI.Models.DataRoom> rooms = DataCache.DataRooms.Where(x => x.DataRoomName.ToLower().Contains(searchString.ToLower()));                
                return Json(rooms, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        [HttpGet]
        public ActionResult List()
        {
            try
            {
                var model = new DataRoomCustomModel();
                GetAllDataRoomsAsync(model);
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
                GetAllDataRoomsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetAllDataRoomsAsync(DataRoomCustomModel model)
        {
            try
            {
                int companyId = Convert.ToInt32(Session["CompanyId"]);
                IEnumerable<DataRooms.UI.Models.DataRoom> datarooms = null;
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    datarooms = DataCache.DataRooms.Where(x => x.IsActive == true && x.CompanyId == companyId && x.DataRoomName.ToLower().Contains(model.SearchString.ToLower()));
                }
                else
                {
                    datarooms = DataCache.DataRooms.Where(x=>x.IsActive == true && x.CompanyId == companyId);
                }
                //IPagedList<DataRooms.UI.Models.DataRoom> pageddatarooms = null;
                //switch (model.SortColumn)
                //{
                //    case "CreatedOn":
                //        if (model.SortOrder.Equals("desc"))
                //            pageddatarooms = datarooms.OrderByDescending
                //                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        else
                //            pageddatarooms = datarooms.OrderBy
                //                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //    case "DataRoomName":
                //        if (model.SortOrder.Equals("desc"))
                //            pageddatarooms = datarooms.OrderByDescending
                //                    (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        else
                //            pageddatarooms = datarooms.OrderBy
                //                    (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //}
                //model.PagedDataRooms = pageddatarooms;
                model.DataRooms = datarooms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var dataroom = new DataRooms.UI.Models.DataRoom();
                if (id > 0)
                    dataroom = DataCache.DataRooms.Single(x=>x.Id == id);

                var companyId = Convert.ToInt32(Session["CompanyId"]);
                var company = DataCache.Companies.Single(x => x.Id == companyId);
                if(company.IsLogsRequired == true)
                {
                    dataroom.IsLogsRequiredforCompany = true;
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_editdataroom.cshtml", dataroom)
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
        public async Task<ActionResult> Edit(DataRooms.UI.Models.DataRoom dataroom)
        {
            try
            {
                int dataroomid = 0;
                dataroom.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                dataroom.CompanyName = Convert.ToString(Session["CompanyName"]);
                var company = DataCache.Companies.Single(x=>x.Id == dataroom.CompanyId);                
                if (dataroom.Id > 0)
                {
                    var originaldataroom = DataCache.DataRooms.First(x=>x.Id == dataroom.Id);
                    dataroom.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    dataroom.ModifierName = Convert.ToString(Session["UserName"]);                    
                    dataroom.ModifiedOn = DateTime.Now;
                    dataroom.RelativePath = originaldataroom.RelativePath;
                    dataroom.CreatedBy = originaldataroom.CreatedBy;
                    dataroom.CreatorName = originaldataroom.CreatorName;
                    dataroom.CreatedOn = originaldataroom.CreatedOn;
                    dataroomid = dataroom.Id;
                    await _service.UpdateDataRoom(dataroom);
                    new Thread(() => DataCache.RefreshSingleDataRoom(dataroom)).Start();
                    int activityLogId = await _logger.LogActivity(dataroom.CompanyId, "SharBox Modification", "SharBox - " + dataroom.DataRoomName + " has been modified",dataroomid: dataroom.Id,dataroomname: dataroom.DataRoomName);
                    await _logger.LogDataDifference(activityLogId, originaldataroom, dataroom,dataRoomId: originaldataroom.Id);
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    dataroom.Guid = guid.ToString();                                       
                    dataroom.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    dataroom.CreatorName = Convert.ToString(Session["UserName"]);
                    dataroom.CreatedOn = DateTime.Now;
                    dataroom.Id = await _service.SaveDataRoom(dataroom);
                    string relativePath = _fileManager.SaveDataRoomtoWorkSpace(company.RelativePath, dataroom.Id.ToString());
                    dataroom.RelativePath = company.RelativePath + "/" + dataroom.Id;
                    await _service.UpdateDataRoom(dataroom);
                    await _permissionManager.ManageDataRoomPermission(dataroom);
                    //new Thread(() => DataCache.RefreshSingleDataRoom(dataroom)).Start();
                    DataCache.RefreshSingleDataRoom(dataroom);
                    await _logger.LogActivity(dataroom.CompanyId, "SharBox Creation", "SharBox - " + dataroom.DataRoomName + " has been created",dataroomid: dataroom.Id, dataroomname: dataroom.DataRoomName);
                }
                TempData["Notification"] = "SharBox Saved Successfully";
                return RedirectToAction("List", "ManageDataRoom", new { area = "DataRoom" });
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
                var dataroom = DataCache.DataRooms.Single(x=>x.Id == id);
                // Delete Data Room
                dataroom.IsActive = false;
                dataroom.DeletedBy = Convert.ToInt32(Session["UserId"]);
                dataroom.DeletorName = Convert.ToString(Session["UserName"]);
                dataroom.DeletedOn = DateTime.Now;
                await _service.UpdateDataRoom(dataroom);
                new Thread(() => DataCache.RefreshSingleDataRoom(dataroom)).Start();
                await _logger.LogActivity(dataroom.CompanyId, "SharBox Deletion", "SharBox - " + dataroom.DataRoomName + " has been deleted", dataroomid: dataroom.Id, dataroomname: dataroom.DataRoomName);
                // Delete Folders of Data Room
                //var folders = DataCache.Folders.Where(x => x.DataRoomId == dataroom.Id);
                //if(folders!=null && folders.Count() > 0)
                //{
                //    foreach(var folder in folders.ToList())
                //    {
                //        folder.IsActive = false;
                //        folder.DeletedBy = Convert.ToInt32(Session["UserId"]);
                //        folder.DeletorName = Convert.ToString(Session["UserName"]);
                //        folder.DeletedOn = DateTime.Now;
                //        await _folderService.UpdateFolder(folder);
                //        new Thread(() => DataCache.RefreshSingleFolder(folder)).Start();
                //        await _logger.LogActivity("Folder Deletion", "Folder - " + folder.FolderName + " has been deleted", dataroomid: dataroom.Id, dataroomname: dataroom.DataRoomName,folderid:folder.Id,foldername:folder.FolderName);
                //    }
                //}

                ////var files = DataCache.Files.Where(x => x.DataRoomId == dataroom.Id);
                ////if (files != null && files.Count() > 0)
                ////{
                ////    foreach (var file in files.ToList())
                ////    {
                ////        file.IsActive = false;
                ////        file.DeletedBy = Convert.ToInt32(Session["UserId"]);
                ////        file.DeletorName = Convert.ToString(Session["UserName"]);
                ////        file.DeletedOn = DateTime.Now;
                ////        await _fileService.UpdateFile(file);
                ////        new Thread(() => DataCache.RefreshSingleFile(file)).Start();
                ////        await _logger.LogActivity("File Deletion", "File - " + file.FileName + " has been deleted", dataroomid: dataroom.Id, dataroomname: dataroom.DataRoomName, folderid: file.FolderId, foldername: file.FolderName,fileid:file.Id,filename:file.FileName);
                ////    }
                ////}

                //DeleteDataRoomfromWorkSpace(dataroom.DataRoomName);                
                TempData["Notification"] = "SharBox Deleted Successfully";
                return RedirectToAction("List", "ManageDataRoom", new { area = "DataRoom" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        
        
    }
}