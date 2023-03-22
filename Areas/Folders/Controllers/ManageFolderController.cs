using DataRooms.UI.Code;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Folders.Controllers
{
    [SessionExpire]
    public class ManageFolderController : Controller
    {
        private FolderService _service { get; set; }
        private FileService _fileService { get; set; }
        private FolderPermissionService _folderPermissionService { get; set; }
        private LogService _logger { get; set; }
        public string _workspacepath = string.Empty;
        PermissionManager _permissionManager;
        FileManager _fileManager;
        public ManageFolderController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _workspacepath = ConfigurationManager.AppSettings["WorkspacePath"];
            _service = new FolderService(token);
            _fileService = new FileService(token);
            _folderPermissionService = new FolderPermissionService(token);
            _logger = new LogService(token);
            _permissionManager = new PermissionManager(token);
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            _fileManager = new FileManager(token, companyId);
        }


        //[HttpGet]
        //public async Task<ActionResult> List()
        //{
        //    try
        //    {
        //        IEnumerable<Folder> folders = await GetFoldersAsync("empty");
        //        return View(folders);
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
        //        IEnumerable<Folder> folders = await GetFoldersAsync(searchString);
        //        return View(folders);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<IEnumerable<Folder>> GetFoldersAsync(string searchString)
        //{
        //    try
        //    {
        //        return await _service.GetFoldersAsync(0, 0, searchString);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var user = new Folder();
                if (id > 0)
                    user = DataCache.Folders.Single(x=>x.Id == id);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Users\Views\Shared\_edituser.cshtml", user)
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
        public async Task<JsonResult> Edit(Folder folder)
        {
            try
            {
                
                if (folder.Id > 0)
                {
                    var originalfolder = DataCache.Folders.First(x => x.Id == folder.Id);
                    folder.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    folder.ModifierName = Convert.ToString(Session["UserName"]);
                    folder.ModifiedOn = DateTime.Now;
                    folder.CreatedBy = originalfolder.CreatedBy;
                    folder.CreatorName = originalfolder.CreatorName;
                    folder.CreatedOn = originalfolder.CreatedOn;
                    await _service.UpdateFolder(folder);
                    new Thread(() => DataCache.RefreshSingleFolder(folder)).Start();
                    await _logger.LogActivity(folder.CompanyId,"Folder Modification", "Folder - " + folder.FolderName + " has been modified under " + folder.DataRoomName,dataroomid: folder.DataRoomId, dataroomname: folder.DataRoomName, folderid: folder.Id,foldername: folder.FolderName);
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    folder.Guid = guid.ToString();
                    folder.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    folder.CreatorName = Convert.ToString(Session["UserName"]);
                    folder.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    folder.CompanyName = Convert.ToString(Session["CompanyName"]);
                    folder.CreatedOn = DateTime.Now;
                    int id = await _service.SaveFolder(folder);
                    folder.Id = id;
                    string relativePath = _fileManager.SaveFoldertoWorkSpace(id.ToString(), folder.ParentFolderId, folder.DataRoomId);
                    folder.RelativePath = relativePath;
                    await _service.UpdateFolder(folder);
                    new Thread(() => DataCache.RefreshSingleFolder(folder)).Start();
                    await _permissionManager.ApplyCreatorPermissionstoFolder(folder);
                    await _permissionManager.ApplyDataRoomPermissionstoFolder(folder);
                    await _logger.LogActivity(folder.CompanyId,"Folder Creation", "Folder - " + folder.FolderName + " has been created under " + folder.DataRoomName, dataroomid: folder.DataRoomId, dataroomname: folder.DataRoomName, folderid: folder.Id, foldername: folder.FolderName);
                }
                return Json(folder.ParentFolderId);
                //TempData["Notification"] = "Folder Saved Successfully";
                //return RedirectToAction("Index", "FileExplorer", new { area = "Explorer", dataroomid = folder.DataRoomId });
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
                Folder folder = DataCache.Folders.Single(x=>x.Id == id);
                folder.IsActive = false;
                folder.DeletedBy = Convert.ToInt32(Session["UserId"]);
                folder.DeletorName = Convert.ToString(Session["UserName"]);
                folder.DeletedOn = DateTime.Now;
                await _service.UpdateFolder(folder);
                new Thread(() => DataCache.RefreshSingleFolder(folder)).Start();
                await _logger.LogActivity(folder.CompanyId,"Folder Modification", "Folder - " + folder.FolderName + " has been deleted under " + folder.DataRoomName, dataroomid: folder.DataRoomId, dataroomname: folder.DataRoomName, folderid: folder.Id, foldername: folder.FolderName);
                return RedirectToAction("Index", "FileExplorer", new { area = "Explorer", dataroomid = folder.DataRoomId, folderid = folder.ParentFolderId });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult GetDataRoomWorkFlows(int dataroomid,int folderid)
        {
            try
            {
                var folder = DataCache.Folders.Single(x => x.Id == folderid);
                var dataroom = DataCache.DataRooms.Single(x => x.Id == dataroomid);
                ViewBag.DataRoomName = folder.DataRoomName;
                ViewBag.ParentFolder = folder.ParentFolderName;
                ViewBag.Folder = folder.FolderName;
                ViewBag.FolderId = folder.Id;
                ViewBag.WorkFlowId = folder.WorkFlowId;
                IEnumerable<WorkFlowMaster> workFlowMasters = null;
                var workflows = DataCache.WorkFlows.Where(x => x.DataRoomId == dataroomid);
                if (workflows != null && workflows.Count() > 0)
                {
                    workFlowMasters = workflows.ToList();
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Folders\Views\Shared\_assignworkflowtofolder.cshtml", workFlowMasters)
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

        public async Task<JsonResult> AssignWorkFlowtoFolder(int folderid,int workflowid)
        {
            int activityLogId = 0;
            try
            {
                var folder = DataCache.Folders.Single(x => x.Id == folderid);
                var workflow = DataCache.WorkFlows.Single(x => x.Id == workflowid);
                folder.WorkFlowId = workflow.Id;
                folder.WorkFlowName = workflow.WorkFlowName;
                folder.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                folder.ModifierName = Convert.ToString(Session["UserName"]);
                folder.ModifiedOn = DateTime.Now;
                await _service.UpdateFolder(folder);
                new Thread(() => DataCache.RefreshSingleFolder(folder)).Start();
                activityLogId = await _logger.LogActivity(folder.CompanyId,"Assign Work Flow to Folder", "Work Flow - " + workflow.WorkFlowName + " has been assigned to folder - " + folder.FolderName, folder.DataRoomId, folder.Id);                
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("Success",JsonRequestBehavior.AllowGet);
        }
    }
}