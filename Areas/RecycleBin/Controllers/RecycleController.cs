using DataRooms.UI.Areas.RecycleBin.Model;
using DataRooms.UI.Code;
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

namespace DataRooms.UI.Areas.RecycleBin.Controllers
{
    [SessionExpire]
    public class RecycleController : Controller
    {
        private DataRoomService _service { get; set; }
        private FolderService _folderService { get; set; }
        private FileService _fileService { get; set; }
        private LogService _logger { get; set; }
        public string _workspacepath = string.Empty;
        private FileManager _fileManager;
        

        public RecycleController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new DataRoomService(token);
            _folderService = new FolderService(token);
            _fileService = new FileService(token);
            _logger = new LogService(token);
            _workspacepath = ConfigurationManager.AppSettings["WorkspacePath"];
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            _fileManager = new FileManager(token, companyId);
        }


        [HttpGet]
        public ActionResult List()
        {
            var model = new RecycleBinModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(RecycleBinModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(RecycleBinModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    var datarooms = DataCache.DataRooms.Where(x => x.IsActive == false && x.DataRoomName.ToLower().Contains(model.SearchString.ToLower()));
                    if (datarooms != null && datarooms.Count() > 0)
                    {
                        model.DataRooms = datarooms;
                    }
                    var folders = DataCache.Folders.Where(x => x.IsActive == false && x.FolderName.ToLower().Contains(model.SearchString.ToLower()));
                    if (folders != null && folders.Count() > 0)
                    {
                        model.Folders = folders;
                    }
                    var files = DataCache.Files.Where(x => x.IsActive == false && x.FileName.ToLower().Contains(model.SearchString.ToLower()));
                    if (files != null && files.Count() > 0)
                    {
                        model.Files = files;
                    }
                }
                else
                {
                    var datarooms = DataCache.DataRooms.Where(x => x.IsActive == false);
                    if (datarooms != null && datarooms.Count() > 0)
                    {
                        model.DataRooms = datarooms;
                    }
                    var folders = DataCache.Folders.Where(x => x.IsActive == false);
                    if (folders != null && folders.Count() > 0)
                    {
                        model.Folders = folders;
                    }
                    var files = DataCache.Files.Where(x => x.IsActive == false);
                    if (files != null && files.Count() > 0)
                    {
                        model.Files = files;
                    }
                }
                var adminRole = AppRole.Admin;
                if (adminRole == Convert.ToInt32(Session["CurrentRoleId"]))
                {
                    // All Data will be available for Admin
                }
                else
                {
                    var loggedInUser = Convert.ToInt32(Session["UserId"]);
                    var dataroompermissions = DataCache.DataRoomPermissions.Where(x => x.UserId == loggedInUser && (x.HasFullControl == true || x.HasDelete == true));
                    if (dataroompermissions != null && dataroompermissions.Count() > 0)
                    {
                        List<int> dataroomids = dataroompermissions.Select(x => x.DataRoomId).Distinct().ToList();
                        if(model.DataRooms!=null && model.DataRooms.Count() > 0)
                        {
                            model.DataRooms = model.DataRooms.Where(x => dataroomids.Contains(x.Id)).ToList();
                        }
                    }
                    var folderpermissions = DataCache.FolderPermissions.Where(x => x.UserId == loggedInUser && (x.HasFullControl == true || x.HasDelete == true));
                    if (folderpermissions != null && folderpermissions.Count() > 0)
                    {
                        List<int> folderids = folderpermissions.Select(x => x.FolderId).Distinct().ToList();
                        if(model.Folders!=null && model.Folders.Count() > 0)
                        {
                            model.Folders = model.Folders.Where(x => folderids.Contains(x.Id)).ToList();
                        }
                    }
                    var filepermissions = DataCache.FilePermissions.Where(x => x.UserId == loggedInUser && (x.HasFullControl == true || x.HasDelete == true));
                    if (filepermissions != null && filepermissions.Count() > 0)
                    {
                        List<int> fileids = filepermissions.Select(x => x.FileId).Distinct().ToList();
                        if(model.Files!=null && model.Files.Count() > 0)
                        {
                            model.Files = model.Files.Where(x => fileids.Contains(x.Id)).ToList();
                        }
                    }
                }
                
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> Restore(int id, string flag)
        {
            try
            {
                if (flag == "Data Room")
                {
                    var dataroom = DataCache.DataRooms.Single(x => x.Id == id);
                    dataroom.IsActive = true;
                    dataroom.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    dataroom.ModifierName = Convert.ToString(Session["UserName"]);
                    dataroom.ModifiedOn = DateTime.Now;
                    await _service.UpdateDataRoom(dataroom);
                    new Thread(() => DataCache.RefreshSingleDataRoom(dataroom)).Start();
                    await _logger.LogActivity(dataroom.CompanyId, "SharBox Restoration", "SharBox - " + dataroom.DataRoomName + " has been restored",
                        dataroomid: dataroom.Id, dataroomname: dataroom.DataRoomName);
                    TempData["Message"] = "SharBox - " + dataroom.DataRoomName + " restored successfully";
                }
                else if (flag == "Folder")
                {
                    Folder folder = DataCache.Folders.Single(x => x.Id == id);
                    folder.IsActive = true;
                    folder.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    folder.ModifierName = Convert.ToString(Session["UserName"]);
                    folder.ModifiedOn = DateTime.Now;
                    await _folderService.UpdateFolder(folder);
                    new Thread(() => DataCache.RefreshSingleFolder(folder)).Start();
                    await _logger.LogActivity(folder.CompanyId, "Folder Restoration", "Folder - " + folder.FolderName + " has been restored under "
                        + folder.DataRoomName, dataroomid: folder.DataRoomId, dataroomname: folder.DataRoomName, folderid: folder.Id,
                        foldername: folder.FolderName);
                    TempData["Message"] = "Folder - " + folder.FolderName + " restored successfully";
                }
                else
                {
                    var file = DataCache.Files.Single(x => x.Id == id);
                    file.IsActive = true;
                    file.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    file.ModifierName = Convert.ToString(Session["UserName"]);
                    file.ModifiedOn = DateTime.Now;
                    await _fileService.UpdateFile(file);
                    new Thread(() => DataCache.RefreshSingleFile(file)).Start();
                    await _logger.LogActivity(file.CompanyId,"File Restoration", "File - " + file.FileName + " has been restored in " + file.FolderName + " " +
                        "under sharbox - " + file.DataRoomName, dataroomid: file.DataRoomId, dataroomname: file.DataRoomName,
                        folderid: file.Id, foldername: file.FolderName, fileid: file.Id, filename: file.FileName);
                    TempData["Message"] = "File - " + file.FileName + " restored successfully";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("List");
        }

        public async Task<ActionResult> Delete(int id,string flag)
        {
            try
            {
                if(flag == "Data Room")
                {
                    var dataroom = DataCache.DataRooms.Single(x => x.Id == id);
                    dataroom.IsActive = false;
                    dataroom.DeletedBy = Convert.ToInt32(Session["UserId"]);
                    dataroom.DeletorName = Convert.ToString(Session["UserName"]);
                    dataroom.DeletedOn = DateTime.Now;
                    await _service.DeleteDataRoom(dataroom);
                    new Thread(() => DataCache.RemoveDataRoomfromCache(dataroom)).Start();
                    _fileManager.DeleteDataRoomfromWorkSpace(dataroom.RelativePath);
                    await _logger.LogActivity(dataroom.CompanyId,"SharBox Permanent Deletion", "SharBox - " + dataroom.DataRoomName + " has been deleted", 
                        dataroomid: dataroom.Id, dataroomname: dataroom.DataRoomName);
                    TempData["Message"] = "SharBox - " + dataroom.DataRoomName + " deleted successfully";
                }
                else if(flag == "Folder")
                {
                    Folder folder = DataCache.Folders.Single(x => x.Id == id);
                    folder.IsActive = false;
                    folder.DeletedBy = Convert.ToInt32(Session["UserId"]);
                    folder.DeletorName = Convert.ToString(Session["UserName"]);
                    folder.DeletedOn = DateTime.Now;
                    await _folderService.DeleteFolder(folder);
                    //DeleteFolderfromWorkSpace(folder.RelativePath);
                    _fileManager.DeleteFolderfromWorkSpace(folder.RelativePath);
                    new Thread(() => DataCache.RemoveFolderfromCache(folder)).Start();
                    _fileManager.DeleteFolderfromWorkSpace(folder.RelativePath);
                    await _logger.LogActivity(folder.CompanyId,"Folder Parmanent Deletion", "Folder - " + folder.FolderName + " has been deleted under " 
                        + folder.DataRoomName, dataroomid: folder.DataRoomId, dataroomname: folder.DataRoomName, folderid: folder.Id, 
                        foldername: folder.FolderName);
                    TempData["Message"] = "Folder - " + folder.FolderName + " deleted successfully";
                }
                else
                {
                    var file = DataCache.Files.Single(x => x.Id == id);
                    file.IsActive = false;
                    file.DeletedBy = Convert.ToInt32(Session["UserId"]);
                    file.DeletorName = Convert.ToString(Session["UserName"]);
                    file.DeletedOn = DateTime.Now;
                    await _fileService.DeleteFile(file);
                    new Thread(() => DataCache.RemoveFilefromCache(file)).Start();
                    _fileManager.DeleteFilefromWorkSpace(file.RelativePath);
                    await _logger.LogActivity(file.CompanyId,"File Permanent Delete", "File - " + file.FileName + " has been deleted in " + file.FolderName + " " +
                        "under sharbox - " + file.DataRoomName, dataroomid: file.DataRoomId, dataroomname: file.DataRoomName,
                        folderid: file.Id, foldername: file.FolderName, fileid: file.Id, filename: file.FileName);
                    TempData["Message"] = "File - " + file.FileName + " deleted successfully";
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("List");
        }

        public async Task<ActionResult> EmptyRecycleBin()
        {
            try
            {
                RecycleBinModel model = new RecycleBinModel();
                PrepareList(model);
                if(model.DataRooms!=null && model.DataRooms.Count() > 0)
                {
                    foreach(var dataroom in model.DataRooms.ToList())
                    {
                        dataroom.IsActive = false;
                        dataroom.DeletedBy = Convert.ToInt32(Session["UserId"]);
                        dataroom.DeletorName = Convert.ToString(Session["UserName"]);
                        dataroom.DeletedOn = DateTime.Now;
                        await _service.DeleteDataRoom(dataroom);
                        new Thread(() => DataCache.RemoveDataRoomfromCache(dataroom)).Start();
                        await _logger.LogActivity(dataroom.CompanyId,"SharBox Permanent Deletion", "SharBox - " + dataroom.DataRoomName + " has been deleted",
                            dataroomid: dataroom.Id, dataroomname: dataroom.DataRoomName);
                    }
                }
                if (model.Folders != null && model.Folders.Count() > 0)
                {
                    foreach (var folder in model.Folders.ToList())
                    {
                        folder.IsActive = false;
                        folder.DeletedBy = Convert.ToInt32(Session["UserId"]);
                        folder.DeletorName = Convert.ToString(Session["UserName"]);
                        folder.DeletedOn = DateTime.Now;
                        await _folderService.DeleteFolder(folder);
                        DeleteFolderfromWorkSpace(folder.RelativePath);
                        new Thread(() => DataCache.RemoveFolderfromCache(folder)).Start();
                        await _logger.LogActivity(folder.CompanyId,"Folder Parmanent Deletion", "Folder - " + folder.FolderName + " has been deleted under "
                            + folder.DataRoomName, dataroomid: folder.DataRoomId, dataroomname: folder.DataRoomName, folderid: folder.Id,
                            foldername: folder.FolderName);
                    }
                }
                if (model.Files != null && model.Files.Count() > 0)
                {
                    foreach (var file in model.Files.ToList())
                    {
                        file.IsActive = false;
                        file.DeletedBy = Convert.ToInt32(Session["UserId"]);
                        file.DeletorName = Convert.ToString(Session["UserName"]);
                        file.DeletedOn = DateTime.Now;
                        await _fileService.DeleteFile(file);
                        new Thread(() => DataCache.RemoveFilefromCache(file)).Start();
                        _fileManager.DeleteFilefromWorkSpace(file.RelativePath);
                        await _logger.LogActivity(file.CompanyId, "File Permanent Delete", "File - " + file.FileName + " has been deleted in " + file.FolderName + " " +
                            "under sharbox - " + file.DataRoomName, dataroomid: file.DataRoomId, dataroomname: file.DataRoomName,
                            folderid: file.Id, foldername: file.FolderName, fileid: file.Id, filename: file.FileName);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            TempData["Message"] = "Recycle Bin Empty Successful";
            return RedirectToAction("List");
        }

        

        public void DeleteFolderfromWorkSpace(string relativePath)
        {
            try
            {
                string folderpath = System.IO.Path.Combine(_workspacepath, relativePath);
                if (System.IO.Directory.Exists(folderpath))
                {
                    if (System.IO.Directory.Exists(folderpath))
                        System.IO.Directory.Delete(folderpath, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteDataRoomfromWorkSpace(string dataroomname)
        {
            try
            {
                new Thread(() => {
                    string dataroomPath = System.IO.Path.Combine(_workspacepath, dataroomname);
                    if (System.IO.Directory.Exists(dataroomPath))
                    {
                        System.IO.Directory.Delete(dataroomPath, true);
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}