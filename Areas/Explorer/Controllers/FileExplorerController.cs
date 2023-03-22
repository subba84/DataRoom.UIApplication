using AutoMapper;
using DataRooms.UI.Areas.Explorer.Model;
using DataRooms.UI.Code;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Explorer.Controllers
{
    [SessionExpire]
    public class FileExplorerController : Controller
    {
        FileService _fileService { get; set; }
        FolderService _folderService { get; set; }
        public int _userId { get; set; }
        public string IsFileCacheEnabled = null;
        public bool _isFileCachingEnabled = false;
        public IMapper _mapper;
        public string _workspacepath = string.Empty;
        private LogService _logger { get; set; }
        private PermissionManager _permissionManager;
        private FileManager _fileManager;

        public FileExplorerController(IMapper mapper)
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _fileService = new FileService(token);
            _folderService = new FolderService(token);
            _permissionManager = new PermissionManager(token);
            int companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            _fileManager = new FileManager(token, companyId);
            _userId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
            _logger = new LogService(token);
            IsFileCacheEnabled = ConfigurationManager.AppSettings["IsFileCacheEnabled"];
            if (IsFileCacheEnabled == "Y")
            {
                _isFileCachingEnabled = true;
            }
            _mapper = mapper;
            _workspacepath = ConfigurationManager.AppSettings["WorkspacePath"];
        }

        [HttpGet]
        public async Task<ActionResult> Index(int dataroomid=0,int folderid = 0)
        {
            ExplorerCustomModel model = new ExplorerCustomModel();
            await GetFoldersandFiles(dataroomid, folderid, (folderid > 0 ? true : false), model);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ExplorerCustomModel model)
        {
            await GetFoldersandFiles(model.DataRoom.Id, model.Folder.Id, false, model);
            return View(model);
        }


        public async Task GetFoldersandFiles(int dataroomid, int folderid, bool isParentFolder, ExplorerCustomModel model)
        {
            try
            {
                model.DataRoom = DataCache.DataRooms.First(x => x.IsActive == true && x.Id == dataroomid);
                string breadcrumbpath = "";
                if(folderid == 0)
                {
                    breadcrumbpath = "<a href='#' class='breadcrumbpart' data-dataroomid='" + model.DataRoom.Id + "' data-folderid='0'>" + model.DataRoom.DataRoomName + "</a>";
                }
                else
                {
                    breadcrumbpath = _fileManager.BuildBreadCrumbs(folderid);
                }
                model.FolderTreeView = breadcrumbpath;
                model.IsParentFolder = isParentFolder;
                model.SortColumn = string.IsNullOrEmpty(model.SortColumn) ? "CreatedOn" : model.SortColumn;
                
                model.DataRoomPermission = DataCache.DataRoomPermissions.FirstOrDefault(x => x.DataRoomId == dataroomid && x.UserId == _userId);
                if(folderid > 0)
                {
                    model.FolderPermission = DataCache.FolderPermissions.FirstOrDefault(x => x.FolderId == folderid && x.UserId == _userId);
                }
                else
                {
                    model.FolderPermission = new FolderPermission();
                }
                model.Folder = folderid > 0 ? DataCache.Folders.First(x=>x.IsActive == true && x.Id == folderid) : new Folder();
                List<DataRoomContentModel> foldersandfiles = new List<DataRoomContentModel>();
                IEnumerable<Folder> folders = DataCache.Folders.Where(x => x.IsActive == true).Where(x => x.DataRoomId == dataroomid);
                IEnumerable<ItemTrackerMetaData> itemTrackers = DataCache.ItemTrackerMetaData.Where(x => x.IsActive == true).Where(x => x.DataRoomId == dataroomid && x.FolderId == folderid);
                //model.FolderTreeView = GetDOMTreeView(folders.ToList(), model.DataRoom.Id, model.DataRoom.DataRoomName);
                model.Folders = !isParentFolder ? folders.Where(x => x.DataRoomId == dataroomid && x.ParentFolderId == 0).ToList() : folders.Where(x => x.ParentFolderId == folderid).ToList();
                model.ItemTrackers = isParentFolder ? itemTrackers.Where(x => x.DataRoomId == dataroomid).ToList() : itemTrackers.Where(x => x.FolderId == folderid).ToList();
                model.IsFileCachingEnabled = _isFileCachingEnabled;
                var loggedInUserRole = Convert.ToInt32(Session["CurrentRoleId"]);
                if (model.Folder != null && model.Folders.Count() > 0)
                {                    
                    if(loggedInUserRole == AppRole.Admin)
                    {
                        if (model.Folders != null && model.Folders.Count() > 0)
                            model.FolderswithPemrissions = _mapper.Map<IEnumerable<Folder>, IEnumerable<FolderwithPermission>>(model.Folders);
                        if (model.FolderswithPemrissions == null)
                            model.FolderswithPemrissions = new List<FolderwithPermission>();
                        if (model.FolderswithPemrissions != null && model.FolderswithPemrissions.Count() > 0)
                        {
                            model.FolderswithPemrissions = model.FolderswithPemrissions.Select(x =>
                            {
                                x.FolderPermission = new FolderPermission { HasFullControl = true, FolderId = x.Id, FolderName=x.FolderName };
                                //x.IsFolderExists = _fileManager.IsFolderExists(x.Id);
                                return x;
                            }).ToList();
                        }
                    }
                    else
                    {
                        List<int> folderIds = new List<int>();
                        var folderPermissionDetails = DataCache.FolderPermissions.Where(x => x.UserId == _userId);
                        if (folderPermissionDetails != null && folderPermissionDetails.Count() > 0)
                        {
                            folderIds = folderPermissionDetails.Select(x => x.FolderId).ToList();
                        }
                        model.Folders = model.Folders.Where(x => folderIds.Contains(x.Id));
                        if (model.Folders != null && model.Folders.Count() > 0)
                            model.FolderswithPemrissions = _mapper.Map<IEnumerable<Folder>, IEnumerable<FolderwithPermission>>(model.Folders);
                        if (model.FolderswithPemrissions == null)
                            model.FolderswithPemrissions = new List<FolderwithPermission>();
                        if (model.FolderswithPemrissions != null && model.FolderswithPemrissions.Count() > 0)
                        {
                            model.FolderswithPemrissions = model.FolderswithPemrissions.Select(x =>
                            {
                                x.FolderPermission = DataCache.FolderPermissions.Where(y => y.FolderId == x.Id && y.UserId == _userId && y.IsActive == true).FirstOrDefault();
                                //x.IsFolderExists = _fileManager.IsFolderExists(x.Id);
                                return x;
                            }).ToList();
                        }
                    }                    
                }

                if(model.ItemTrackers != null && model.ItemTrackers.Count() > 0)
                {
                    if (loggedInUserRole == AppRole.Admin)
                    {
                        //model.ItemTrackersswithPemrissions = _mapper.Map<IEnumerable<ItemTrackerMetaData>, IEnumerable<ItemTrackerwithPermission>>(model.ItemTrackers);
                        if (model.ItemTrackers != null && model.ItemTrackers.Count() > 0)
                        {
                            List<ItemTrackerwithPermission> permissionList = new List<ItemTrackerwithPermission>();
                            foreach (var item in model.ItemTrackers)
                            {
                                ItemTrackerwithPermission itemTrackerwithPermission = new ItemTrackerwithPermission();
                                itemTrackerwithPermission.Id = item.Id;
                                itemTrackerwithPermission.ItemTrackerName = item.ItemTrackerName;
                                itemTrackerwithPermission.DataRoomId = item.DataRoomId;
                                itemTrackerwithPermission.DataRoomName = item.DataRoomName;
                                itemTrackerwithPermission.FolderId = item.FolderId;
                                itemTrackerwithPermission.FolderName = item.FolderName;
                                itemTrackerwithPermission.CreatorName = item.CreatorName;
                                itemTrackerwithPermission.CreatedOn = item.CreatedOn;
                                itemTrackerwithPermission.ItemTrackerPermission = new ItemTrackerPermission { ItemTrackerId = item.Id, ItemTrackerName = item.ItemTrackerName, DataRoomId = item.DataRoomId, DataRoomName = item.DataRoomName, FolderId = item.FolderId, FolderName = item.FolderName,CreatedOn = item.CreatedOn };
                                permissionList.Add(itemTrackerwithPermission);
                            }
                            model.ItemTrackersswithPemrissions = permissionList;
                        }
                        if (model.ItemTrackersswithPemrissions == null)
                            model.ItemTrackersswithPemrissions = new List<ItemTrackerwithPermission>();
                        if (model.ItemTrackersswithPemrissions != null && model.ItemTrackersswithPemrissions.Count() > 0)
                        {
                            model.ItemTrackersswithPemrissions = model.ItemTrackersswithPemrissions.Select(x =>
                            {
                                x.ItemTrackerPermission = new ItemTrackerPermission { HasFullControl = true, ItemTrackerId = x.Id, ItemTrackerName = x.ItemTrackerName,CreatorName = x.CreatorName, CreatedOn = x.CreatedOn };
                                //x.IsFolderExists = _fileManager.IsFolderExists(x.Id);
                                return x;
                            }).ToList();
                        }
                    }
                    else
                    {
                        List<int> trackerIds = new List<int>();
                        var itemTrackerPermissionDetails = DataCache.ItemTrackerPermissions.Where(x => x.UserId == _userId);
                        if (itemTrackerPermissionDetails != null && itemTrackerPermissionDetails.Count() > 0)
                        {
                            trackerIds = itemTrackerPermissionDetails.Select(x => x.ItemTrackerId).ToList();
                        }
                        model.ItemTrackers = model.ItemTrackers.Where(x => trackerIds.Contains(x.Id));
                        if (model.ItemTrackers != null && model.ItemTrackers.Count() > 0)
                        {
                            List<ItemTrackerwithPermission> permissionList = new List<ItemTrackerwithPermission>();
                            foreach (var item in model.ItemTrackers)
                            {
                                ItemTrackerwithPermission itemTrackerwithPermission = new ItemTrackerwithPermission();
                                itemTrackerwithPermission.Id = item.Id;
                                itemTrackerwithPermission.ItemTrackerName = item.ItemTrackerName;
                                itemTrackerwithPermission.DataRoomId = item.DataRoomId;
                                itemTrackerwithPermission.DataRoomName = item.DataRoomName;
                                itemTrackerwithPermission.FolderId = item.FolderId;
                                itemTrackerwithPermission.FolderName = item.FolderName;
                                itemTrackerwithPermission.CreatorName = item.CreatorName;
                                itemTrackerwithPermission.CreatedOn = item.CreatedOn;
                                itemTrackerwithPermission.ItemTrackerPermission = new ItemTrackerPermission { ItemTrackerId = item.Id, ItemTrackerName = item.ItemTrackerName, DataRoomId = item.DataRoomId, DataRoomName = item.DataRoomName, FolderId = item.FolderId, FolderName = item.FolderName,CreatedOn = item.CreatedOn };
                                permissionList.Add(itemTrackerwithPermission);
                            }
                            model.ItemTrackersswithPemrissions = permissionList;
                            if (model.ItemTrackersswithPemrissions != null && model.ItemTrackersswithPemrissions.Count() > 0)
                            {
                                model.ItemTrackersswithPemrissions = model.ItemTrackersswithPemrissions.Select(x =>
                                {
                                    x.ItemTrackerPermission = DataCache.ItemTrackerPermissions.Where(y => y.ItemTrackerId == x.Id && y.UserId == _userId && y.IsActive == true).FirstOrDefault();
                                    //x.IsFolderExists = _fileManager.IsFolderExists(x.Id);
                                    return x;
                                }).ToList();
                            }
                        }
                        //model.ItemTrackersswithPemrissions = _mapper.Map<IEnumerable<ItemTrackerMetaData>, IEnumerable<ItemTrackerwithPermission>>(model.ItemTrackers);
                        if (model.ItemTrackersswithPemrissions == null)
                            model.ItemTrackersswithPemrissions = new List<ItemTrackerwithPermission>();
                        //if (model.ItemTrackersswithPemrissions != null && model.ItemTrackersswithPemrissions.Count() > 0)
                        //{
                        //    model.ItemTrackersswithPemrissions = model.ItemTrackersswithPemrissions.Select(x =>
                        //    {
                        //        x.ItemTrackerPermission = DataCache.ItemTrackerPermissions.Where(y => y.ItemTrackerId == x.Id && y.UserId == _userId && y.IsActive == true).FirstOrDefault();
                        //        //x.IsFolderExists = _fileManager.IsFolderExists(x.Id);
                        //        return x;
                        //    }).ToList();
                        //}
                    }
                }
                

                if (_isFileCachingEnabled)
                {
                    if (loggedInUserRole == AppRole.Admin)
                    {
                        var filesList = DataCache.Files.Where(x => x.IsActive == true && x.DataRoomId == model.DataRoom.Id && x.FolderId == folderid && x.IsArchived == false);                        
                        model.Files = filesList;
                        model.FileswithPermissions = _mapper.Map<IEnumerable<File>, IEnumerable<FilewithPermission>>(model.Files);
                        if (model.FileswithPermissions != null && model.FileswithPermissions.Count() > 0)
                        {
                            foreach (var item in model.FileswithPermissions)
                            {
                                item.FilePermission = new FilePermission { HasFullControl = true, FolderId = item.FolderId,FolderName=item.FolderName,FileId = item.Id,FileName=item.FileName};
                                //item.IsFileExists = _fileManager.IsFileExists(item.Id);
                            }
                        }
                    }
                    else
                    {
                        var filesList = DataCache.Files.Where(x => x.IsActive == true && x.DataRoomId == model.DataRoom.Id && x.FolderId == folderid && x.IsArchived == false);
                        if (filesList != null && filesList.Count() > 0)
                        {
                            List<int> fileIds = new List<int>();
                            var filePermissionDetails = DataCache.FilePermissions.Where(x => x.UserId == _userId);
                            if (filePermissionDetails != null && filePermissionDetails.Count() > 0)
                            {
                                fileIds = filePermissionDetails.Select(x => x.FileId).ToList();
                            }
                            filesList = filesList.Where(x => fileIds.Contains(x.Id));
                        }
                        model.Files = filesList;
                        model.FileswithPermissions = _mapper.Map<IEnumerable<File>, IEnumerable<FilewithPermission>>(model.Files);
                        if (model.FileswithPermissions != null && model.FileswithPermissions.Count() > 0)
                        {
                            foreach (var item in model.FileswithPermissions)
                            {
                                item.FilePermission = DataCache.FilePermissions.Where(x => x.FileId == item.Id && x.UserId == _userId && x.IsActive == true).FirstOrDefault();
                                //item.IsFileExists = _fileManager.IsFileExists(item.Id);
                            }
                        }
                    }
                    
                }
                else
                {
                    FilterModel filter = new FilterModel();
                    filter.CurrentPage = model.CurrentPage;
                    filter.RecordsPerPage = model.RecordsPerPage;
                    filter.SearchString = model.SearchString;
                    filter.Sort = model.SortColumn;
                    filter.TableName = "File";
                    filter.WhereCondition = " IsActive=1 and FolderId = " + folderid;
                    filter.DataRoomId = model.DataRoom.Id;
                    model.Files = await _fileService.GetFiles(filter);
                }

                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    model.FolderswithPemrissions = model.FolderswithPemrissions.Where(x => x.FolderName.ToLower().Contains(model.SearchString.ToLower()));
                    model.FileswithPermissions = model.FileswithPermissions.Where(x => x.FileName.ToLower().Contains(model.SearchString.ToLower()));
                    model.ItemTrackersswithPemrissions = model.ItemTrackersswithPemrissions.Where(x => x.ItemTrackerName.ToLower().Contains(model.SearchString.ToLower()));
                }
                //model.PagedFolders = model.Folders.ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //model.PagedFiles = model.Files.ToPagedList(model.CurrentPage,model.RecordsPerPage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> GetFolderContent(int dataroomid, int parentfolderid)
        {
            try
            {
                ExplorerCustomModel model = new ExplorerCustomModel();
                await GetFoldersandFiles(dataroomid, parentfolderid, true, model);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~/Areas/Explorer/Views/Shared/_explorerview.cshtml", model)
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
        public async Task<JsonResult> GetPagedFoldersandPagedFiles(
            DataRoomContentModel filtermodel)
        {
            ExplorerCustomModel model = new ExplorerCustomModel();
            model.CurrentPage = filtermodel.CurrentPage;
            model.RecordsPerPage = filtermodel.RecordsPerPage;
            model.SearchString = filtermodel.SearchString;
            model.IsFileCachingEnabled = _isFileCachingEnabled;
            if (_isFileCachingEnabled)
            {
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    model.CurrentPage = 1;
                }
            }
            else
            {
                FilterModel filter = new FilterModel();
                filter.CurrentPage = model.CurrentPage;
                filter.RecordsPerPage = model.RecordsPerPage;
                filter.SearchString = model.SearchString;
                filter.Sort = model.SortColumn;
                filter.TableName = "File";
                filter.WhereCondition = " IsActive=1";
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    model.CurrentPage = 1;
                    filter.WhereCondition += " and lower(filename) like '%" + model.SearchString.ToLower() + "%' or lower(foldername) like '%" + model.SearchString.ToLower() + "%'";
                }
                model.Files = await _fileService.GetFiles(filter);
            }
            //filter.DataRoomId = model.DataRoom.Id;
            //GetFoldersandFiles(filtermodel.DataRoomId, filtermodel.FolderId, filtermodel.IsParentFolder, model);

            JsonResult jr = Json(new
            {
                HTML = model
            });
            jr.MaxJsonLength = int.MaxValue;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }

        private string GetDOMTreeView(List<Folder> folders, int dataroomid, string dataroomname)
        {
            string DOMTreeView = "";
            if (folders.Count > 0)
            {
                DOMTreeView += CreateTreeViewMenu(folders, 0, string.Empty);
            }
            return "<ul id='tree2' class='tree' style='margin-left:-17px'>" + "<li class='node' data-id = '" + 0 + "'  data-name = '' data-path= '" + dataroomname + "'>" + dataroomname + "<ul>" + DOMTreeView + "</ul></li></ul>";
        }

        private string CreateTreeViewMenu(List<Folder> folders, int parentfolderid, string parentFolderName)
        {
            string DOMDataList = "";

            int menuNumber = 0;
            string menuName = "";

            var filterFolders = folders.Where(x => x.ParentFolderId == parentfolderid);

            foreach (Folder folder in filterFolders)
            {
                menuNumber = folder.Id;
                menuName = folder.FolderName;
                var path = parentfolderid == 0 ? menuName : parentFolderName + " > " + menuName;
                DOMDataList += "<li  class='node' data-id='" + menuNumber + "'  data-name='" + menuName + "' data-path='" + path + "'>";
                DOMDataList += menuName;

                var childFolders = folders.Where(x => x.ParentFolderId == menuNumber);
                if (childFolders.Count() != 0)
                {
                    DOMDataList += "<ul>";
                    DOMDataList += CreateTreeViewMenu(folders, menuNumber, menuName).Replace
                                   ("<li class='node'>", "<li>");
                    DOMDataList += "</ul></li>";
                }
                else
                {
                    DOMDataList += "</li>";
                }
            }
            return DOMDataList;
        }

        public JsonResult GetFoldersforCopyandMove(int folderid,string flag="Copy")
        {
            try
            {
                ExplorerCustomModel model = new ExplorerCustomModel();
                var dataroompermissions = DataCache.DataRoomPermissions.Where(x => x.UserId == _userId && (x.HasFullControl == true || x.HasWrite == true));
                if(dataroompermissions!=null && dataroompermissions.Count() > 0)
                {
                    List<int> dataroomids = dataroompermissions.Select(x => x.DataRoomId).Distinct().ToList();
                    var datarooms = DataCache.DataRooms.Where(x => dataroomids.Contains(x.Id));
                    if(datarooms!=null && datarooms.Count() > 0)
                    {
                        model.DataRooms = datarooms.ToList();
                    }
                }

                List<int> folderIds = new List<int>();
                var folderPermissionDetails = DataCache.FolderPermissions.Where(x => x.UserId == _userId && (x.HasFullControl == true || x.HasWrite == true));
                if (folderPermissionDetails != null && folderPermissionDetails.Count() > 0)
                {
                    folderIds = folderPermissionDetails.Select(x => x.FolderId).ToList();
                }
                model.Folders = DataCache.Folders.Where(x => folderIds.Contains(x.Id) && x.ParentFolderId == 0);
                if (model.Folders != null && model.Folders.Count() > 0)
                    model.Folders = model.Folders.ToList();

                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~/Areas/Explorer/Views/Shared/_copyandmovetreeview.cshtml", model)
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

        public JsonResult GetFoldersbasedonParentId(int dataroomid,int folderid)
        {
            try
            {
                var folders = DataCache.Folders.Where(x => x.DataRoomId == dataroomid && x.ParentFolderId == folderid && x.IsActive == true);
                if (folders != null && folders.Count() > 0)
                {
                    return Json(folders, JsonRequestBehavior.AllowGet);
                }
                return Json("");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> CopyFile(int targetfolderid,int selectedfileid,int targetDataRoomId)
        {
            int activityid = 0;
            try
            {
                var targetfolder = DataCache.Folders.Where(x => x.Id == targetfolderid);
                var selectedfile = DataCache.Files.Where(x => x.Id == selectedfileid);
                if (targetfolder.Count() > 0 && selectedfile!=null)
                {
                    Folder folder = targetfolder.First();
                    File file = selectedfile.First();
                    File fl = new File();
                    var dataroom = DataCache.DataRooms.First(x=>x.Id == folder.DataRoomId);
                    fl.DataRoomId = folder.DataRoomId;
                    fl.DataRoomName = folder.DataRoomName;
                    fl.FolderId = folder.Id;
                    fl.FolderName = folder.FolderName;
                    fl.FileName = file.FileName;
                    fl.ContentType = file.ContentType;
                    fl.FileSize = file.FileSize;
                    fl.FileVersion = file.FileVersion;
                    fl.RelativePath = folder.RelativePath + "/" + file.FileName; /*FileCopy(file, folder);*/
                    fl.IsActive = true;
                    fl.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    fl.CreatorName = Convert.ToString(Session["UserName"]);
                    fl.CreatedOn = DateTime.Now;
                    fl.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    fl.CompanyName = Convert.ToString(Session["CompanyName"]);
                    fl.IsApproved = null;
                    fl.ApprovedBy = null;
                    fl.ApproverName = null;
                    fl.ApprovedOn = null;
                    fl.Status = null;
                    if(folder.WorkFlowId > 0)
                    {
                        fl.Status = "Submitted";
                        fl.IsWorkFlowRequired = true;
                    }
                    fl.Id = await _fileService.SaveFile(fl);
                    _fileManager.CopyFile(file.RelativePath, folder.RelativePath + "/" + file.FileName);
                    new Thread(() => DataCache.RefreshSingleFile(fl)).Start();
                    await _permissionManager.ApplyCreatorPermissionstoFile(fl);
                    activityid = await _logger.LogActivity(fl.CompanyId,"File Copy", "File - " + fl.FileName + " was copied from " + file.FolderName + " to " + fl.FolderName + " by " + Convert.ToString(Session["Name"]), dataroomid: fl.DataRoomId, dataroomname: fl.DataRoomName, fileid: fl.Id, folderid: fl.FolderId, foldername: fl.FolderName, filename: fl.FileName);
                    await _logger.LogDataDifference(activityid, new File(), fl);
                }
                else if(targetDataRoomId > 0 && targetfolderid == 0)
                {
                    var dataroom = DataCache.DataRooms.First(x => x.Id == targetDataRoomId);
                    File file = selectedfile.First();
                    File fl = new File();
                    fl.DataRoomId = dataroom.Id;
                    fl.DataRoomName = dataroom.DataRoomName;
                    fl.FolderId = 0;
                    fl.FolderName = String.Empty;
                    fl.FileName = file.FileName;
                    fl.ContentType = file.ContentType;
                    fl.FileSize = file.FileSize;
                    fl.FileVersion = file.FileVersion;
                    fl.RelativePath = dataroom.RelativePath + "/" + file.Id; /*FileCopy(file, folder);*/
                    fl.IsActive = true;
                    fl.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    fl.CreatorName = Convert.ToString(Session["UserName"]);
                    fl.CreatedOn = DateTime.Now;
                    fl.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    fl.CompanyName = Convert.ToString(Session["CompanyName"]);
                    fl.IsApproved = null;
                    fl.ApprovedBy = null;
                    fl.ApproverName = null;
                    fl.ApprovedOn = null;
                    fl.Status = null;
                    fl.Id = await _fileService.SaveFile(fl);
                    _fileManager.CopyFile(file.RelativePath, dataroom.RelativePath + "/" + file.Id);
                    new Thread(() => DataCache.RefreshSingleFile(fl)).Start();
                    await _permissionManager.ApplyCreatorPermissionstoFile(fl);
                    activityid = await _logger.LogActivity(fl.CompanyId, "File Copy", "File - " + fl.FileName + " was copied from " + file.FolderName + " to " + (string.IsNullOrEmpty(fl.FolderName) ? fl.DataRoomName : fl.FolderName) + " by " + Convert.ToString(Session["Name"]), dataroomid: fl.DataRoomId, dataroomname: fl.DataRoomName, fileid: fl.Id, folderid: fl.FolderId, foldername: fl.FolderName, filename: fl.FileName);
                    await _logger.LogDataDifference(activityid, new File(), fl);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("Success");
        }

        //public string FileCopy(File file,Folder target)
        //{
        //    string relativePath = target.RelativePath + "/" + file.FileName;
        //    try
        //    {
        //        string sourceFilePath = System.IO.Path.Combine(_workspacepath, file.RelativePath);                
        //        string destinationPath = System.IO.Path.Combine(_workspacepath, relativePath);
        //        if (System.IO.File.Exists(sourceFilePath))
        //        {
        //            if (!System.IO.File.Exists(destinationPath))
        //            {
        //                System.IO.File.Copy(sourceFilePath, destinationPath);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return relativePath;
        //}

        public async Task<JsonResult> MoveFile(int targetfolderid, int selectedfileid, int targetDataRoomId)
        {
            int activityid = 0;
            try
            {
                var targetfolder = DataCache.Folders.Where(x => x.Id == targetfolderid);
                var selectedfile = DataCache.Files.Where(x => x.Id == selectedfileid);
                if (targetfolder.Count() > 0 && selectedfile != null)
                {
                    Folder folder = targetfolder.First();
                    File file = selectedfile.First();
                    File oldfile = file.Clone();
                    var dataroom = DataCache.DataRooms.First(x => x.Id == folder.DataRoomId);
                    file.DataRoomId = folder.DataRoomId;
                    file.DataRoomName = folder.DataRoomName;
                    file.FolderId = folder.Id;
                    file.FolderName = folder.FolderName;
                    file.RelativePath = folder.RelativePath + "/" + file.FileName; 
                    file.IsActive = true;
                    file.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    file.ModifierName = Convert.ToString(Session["UserName"]);
                    file.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    file.CompanyName = Convert.ToString(Session["CompanyName"]);
                    file.ModifiedOn = DateTime.Now;
                    file.IsApproved = null;
                    file.ApprovedBy = null;
                    file.ApproverName = null;
                    file.ApprovedOn = null;
                    file.Status = null;
                    if (folder.WorkFlowId > 0)
                    {
                        file.Status = "Submitted";
                        file.IsWorkFlowRequired = true;
                    }
                    _fileManager.MoveFile(oldfile.RelativePath, folder.RelativePath + "/" + file.FileName);
                    await _fileService.UpdateFile(file);                    
                    new Thread(() => DataCache.RefreshSingleFile(file)).Start();
                    await _permissionManager.ApplyCreatorPermissionstoFile(file);
                    activityid = await _logger.LogActivity(file.CompanyId,"File Move", "File - " + file.FileName + " was copied from " + file.FolderName + " to " + file.FolderName + " by " + Convert.ToString(Session["Name"]), dataroomid: file.DataRoomId, dataroomname: file.DataRoomName, fileid: file.Id, folderid: file.FolderId, foldername: file.FolderName, filename: file.FileName);
                    await _logger.LogDataDifference(activityid, oldfile, file);
                }
                else if (targetDataRoomId > 0 && targetfolderid == 0)
                {
                    File file = selectedfile.First();
                    File oldfile = file.Clone();
                    var dataroom = DataCache.DataRooms.First(x => x.Id == targetDataRoomId);
                    file.DataRoomId = dataroom.Id;
                    file.DataRoomName = dataroom.DataRoomName;
                    file.FolderId = 0;
                    file.FolderName = null;
                    file.RelativePath = dataroom.RelativePath + "/" + file.Id;
                    file.IsActive = true;
                    file.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    file.ModifierName = Convert.ToString(Session["UserName"]);
                    file.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    file.CompanyName = Convert.ToString(Session["CompanyName"]);
                    file.ModifiedOn = DateTime.Now;
                    file.IsApproved = null;
                    file.ApprovedBy = null;
                    file.ApproverName = null;
                    file.ApprovedOn = null;
                    file.Status = null;                    
                    _fileManager.MoveFile(oldfile.RelativePath, dataroom.RelativePath + "/" + file.Id);
                    await _fileService.UpdateFile(file);
                    new Thread(() => DataCache.RefreshSingleFile(file)).Start();
                    await _permissionManager.ApplyCreatorPermissionstoFile(file);
                    activityid = await _logger.LogActivity(file.CompanyId, "File Move", "File - " + file.FileName + " was copied from " + file.FolderName + " to " + file.FolderName + " by " + Convert.ToString(Session["Name"]), dataroomid: file.DataRoomId, dataroomname: file.DataRoomName, fileid: file.Id, folderid: file.FolderId, foldername: file.FolderName, filename: file.FileName);
                    await _logger.LogDataDifference(activityid, oldfile, file);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success");
        }

        public async Task<JsonResult> CopyFolder(int targetdataroomid, int targetfolderid, int selectedfolderid)
        {
            int? activityid = 0;
            try
            {
                var dataroom = DataCache.DataRooms.First(x => x.Id == targetdataroomid);
                var selectedfolder = DataCache.Folders.First(x => x.Id == selectedfolderid);
                int parentfolderid = selectedfolder.Id;

                List<Models.Folder> folderstobecopied = new List<Models.Folder>();
                // Include the parent folder for copying
                folderstobecopied.Add(selectedfolder);
                Action<Models.Folder> SetChildren = null;
                SetChildren = parent =>
                {
                    var subfolders = DataCache.Folders
                        .Where(childItem => childItem.ParentFolderId == parent.Id)
                        .ToList();

                    foreach (var item in subfolders)
                    {
                        // Add Folder to list
                        folderstobecopied.Add(item);
                    }

                    //Recursively call the SetChildren method for each child.
                    subfolders
                        .ForEach(SetChildren);
                };

                //Initialize the hierarchical list to root level items
                List<Models.Folder> hierarchicalItems = DataCache.Folders
                    .Where(rootItem => rootItem.Id == selectedfolder.Id)
                    .ToList();

                //Call the SetChildren method to set the children on each root level item.
                hierarchicalItems.ForEach(SetChildren);

                Models.Folder targetFolder = new Models.Folder();
                if (targetfolderid > 0)
                {
                    targetFolder = DataCache.Folders.First(x => x.Id == targetfolderid);
                }

                // Create mapping 
                List<KeyValuePair<int, int>> mappingFolders = new List<KeyValuePair<int, int>>();
                if (folderstobecopied != null && folderstobecopied.Count() > 0)
                {
                    foreach(var sourcefolder in folderstobecopied.OrderBy(x=>x.ParentFolderId).ToList())
                    {
                        var destFolder = await CopyFolderStructure(dataroom, sourcefolder, targetFolder);
                        mappingFolders.Add(new KeyValuePair<int, int>(sourcefolder.Id, destFolder.Id));
                    }
                }

                // Update Parent Ids
                foreach(var item in mappingFolders)
                {
                    var srcFolder = DataCache.Folders.First(x => x.Id == item.Key);
                    var destFolder = DataCache.Folders.First(x=>x.Id == item.Value);

                    var srcParentId = srcFolder.ParentFolderId;
                    var destParentId = 0;
                    var destParentFolder = new Folder();
                    if (srcParentId > 0 && mappingFolders.Select(x=>x.Key).ToList().Contains(srcParentId))
                    {
                        destParentId = mappingFolders.First(x => x.Key == srcParentId).Value;
                        destParentFolder = DataCache.Folders.First(x => x.Id == destParentId);
                    }
                    else
                    {
                        destParentFolder.Id = targetFolder.Id;
                        destParentFolder.FolderName = targetFolder.FolderName;
                    }
                    destFolder.ParentFolderId = destParentFolder.Id;
                    destFolder.ParentFolderName = destParentFolder.FolderName;

                    
                    destFolder.RelativePath = _fileManager.SaveFoldertoWorkSpace(destFolder.Id.ToString(), destFolder.ParentFolderId,  dataroom.Id);
                    
                    await _folderService.UpdateFolder(destFolder);
                    new Thread(() => DataCache.RefreshSingleFolder(destFolder)).Start();

                    // Copy All Files of Selected Folder to Destination Folder
                    var files = DataCache.Files.Where(x => x.FolderId == srcFolder.Id);
                    if (files != null && files.Count() > 0)
                    {
                        foreach (var file in files.ToList())
                        {
                            File fl = new File();
                            fl.DataRoomId = dataroom.Id;
                            fl.DataRoomName = dataroom.DataRoomName;
                            fl.FolderId = destFolder.Id;
                            fl.FolderName = destFolder.FolderName;
                            fl.FileName = file.FileName;
                            fl.ContentType = file.ContentType;
                            fl.FileSize = file.FileSize;
                            fl.FileVersion = file.FileVersion;
                            fl.RelativePath = destFolder.RelativePath + "/" + file.Id; /*FileCopy(file, folder);*/
                            fl.IsActive = true;
                            fl.CreatedBy = Convert.ToInt32(Session["UserId"]);
                            fl.CreatorName = Convert.ToString(Session["UserName"]);
                            fl.CreatedOn = DateTime.Now;
                            fl.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                            fl.CompanyName = Convert.ToString(Session["CompanyName"]);
                            fl.IsApproved = null;
                            fl.ApprovedBy = null;
                            fl.ApproverName = null;
                            fl.ApprovedOn = null;
                            fl.Status = null;
                            fl.Id = await _fileService.SaveFile(fl);
                            _fileManager.CopyFile(file.RelativePath, fl.RelativePath);
                            new Thread(() => DataCache.RefreshSingleFile(fl)).Start();
                            await _permissionManager.ApplyCreatorPermissionstoFile(fl);
                            activityid = await _logger.LogActivity(fl.CompanyId, "File Copy", "File - " + fl.FileName + " was copied from " + file.FolderName + " to " + fl.FolderName + " by " + Convert.ToString(Session["Name"]), dataroomid: fl.DataRoomId, dataroomname: fl.DataRoomName, fileid: fl.Id, folderid: fl.FolderId, foldername: fl.FolderName, filename: fl.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success");
        }

        public async Task<JsonResult> MoveFolder(int targetdataroomid, int targetfolderid, int selectedfolderid)
        {
            int? activityid = 0;
            try
            {
                var dataroom = DataCache.DataRooms.First(x => x.Id == targetdataroomid);
                var selectedfolder = DataCache.Folders.First(x => x.Id == selectedfolderid);
                int parentfolderid = selectedfolder.Id;

                List<Models.Folder> folderstobecopied = new List<Models.Folder>();
                // Include the parent folder for copying
                folderstobecopied.Add(selectedfolder);
                Action<Models.Folder> SetChildren = null;
                SetChildren = parent =>
                {
                    var subfolders = DataCache.Folders
                        .Where(childItem => childItem.ParentFolderId == parent.Id)
                        .ToList();

                    foreach (var item in subfolders)
                    {
                        // Add Folder to list
                        folderstobecopied.Add(item);
                    }

                    //Recursively call the SetChildren method for each child.
                    subfolders
                        .ForEach(SetChildren);
                };

                //Initialize the hierarchical list to root level items
                List<Models.Folder> hierarchicalItems = DataCache.Folders
                    .Where(rootItem => rootItem.Id == selectedfolder.Id)
                    .ToList();

                //Call the SetChildren method to set the children on each root level item.
                hierarchicalItems.ForEach(SetChildren);

                Models.Folder targetFolder = new Models.Folder();
                if (targetfolderid > 0)
                {
                    targetFolder = DataCache.Folders.First(x => x.Id == targetfolderid);
                }

                // Create mapping 
                List<KeyValuePair<int, int>> mappingFolders = new List<KeyValuePair<int, int>>();
                if (folderstobecopied != null && folderstobecopied.Count() > 0)
                {
                    foreach (var sourcefolder in folderstobecopied.OrderBy(x => x.ParentFolderId).ToList())
                    {
                        var destFolder = await CopyFolderStructure(dataroom, sourcefolder, targetFolder);
                        mappingFolders.Add(new KeyValuePair<int, int>(sourcefolder.Id, destFolder.Id));
                    }
                }

                // Update Parent Ids
                foreach (var item in mappingFolders)
                {
                    var srcFolder = DataCache.Folders.First(x => x.Id == item.Key);
                    var destFolder = DataCache.Folders.First(x => x.Id == item.Value);

                    var srcParentId = srcFolder.ParentFolderId;
                    var destParentId = 0;
                    var destParentFolder = new Folder();
                    if (srcParentId > 0)
                    {
                        if (mappingFolders.Select(x => x.Key).ToList().Contains(srcParentId))
                        {
                            destParentId = mappingFolders.First(x => x.Key == srcParentId).Value;
                        }
                        else
                        {
                            destParentId = targetfolderid;
                        }
                        destParentFolder = DataCache.Folders.First(x => x.Id == destParentId);
                    }
                    destFolder.ParentFolderId = destParentFolder.Id;
                    destFolder.ParentFolderName = destParentFolder.FolderName;


                    destFolder.RelativePath = _fileManager.SaveFoldertoWorkSpace(destFolder.Id.ToString(), destFolder.ParentFolderId, dataroom.Id);

                    await _folderService.UpdateFolder(destFolder);
                    new Thread(() => DataCache.RefreshSingleFolder(destFolder)).Start();

                    // Copy All Files of Selected Folder to Destination Folder
                    var files = DataCache.Files.Where(x => x.FolderId == srcFolder.Id);
                    if (files != null && files.Count() > 0)
                    {
                        foreach (var file in files.ToList())
                        {
                            File fl = new File();
                            fl.DataRoomId = dataroom.Id;
                            fl.DataRoomName = dataroom.DataRoomName;
                            fl.FolderId = destFolder.Id;
                            fl.FolderName = destFolder.FolderName;
                            fl.FileName = file.FileName;
                            fl.ContentType = file.ContentType;
                            fl.FileSize = file.FileSize;
                            fl.FileVersion = file.FileVersion;
                            fl.RelativePath = destFolder.RelativePath + "/" + file.Id; /*FileCopy(file, folder);*/
                            fl.IsActive = true;
                            fl.CreatedBy = Convert.ToInt32(Session["UserId"]);
                            fl.CreatorName = Convert.ToString(Session["UserName"]);
                            fl.CreatedOn = DateTime.Now;
                            fl.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                            fl.CompanyName = Convert.ToString(Session["CompanyName"]);
                            fl.IsApproved = null;
                            fl.ApprovedBy = null;
                            fl.ApproverName = null;
                            fl.ApprovedOn = null;
                            fl.Status = null;
                            fl.Id = await _fileService.SaveFile(fl);
                            _fileManager.MoveFile(file.RelativePath, fl.RelativePath);
                            new Thread(() => DataCache.RefreshSingleFile(fl)).Start();
                            await _permissionManager.ApplyCreatorPermissionstoFile(fl);
                            activityid = await _logger.LogActivity(fl.CompanyId, "File Copy", "File - " + fl.FileName + " was copied from " + file.FolderName + " to " + fl.FolderName + " by " + Convert.ToString(Session["Name"]), dataroomid: fl.DataRoomId, dataroomname: fl.DataRoomName, fileid: fl.Id, folderid: fl.FolderId, foldername: fl.FolderName, filename: fl.FileName);
                        }
                    }
                }

                // Delete Folders after copy
                foreach (var item in mappingFolders)
                {
                    var srcFolder = DataCache.Folders.First(x => x.Id == item.Key);
                    await _folderService.DeleteFolder(srcFolder);
                    _fileManager.DeleteFolderfromWorkSpace(srcFolder.RelativePath);
                    new Thread(() => DataCache.RemoveFolderfromCache(srcFolder)).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success");
        }

        public async Task DownloadFolder(int folderid)
        {
            int? activityid = 0;
            try
            {
                var selectedfolder = DataCache.Folders.First(x => x.Id == folderid);
                var dataroom = DataCache.DataRooms.First(x => x.Id == selectedfolder.DataRoomId);
                int parentfolderid = selectedfolder.Id;
                FileEncryption fileEncryption = new FileEncryption();

                List<Models.Folder> folderstobecopied = new List<Models.Folder>();
                // Include the parent folder for copying
                folderstobecopied.Add(selectedfolder);
                Action<Models.Folder> SetChildren = null;
                SetChildren = parent =>
                {
                    var subfolders = DataCache.Folders
                        .Where(childItem => childItem.ParentFolderId == parent.Id)
                        .ToList();

                    foreach (var item in subfolders)
                    {
                        // Add Folder to list
                        folderstobecopied.Add(item);
                    }

                    //Recursively call the SetChildren method for each child.
                    subfolders
                        .ForEach(SetChildren);
                };

                //Initialize the hierarchical list to root level items
                List<Models.Folder> hierarchicalItems = DataCache.Folders
                    .Where(rootItem => rootItem.Id == selectedfolder.Id)
                    .ToList();

                //Call the SetChildren method to set the children on each root level item.
                hierarchicalItems.ForEach(SetChildren);

                var temppath = System.Web.HttpContext.Current.Server.MapPath("~/Temp/");
                var tempfolderpath = System.IO.Path.Combine(temppath,selectedfolder.Guid);
                if (folderstobecopied != null && folderstobecopied.Count() > 0)
                {
                    foreach (var sourcefolder in folderstobecopied.OrderBy(x => x.ParentFolderId).ToList())
                    {
                        var parentFolder = new Folder();
                        if(sourcefolder.ParentFolderId > 0)
                        {
                            parentFolder = DataCache.Folders.First(x=>x.Id == sourcefolder.ParentFolderId);
                            var pathparts = parentFolder.RelativePath.Split(new string[] { parentFolder.DataRoomName }, StringSplitOptions.None);
                            var parentfolderpath = tempfolderpath + "/" + pathparts[1];
                            if (!System.IO.Directory.Exists(parentfolderpath))
                            {
                                System.IO.Directory.CreateDirectory(parentfolderpath);
                                var files = DataCache.Files.Where(x => x.FolderId == parentFolder.Id);
                                if(files!=null && files.Count() > 0)
                                {
                                    foreach(var file in files.ToList())
                                    {
                                        byte[] filebyteArray = _fileManager.GetFileByteArray(file.RelativePath);
                                        if (filebyteArray != null)
                                        {
                                            string tempfilepath = Server.MapPath("~/Temp/") + file.FileName;
                                            System.IO.File.WriteAllBytes(tempfilepath, filebyteArray);
                                            var decryptedfilepath = Server.MapPath("~/Temp/EncryptionTemp/") + file.FileName;
                                            fileEncryption.DecryptFile(tempfilepath, decryptedfilepath);
                                            if (!System.IO.File.Exists(parentfolderpath + "/" + file.FileName))
                                            {
                                                System.IO.File.Move(decryptedfilepath, parentfolderpath + "/" + file.FileName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        
                        var childpathparts = sourcefolder.RelativePath.Split(new string[] { dataroom.RelativePath }, StringSplitOptions.None);
                        var folderpath = tempfolderpath + "/" + childpathparts[1];
                        if (!System.IO.Directory.Exists(folderpath))
                        {
                            System.IO.Directory.CreateDirectory(folderpath);
                            var files = DataCache.Files.Where(x => x.FolderId == sourcefolder.Id);
                            if (files != null && files.Count() > 0)
                            {
                                foreach (var file in files.ToList())
                                {
                                    byte[] filebyteArray = _fileManager.GetFileByteArray(file.RelativePath);
                                    if (filebyteArray != null)
                                    {
                                        string tempfilepath = Server.MapPath("~/Temp/") + file.FileName;
                                        System.IO.File.WriteAllBytes(tempfilepath, filebyteArray);
                                        var decryptedfilepath = Server.MapPath("~/Temp/EncryptionTemp/") + file.FileName;
                                        fileEncryption.DecryptFile(tempfilepath, decryptedfilepath);
                                        if (!System.IO.File.Exists(folderpath + "/" + file.FileName))
                                        {
                                            System.IO.File.Move(decryptedfilepath, folderpath + "/" + file.FileName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                var zipsourcepath = tempfolderpath + "/" + selectedfolder.Id;
                var zippath = tempfolderpath + "/" + selectedfolder.FolderName + ".zip";
                if (!System.IO.File.Exists(zippath))
                {
                    ZipFile.CreateFromDirectory(zipsourcepath, zippath);
                }
                

                byte[] data = System.IO.File.ReadAllBytes(zippath);
                if (System.IO.File.Exists(zippath))
                {
                    System.IO.File.Delete(zippath);
                }
                if (System.IO.File.Exists(zipsourcepath))
                {
                    System.IO.File.Delete(zipsourcepath);
                }
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition", "attachment;filename=" + selectedfolder.FolderName + ".zip");
                
                response.BinaryWrite(data);
                response.End();
                activityid = await _logger.LogActivity(selectedfolder.CompanyId, "Folder Download", "Folder - " + selectedfolder.FolderName + " has been downloaded from sharbox - " + selectedfolder.DataRoomName, dataroomid: selectedfolder.DataRoomId, dataroomname: selectedfolder.DataRoomName, folderid: selectedfolder.Id, foldername: selectedfolder.FolderName, fileid: 0, filename: "");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<Models.Folder> CopyFolderStructure(Models.DataRoom dataroom, Models.Folder folder, Models.Folder targetFolder)
        {
            int? activityid;
            try
            {
                Models.Folder destFolder = new Models.Folder();
                destFolder.DataRoomId = dataroom.Id;
                destFolder.DataRoomName = dataroom.DataRoomName;
                destFolder.FolderName = folder.FolderName;
                Guid guid = Guid.NewGuid();
                destFolder.Guid = guid.ToString();
                destFolder.FolderDescription = folder.FolderDescription;
                destFolder.IsActive = true;
                destFolder.CreatedBy = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
                destFolder.CreatorName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
                destFolder.CreatedOn = DateTime.Now;
                destFolder.CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
                destFolder.CompanyName = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);
                destFolder.ParentFolderId = targetFolder.Id;
                destFolder.ParentFolderName = targetFolder.FolderName;                
                destFolder.Id = await _folderService.SaveFolder(destFolder);
                destFolder.RelativePath = (string.IsNullOrEmpty(targetFolder.RelativePath) ? dataroom.RelativePath : targetFolder.RelativePath) + "/" + destFolder.Id;
                await _folderService.UpdateFolder(destFolder);
                //_fileManager.CopyFolder(folder.RelativePath, destFolder.RelativePath);
                new Thread(() => DataCache.RefreshSingleFolder(destFolder)).Start();
                await _permissionManager.ApplyCreatorPermissionstoFolder(destFolder);
                activityid = await _logger.LogActivity(destFolder.CompanyId, "Folder Copy", "Folder - " + destFolder.FolderName + " was copied from " + folder.DataRoomName + "/" + folder.FolderName + " to " + targetFolder.DataRoomName + "/" + targetFolder.FolderName + " by " + Convert.ToString(Session["Name"]), dataroomid: dataroom.Id, dataroomname: dataroom.DataRoomName, fileid: 0, folderid: targetFolder.Id, foldername: targetFolder.FolderName, filename: "");

                return destFolder;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //public string FileMove(File file, Folder target)
        //{
        //    string relativePath = target.RelativePath + "/" + file.FileName;
        //    try
        //    {
        //        string sourceFilePath = System.IO.Path.Combine(_workspacepath, file.RelativePath);
        //        string destinationPath = System.IO.Path.Combine(_workspacepath, relativePath);
        //        if (System.IO.File.Exists(sourceFilePath))
        //        {
        //            if (!System.IO.File.Exists(destinationPath))
        //            {
        //                System.IO.File.Move(sourceFilePath, destinationPath);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return relativePath;
        //}

        public ActionResult TestLayout()
        {
            return View();
        }


        public async Task<JsonResult> GetFileVersions(int fileid)
        {
            try
            {
                IEnumerable<FileVersion> fileVersions = await _fileService.GetFileVersions(fileid);
                if(fileVersions!=null && fileVersions.Count() > 0)
                {
                    foreach(var version in fileVersions)
                    {
                        version.IsFileExists = _fileManager.IsFileVersionExists(version.RelativePath);
                    }
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~/Areas/Files/Views/Shared/_fileversionslist.cshtml", fileVersions)
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