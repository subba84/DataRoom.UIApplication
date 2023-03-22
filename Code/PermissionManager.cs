using DataRooms.UI.Areas.Explorer.Model;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.Code
{
    public class PermissionManager
    {
        public string _token = string.Empty;
        public DataRoomPermissionService _dataRoomPermissionService;
        public FolderPermissionService _folderPermissionService;
        public FilePermissionService _filePermissionService;
        public FolderService _folderService;
        public ItemTrackerService _itemTrackerService;
        int companyId = 0;
        string companyName = "";
        int loggedInUser = 0;
        string loggedInUserName = "";
        public PermissionManager(string token)
        {
            _token = token;
            _dataRoomPermissionService = new DataRoomPermissionService(_token);
            _folderPermissionService = new FolderPermissionService(_token);
            _filePermissionService = new FilePermissionService(_token);
            _itemTrackerService = new ItemTrackerService(_token);
            _folderService = new FolderService(_token);
            companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            companyName = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);
            loggedInUser = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
            loggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        }

        public async Task ApplyDataRoomPermissionstoFolder(Folder folder)
        {
            try
            {
                var dataroompermission = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == folder.DataRoomId);
                if (dataroompermission != null && dataroompermission.Count() > 0)
                {
                    List<FolderPermission> folderPermissions = new List<FolderPermission>();
                    foreach (var permission in dataroompermission)
                    {
                        if (permission.UserId != loggedInUser)
                        {
                            FolderPermission folderPermission = new FolderPermission();
                            folderPermission.DataRoomId = permission.DataRoomId;
                            folderPermission.DataRoomName = permission.DataRoomName;
                            folderPermission.FolderId = folder.Id;
                            folderPermission.FolderName = folder.FolderName;
                            folderPermission.UserId = permission.UserId;
                            folderPermission.UserName = permission.UserName;
                            folderPermission.HasFullControl = permission.HasFullControl;
                            folderPermission.HasRead = permission.HasRead;
                            folderPermission.HasWrite = permission.HasWrite;
                            folderPermission.HasDelete = permission.HasDelete;
                            folderPermission.IsActive = true;
                            folderPermission.CreatedBy = permission.CreatedBy;
                            folderPermission.CreatedOn = DateTime.Now;
                            folderPermission.CompanyId = companyId;
                            folderPermission.CompanyName = companyName;
                            folderPermission.CreatedOn = DateTime.Now;
                            folderPermissions.Add(folderPermission);
                        }
                    }
                    await _folderPermissionService.SaveFolderPermissions(folderPermissions);
                    new Thread(() => DataCache.RefreshFolderPermissions()).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyCreatorPermissionstoFolder(Folder folder)
        {
            try
            {
                var folderpermissions = DataCache.FolderPermissions.Where(x => x.UserId == loggedInUser && x.IsActive == true && x.HasFullControl == true && x.FolderId == folder.Id);
                if (folderpermissions == null || folderpermissions.Count() == 0)
                {
                    List<FolderPermission> folderPermissions = new List<FolderPermission>();
                    FolderPermission folderPermission = new FolderPermission();
                    folderPermission.DataRoomId = folder.DataRoomId;
                    folderPermission.DataRoomName = folder.DataRoomName;
                    folderPermission.FolderId = folder.Id;
                    folderPermission.FolderName = folder.FolderName;
                    folderPermission.UserId = loggedInUser;
                    folderPermission.UserName = loggedInUserName;
                    folderPermission.HasFullControl = true;
                    folderPermission.IsActive = true;
                    folderPermission.CreatedBy = loggedInUser;
                    folderPermission.CreatorName = loggedInUserName;
                    folderPermission.CompanyId = companyId;
                    folderPermission.CompanyName = companyName;
                    folderPermission.CreatedOn = DateTime.Now;
                    folderPermissions.Add(folderPermission);
                    await _folderPermissionService.SaveFolderPermissions(folderPermissions);
                    new Thread(() => DataCache.RefreshFolderPermissions()).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyDataRoomPermissionstoFile(File file)
        {
            try
            {
                var dataroompermission = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == file.DataRoomId);
                if (dataroompermission != null && dataroompermission.Count() > 0)
                {
                    List<FilePermission> filePermissions = new List<FilePermission>();
                    foreach (var permission in dataroompermission)
                    {
                        if (permission.UserId != loggedInUser)
                        {
                            FilePermission filePermission = new FilePermission();
                            filePermission.DataRoomId = permission.DataRoomId;
                            filePermission.DataRoomName = permission.DataRoomName;
                            filePermission.FolderId = file.FolderId;
                            filePermission.FolderName = file.FolderName;
                            filePermission.FileId = file.Id;
                            filePermission.FileName = file.FileName;
                            filePermission.UserId = permission.UserId;
                            filePermission.UserName = permission.UserName;
                            filePermission.HasFullControl = permission.HasFullControl;
                            filePermission.HasRead = permission.HasRead;
                            filePermission.HasWrite = permission.HasWrite;
                            filePermission.HasDelete = permission.HasDelete;
                            filePermission.IsActive = true;
                            filePermission.CompanyId = companyId;
                            filePermission.CompanyName = companyName;
                            filePermission.CreatedBy = permission.CreatedBy;
                            filePermission.CreatedOn = DateTime.Now;
                            filePermissions.Add(filePermission);
                        }
                    }
                    await _filePermissionService.SaveFilePermissions(filePermissions);
                    new Thread(() => DataCache.RefreshFilePermissions()).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyFolderPermissionstoFile(File file)
        {
            try
            {
                var folderpermission = DataCache.FolderPermissions.Where(x => x.FolderId == file.FolderId && x.DataRoomId == file.DataRoomId);
                if (folderpermission != null && folderpermission.Count() > 0)
                {
                    List<FilePermission> filePermissions = new List<FilePermission>();
                    foreach (var permission in folderpermission)
                    {
                        if (permission.UserId != loggedInUser)
                        {
                            FilePermission filePermission = new FilePermission();
                            filePermission.DataRoomId = permission.DataRoomId;
                            filePermission.DataRoomName = permission.DataRoomName;
                            filePermission.FolderId = file.FolderId;
                            filePermission.FolderName = file.FolderName;
                            filePermission.FileId = file.Id;
                            filePermission.FileName = file.FileName;
                            filePermission.UserId = permission.UserId;
                            filePermission.UserName = permission.UserName;
                            filePermission.HasFullControl = permission.HasFullControl;
                            filePermission.HasRead = permission.HasRead;
                            filePermission.HasWrite = permission.HasWrite;
                            filePermission.HasDelete = permission.HasDelete;
                            filePermission.IsActive = true;
                            filePermission.CreatedBy = permission.CreatedBy;
                            filePermission.CreatedOn = DateTime.Now;
                            filePermission.CompanyId = companyId;
                            filePermission.CompanyName = companyName;
                            filePermissions.Add(filePermission);
                        }

                    }
                    await _filePermissionService.SaveFilePermissions(filePermissions);
                    new Thread(() => DataCache.RefreshFilePermissions()).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyCreatorPermissionstoFile(File file)
        {
            try
            {
                var filepermissions = DataCache.FilePermissions.Where(x => x.FileId == file.Id && x.UserId == loggedInUser && x.HasFullControl == true && x.IsActive == true);
                if (filepermissions == null || filepermissions.Count() == 0)
                {
                    List<FilePermission> filePermissions = new List<FilePermission>();
                    FilePermission filePermission = new FilePermission();
                    filePermission.DataRoomId = file.DataRoomId;
                    filePermission.DataRoomName = file.DataRoomName;
                    filePermission.FolderId = file.FolderId;
                    filePermission.FolderName = file.FolderName;
                    filePermission.FileId = file.Id;
                    filePermission.FileName = file.FileName;
                    filePermission.UserId = loggedInUser;
                    filePermission.UserName = loggedInUserName;
                    filePermission.HasFullControl = true;
                    filePermission.IsActive = true;
                    filePermission.CreatedBy = loggedInUser;
                    filePermission.CreatorName = loggedInUserName;
                    filePermission.CreatedOn = DateTime.Now;
                    filePermission.CompanyId = companyId;
                    filePermission.CompanyName = companyName;
                    filePermissions.Add(filePermission);
                    await _filePermissionService.SaveFilePermissions(filePermissions);
                    new Thread(() => DataCache.RefreshFilePermissions()).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public async Task ManageDataRoomPermission(DataRooms.UI.Models.DataRoom dataRoom)
        {
            try
            {
                DataRoomPermission dataRoomPermission = new DataRoomPermission();
                dataRoomPermission.DataRoomId = dataRoom.Id;
                dataRoomPermission.DataRoomName = dataRoom.DataRoomName;
                dataRoomPermission.UserId = loggedInUser;
                dataRoomPermission.UserName = loggedInUserName;
                dataRoomPermission.IsActive = true;
                dataRoomPermission.CreatedBy = loggedInUser;
                dataRoomPermission.CreatorName = loggedInUserName;
                dataRoomPermission.CompanyId = companyId;
                dataRoomPermission.CompanyName = companyName;
                dataRoomPermission.CreatedOn = DateTime.Now;
                dataRoomPermission.HasFullControl = true;
                dataRoomPermission.HasWrite = true;
                dataRoomPermission.HasRead = true;
                dataRoomPermission.HasDelete = true;
                dataRoomPermission.Id = await _dataRoomPermissionService.SaveDataRoomPermission(dataRoomPermission);
                new Thread(() => DataCache.RefreshSingleDataRoomPermission(dataRoomPermission)).Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyPermissionstoFolderHierarchy(int folderid,int userid,string username)
        {
            try
            {
                var folders = await _folderService.GetFolderHierarchy(folderid);
                if(folders!=null && folders.Count() > 0)
                {
                    var dataroomid = folders.Select(x => x.DataRoomId).First();
                    var dataroom = DataCache.DataRooms.First(x=>x.Id == dataroomid);
                    var dataroompermissions = DataCache.DataRoomPermissions.Where(x => x.UserId == userid && x.DataRoomId == dataroomid && x.IsActive == true && (x.HasFullControl == true || x.HasRead == true));
                    if(dataroompermissions == null || dataroompermissions.Count() == 0)
                    {
                        DataRoomPermission dataroompermission = new DataRoomPermission();
                        dataroompermission.DataRoomId = dataroom.Id;
                        dataroompermission.DataRoomName = dataroom.DataRoomName;
                        dataroompermission.UserId = userid;
                        dataroompermission.UserName = username;
                        dataroompermission.HasRead = true;
                        dataroompermission.IsActive = true;
                        dataroompermission.CompanyId = dataroom.CompanyId;
                        dataroompermission.CompanyName = dataroom.CompanyName;
                        dataroompermission.CreatedBy = loggedInUser;
                        dataroompermission.CreatorName = loggedInUserName;
                        dataroompermission.CreatedOn = DateTime.Now;
                        dataroompermission.Id = await _dataRoomPermissionService.SaveDataRoomPermission(dataroompermission);
                        new Thread(() => DataCache.RefreshSingleDataRoomPermission(dataroompermission)).Start();
                    }

                    foreach(var folder in folders.ToList())
                    {
                        var folderpermissions = DataCache.FolderPermissions.Where(x => x.UserId == userid && x.FolderId == folder.Id && x.IsActive == true && (x.HasFullControl == true || x.HasRead == true));
                        if (folderpermissions == null || folderpermissions.Count() == 0)
                        {
                            FolderPermission folderpermission = new FolderPermission();
                            folderpermission.DataRoomId = dataroom.Id;
                            folderpermission.DataRoomName = dataroom.DataRoomName;
                            folderpermission.FolderId = folder.Id;
                            folderpermission.FolderName = folder.FolderName;
                            folderpermission.UserId = userid;
                            folderpermission.UserName = username;
                            folderpermission.HasRead = true;
                            folderpermission.IsActive = true;
                            folderpermission.CompanyId = dataroom.CompanyId;
                            folderpermission.CompanyName = dataroom.CompanyName;
                            folderpermission.CreatedBy = loggedInUser;
                            folderpermission.CreatorName = loggedInUserName;
                            folderpermission.CreatedOn = DateTime.Now;
                            folderpermission.Id = await _folderPermissionService.SaveFolderPermission(folderpermission);
                            new Thread(() => DataCache.RefreshSingleFolderPermission(folderpermission)).Start();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyCreatorPermissionstoItemTracker(ItemTrackerPermission itemTrackerPermission)
        {
            try
            {
                var itemtrackerpermissions = DataCache.ItemTrackerPermissions.Where(x => x.UserId == loggedInUser && x.IsActive == true && x.HasFullControl == true && x.FolderId == itemTrackerPermission.FolderId);
                if (itemtrackerpermissions == null || itemtrackerpermissions.Count() == 0)
                {
                    List<ItemTrackerPermission> itemTrackerPermissions = new List<ItemTrackerPermission>();
                    ItemTrackerPermission permission = new ItemTrackerPermission();
                    permission.DataRoomId = itemTrackerPermission.DataRoomId;
                    permission.DataRoomName = itemTrackerPermission.DataRoomName;
                    permission.FolderId = itemTrackerPermission.FolderId;
                    permission.FolderName = itemTrackerPermission.FolderName;
                    permission.UserId = loggedInUser;
                    permission.UserName = loggedInUserName;
                    permission.HasFullControl = true;
                    permission.IsActive = true;
                    permission.CreatedBy = loggedInUser;
                    permission.CreatorName = loggedInUserName;
                    //permission.CompanyId = companyId;
                    //permission.CompanyName = companyName;
                    permission.CreatedOn = DateTime.Now;
                    itemTrackerPermissions.Add(permission);
                    await _itemTrackerService.SaveItemTrackerPermission(permission);
                    new Thread(() => DataCache.RefreshItemTrackerPermissions()).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyDataRoomPermissionstoItemTracker(ItemTrackerMetaData itemTrackerMetaData)
        {
            try
            {
                var dataroompermission = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == itemTrackerMetaData.DataRoomId);
                if (dataroompermission != null && dataroompermission.Count() > 0)
                {
                    List<ItemTrackerPermission> itemTrackerPermissions = new List<ItemTrackerPermission>();
                    foreach (var permission in dataroompermission)
                    {
                        if (permission.UserId != loggedInUser)
                        {
                            ItemTrackerPermission itemTrackerPermission = new ItemTrackerPermission();
                            itemTrackerPermission.DataRoomId = permission.DataRoomId;
                            itemTrackerPermission.DataRoomName = permission.DataRoomName;
                            itemTrackerPermission.FolderId = itemTrackerMetaData.FolderId;
                            itemTrackerPermission.FolderName = itemTrackerMetaData.FolderName;
                            itemTrackerPermission.ItemTrackerId = itemTrackerMetaData.Id;
                            itemTrackerPermission.ItemTrackerName = itemTrackerMetaData.ItemTrackerName;
                            itemTrackerPermission.UserId = permission.UserId;
                            itemTrackerPermission.UserName = permission.UserName;
                            itemTrackerPermission.HasFullControl = permission.HasFullControl;
                            itemTrackerPermission.HasRead = permission.HasRead;
                            itemTrackerPermission.HasWrite = permission.HasWrite;
                            itemTrackerPermission.HasDelete = permission.HasDelete;
                            itemTrackerPermission.IsActive = true;
                            //itemTrackerPermission.CompanyId = companyId;
                            //itemTrackerPermission.CompanyName = companyName;
                            itemTrackerPermission.CreatedBy = permission.CreatedBy;
                            itemTrackerPermission.CreatedOn = DateTime.Now;
                            itemTrackerPermissions.Add(itemTrackerPermission);
                            await _itemTrackerService.SaveItemTrackerPermission(itemTrackerPermission);
                        }
                    }
                    new Thread(() => DataCache.RefreshFilePermissions()).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyFolderPermissionstoItemTracker(ItemTrackerMetaData itemTrackerMetaData)
        {
            try
            {
                var folderpermission = DataCache.FolderPermissions.Where(x => x.FolderId == itemTrackerMetaData.FolderId);
                if (folderpermission != null && folderpermission.Count() > 0)
                {
                    List<ItemTrackerPermission> itemTrackerPermissions = new List<ItemTrackerPermission>();
                    foreach (var permission in folderpermission)
                    {
                        if (permission.UserId != loggedInUser)
                        {                           
                            ItemTrackerPermission itemTrackerPermission = new ItemTrackerPermission();
                            itemTrackerPermission.DataRoomId = permission.DataRoomId;
                            itemTrackerPermission.DataRoomName = permission.DataRoomName;
                            itemTrackerPermission.FolderId = itemTrackerMetaData.FolderId;
                            itemTrackerPermission.FolderName = itemTrackerMetaData.FolderName;
                            itemTrackerPermission.ItemTrackerId = itemTrackerMetaData.Id;
                            itemTrackerPermission.ItemTrackerName = itemTrackerMetaData.ItemTrackerName;
                            itemTrackerPermission.UserId = permission.UserId;
                            itemTrackerPermission.UserName = permission.UserName;
                            itemTrackerPermission.HasFullControl = permission.HasFullControl;
                            itemTrackerPermission.HasRead = permission.HasRead;
                            itemTrackerPermission.HasWrite = permission.HasWrite;
                            itemTrackerPermission.HasDelete = permission.HasDelete;
                            itemTrackerPermission.IsActive = true;
                            //itemTrackerPermission.CompanyId = companyId;
                            //itemTrackerPermission.CompanyName = companyName;
                            itemTrackerPermission.CreatedBy = permission.CreatedBy;
                            itemTrackerPermission.CreatedOn = DateTime.Now;
                            itemTrackerPermissions.Add(itemTrackerPermission);
                            await _itemTrackerService.SaveItemTrackerPermission(itemTrackerPermission);
                        }
                    }
                    new Thread(() => DataCache.RefreshFilePermissions()).Start();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}