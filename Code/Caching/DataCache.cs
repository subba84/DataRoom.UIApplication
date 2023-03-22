using DataRooms.UI.Areas.Explorer.Model;
using DataRooms.UI.Code.Logging;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms
{
    public static class DataCache
    {
        static ObjectCache cache = new MemoryCache("DataRooms");
        static object locker = new object();

        public const string UsersKey = "UsersKey";
        public const string RolesKey = "RolesKey";
        public const string UserRoleMappingKey = "UserRoleMappingKey";
        public const string DataRoomsKey = "DataRoomsKey";
        public const string FoldersKey = "FoldersKey";
        public const string FilesKey = "FilesKey";
        public const string DataRoomPermissionKey = "DataRoomPermissionKey";
        public const string FolderPermissionKey = "FolderPermissionKey";
        public const string FilePermissionKey = "FilePermissionKey";
        public const string CompanyKey = "CompanyKey";
        public const string WorkFlowKey = "WorkFlowKey";
        public const string DataRoomWorkFlowUserKey = "DataRoomWorkFlowUserKey";
        public const string TaskKey = "TaskKey";
        public const string SettingKey = "SettingKey";
        public const string HostInfoKey = "HostInfoKey";
        public const string ModuleKey = "ModuleKey";
        public const string EmailConfigKey = "EmailConfigKey";
        public const string ADConfigKey = "ADConfigKey";
        public const string ItemTrackerControlKey = "ItemTrackerControlKey";
        public const string ItemTrackerDataKey = "ItemTrackerDataKey";
        public const string ItemTrackerMetaDataKey = "ItemTrackerMetaDataKey";
        public const string ItemTrackerPermissionKey = "ItemTrackerPermissionKey";
        public static string IsFileCacheEnabled = ConfigurationManager.AppSettings["IsFileCacheEnabled"];

        private static string logsPath = string.Empty;
        

        public static void Load()
        {
            var c = Companies;
            var l = LogsPath;
            var u = Users;
            var r = Roles;
            var ur = UserRoleMappings;
            var d = DataRooms;
            var dp = DataRoomPermissions;
            var fd = Folders;
            var fop = FolderPermissions;
            var fip = FilePermissions;
            if (IsFileCacheEnabled == "Y")
            {
                var fl = Files;
            }
            var w = WorkFlows;
            var dw = DataRoomWorkFlowUsers;
            var td = ToDoTasks;
            var st = Settings;
            var hi = HostInformation;
            var m = Module;
            var e = EmailConfiguration;
            var ad = ADConfiguration;
            var id = ItemTrackerData;
            var im = ItemTrackerMetaData;
            var ip = ItemTrackerPermissions;
        }

        public static void CreateLogFolder(string logpath)
        {
            LogFolderCreator.LogPathCreation(logpath);
        }

        public static string LogsPath
        {
            get
            {
                logsPath = ConfigurationManager.AppSettings["LogsPath"];
                return logsPath;
            }
        }

        #region EmailConfiguration
        public static List<EmailConfiguration> EmailConfiguration
        {
            get
            {
                var list = cache[EmailConfigKey] as List<EmailConfiguration>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    list = _service.GetEmailConfigurations();
                    Add(HostInfoKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearEmailConfiguration()
        {
            Clear(EmailConfigKey);
        }

        public static void RefreshEmailConfiguration()
        {
            Clear(EmailConfigKey);
            var x = EmailConfiguration;
        }
        #endregion

        #region ADConfiguration
        public static List<ADInfo> ADConfiguration
        {
            get
            {
                var list = cache[ADConfigKey] as List<ADInfo>;
                if (list == null || list.Count == 0)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    list = _service.GetADInfo();
                    Add(ADConfigKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearADConfiguration()
        {
            Clear(ADConfigKey);
        }

        public static void RefreshADConfiguration()
        {
            Clear(ADConfigKey);
            var x = ADConfiguration;
        }
        #endregion

        #region HostInformation
        public static HostDetails HostInformation
        {
            get
            {
                var list = cache[HostInfoKey] as HostDetails;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    list = _service.GetHostInformation();
                    Add(HostInfoKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearHostInformation()
        {
            Clear(HostInfoKey);
        }

        public static void RefreshHostInformation()
        {
            Clear(HostInfoKey);
            var x = HostInformation;
        }
        #endregion

        #region Module
        public static LicenseInfo Module
        {
            get
            {
                var list = cache[ModuleKey] as LicenseInfo;
                if (list == null)
                {
                    try
                    {
                        DataCacheLoadService _service = new DataCacheLoadService();
                        list = _service.GetAvailableModules();
                    }
                    catch(Exception ex)
                    {
                        list = null;
                    }
                    Add(ModuleKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearModule()
        {
            Clear(ModuleKey);
        }

        public static void RefreshModule()
        {
            Clear(ModuleKey);
            var x = Module;
        }
        #endregion

        #region Companies
        public static List<Company> Companies
        {
            get
            {
                var list = cache[CompanyKey] as List<Company>;
                if (list == null || list.Count() == 0)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<Company> companies = _service.GetCompaniesAsync();
                    if (companies == null)
                        companies = new List<Company>();
                    list = companies.ToList();
                    Add(CompanyKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleCompany(Company company)
        {
            try
            {
                List<Company> existedcopanies = DataCache.Companies;
                var existedcompanydetails = existedcopanies.Where(x => x.Id == company.Id);
                if (existedcompanydetails != null && existedcompanydetails.Count() > 0)
                {
                    DataCache.Companies.Remove(existedcompanydetails.First());
                    DataCache.Companies.Add(company);
                }
                else
                {
                    DataCache.Companies.Add(company);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveCompanyfromCache(Company company)
        {
            try
            {
                List<Company> existedcopanies = DataCache.Companies;
                var existedcompanydetails = existedcopanies.Where(x => x.Id == company.Id);
                if (existedcompanydetails != null && existedcompanydetails.Count() > 0)
                {
                    DataCache.Companies.Remove(existedcompanydetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearCompanies()
        {
            Clear(CompanyKey);
        }

        public static void RefreshCompanies()
        {
            Clear(CompanyKey);
            var x = Companies;
        }
        #endregion

        #region Users
        public static List<User> Users
        {
            get
            {
                var list = cache[UsersKey] as List<User>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<User> users =  _service.GetUsersAsync();
                    if (users == null)
                        users = new List<User>();
                    list = users.ToList();
                    Add(UsersKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleUser(User user)
        {
            try
            {
                List<User> existedusers = DataCache.Users;
                var existeduserdetails = existedusers.Where(x => x.Id == user.Id);
                if (existeduserdetails != null && existeduserdetails.Count() > 0)
                {
                    DataCache.Users.Remove(existeduserdetails.First());
                    DataCache.Users.Add(user);
                }
                else
                {
                    DataCache.Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveUserfromCache(User user)
        {
            try
            {
                List<User> existedusers = DataCache.Users;
                var existeduserdetails = existedusers.Where(x => x.Id == user.Id);
                if (existeduserdetails != null && existeduserdetails.Count() > 0)
                {
                    DataCache.Users.Remove(existeduserdetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearUsers()
        {
            Clear(UsersKey);
        }

        public static void RefreshUsers()
        {
            Clear(UsersKey);
            var x = Users;
        }
        #endregion

        #region Roles
        public static List<RoleMaster> Roles
        {
            get
            {
                var list = cache[RolesKey] as List<RoleMaster>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<RoleMaster> roles = _service.GetRolesAsync();
                    if (roles == null)
                        roles = new List<RoleMaster>();
                    list = roles.ToList();
                    Add(RolesKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearRoles()
        {
            Clear(RolesKey);
        }

        public static void RefreshRoles()
        {
            Clear(RolesKey);
            var x = Roles;
        }
        #endregion

        #region UserRoleMappings
        public static List<UserRoleMapping> UserRoleMappings
        {
            get
            {
                var list = cache[UserRoleMappingKey] as List<UserRoleMapping>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<UserRoleMapping> rolemappings = _service.GetUserRoleMappingsAsync();
                    if (rolemappings == null)
                        rolemappings = new List<UserRoleMapping>();
                    list = rolemappings.ToList();
                    Add(UserRoleMappingKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleUserRoleMapping(UserRoleMapping userrolemapping)
        {
            try
            {
                List<UserRoleMapping> existeduserrolemappings = DataCache.UserRoleMappings;
                var existeduserrolemappingdetails = existeduserrolemappings.Where(x => x.Id == userrolemapping.Id);
                if (existeduserrolemappingdetails != null && existeduserrolemappingdetails.Count() > 0)
                {
                    DataCache.UserRoleMappings.Remove(existeduserrolemappingdetails.First());
                    DataCache.UserRoleMappings.Add(userrolemapping);
                }
                else
                {
                    DataCache.UserRoleMappings.Add(userrolemapping);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveUserRoleMappingfromCache(UserRoleMapping userrolemapping)
        {
            try
            {
                List<UserRoleMapping> existeduserrolemappings = DataCache.UserRoleMappings;
                var existeduserrolemappingdetails = existeduserrolemappings.Where(x => x.Id == userrolemapping.Id);
                if (existeduserrolemappingdetails != null && existeduserrolemappingdetails.Count() > 0)
                {
                    DataCache.UserRoleMappings.Remove(existeduserrolemappingdetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearUserRoleMappings()
        {
            Clear(UserRoleMappingKey);
        }

        public static void RefreshUserRoleMappings()
        {
            Clear(UserRoleMappingKey);
            var x = UserRoleMappings;
        }
        #endregion

        #region DataRooms
        public static List<DataRoom> DataRooms
        {
            get
            {
                var list = cache[DataRoomsKey] as List<DataRoom>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<DataRoom> datarooms = _service.GetDataRooms();
                    if (datarooms == null)
                        datarooms = new List<DataRoom>();
                    list = datarooms.ToList();
                    Add(DataRoomsKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearDataRooms()
        {
            Clear(DataRoomsKey);
        }

        public static void RefreshDataRooms()
        {
            Clear(DataRoomsKey);
            var x = DataRooms;
        }

        public static void RefreshSingleDataRoom(DataRoom room)
        {
            try
            {
                List<DataRoom> existedrooms = DataCache.DataRooms;
                var dataroomdetails = existedrooms.Where(x => x.Id == room.Id);
                if (dataroomdetails != null && dataroomdetails.Count() > 0)
                {
                    DataCache.DataRooms.Remove(dataroomdetails.First());
                    DataCache.DataRooms.Add(room);
                }
                else
                {
                    DataCache.DataRooms.Add(room);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveDataRoomfromCache(DataRoom room)
        {
            try
            {
                List<DataRoom> existedrooms = DataCache.DataRooms;
                var dataroomdetails = existedrooms.Where(x => x.Id == room.Id);
                if (dataroomdetails != null && dataroomdetails.Count() > 0)
                {
                    DataCache.DataRooms.Remove(dataroomdetails.First());
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DataRoomPermissions
        public static List<DataRoomPermission> DataRoomPermissions
        {
            get
            {
                var list = cache[DataRoomPermissionKey] as List<DataRoomPermission>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<DataRoomPermission> dataroompermissions = _service.GetDataRoomPermissions();
                    if (dataroompermissions == null)
                        dataroompermissions = new List<DataRoomPermission>();
                    list = dataroompermissions.ToList();
                    Add(DataRoomPermissionKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleDataRoomPermission(DataRoomPermission dataroompermission)
        {
            try
            {
                List<DataRoomPermission> dataroompermissions = DataCache.DataRoomPermissions;
                var dataroompermissiondetails = dataroompermissions.Where(x => x.Id == dataroompermission.Id);
                if (dataroompermissiondetails != null && dataroompermissiondetails.Count() > 0)
                {
                    DataCache.DataRoomPermissions.Remove(dataroompermissiondetails.First());
                    DataCache.DataRoomPermissions.Add(dataroompermission);
                }
                else
                {
                    DataCache.DataRoomPermissions.Add(dataroompermission);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveDataRoomPermissionfromCache(DataRoomPermission dataroompermission)
        {
            try
            {
                List<DataRoomPermission> dataroompermissions = DataCache.DataRoomPermissions;
                var dataroompermissiondetails = dataroompermissions.Where(x => x.Id == dataroompermission.Id);
                if (dataroompermissiondetails != null && dataroompermissiondetails.Count() > 0)
                {
                    DataCache.DataRoomPermissions.Remove(dataroompermissiondetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearDataRoomPermissions()
        {
            Clear(DataRoomPermissionKey);
        }

        public static void RefreshDataRoomPermissions()
        {
            Clear(DataRoomPermissionKey);
            var x = DataRoomPermissions;
        }
        #endregion

        #region Folders
        public static List<Folder> Folders
        {
            get
            {
                var list = cache[FoldersKey] as List<Folder>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<Folder> folders = _service.GetFolders();
                    if (folders == null)
                        folders = new List<Folder>();
                    list = folders.ToList();
                    Add(FoldersKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleFolder(Folder folder)
        {
            try
            {
                List<Folder> folders = DataCache.Folders;
                var folderdetails = folders.Where(x => x.Id == folder.Id);
                if (folderdetails != null && folderdetails.Count() > 0)
                {
                    DataCache.Folders.Remove(folderdetails.First());
                    DataCache.Folders.Add(folder);
                }
                else
                {
                    DataCache.Folders.Add(folder);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveFolderfromCache(Folder folder)
        {
            try
            {
                List<Folder> folders = DataCache.Folders;
                var folderdetails = folders.Where(x => x.Id == folder.Id);
                if (folderdetails != null && folderdetails.Count() > 0)
                {
                    DataCache.Folders.Remove(folderdetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearFolders()
        {
            Clear(FoldersKey);
        }

        public static void RefreshFolders()
        {
            Clear(FoldersKey);
            var x = Folders;
        }
        #endregion

        #region FolderPermissions
        public static List<FolderPermission> FolderPermissions
        {
            get
            {
                var list = cache[FolderPermissionKey] as List<FolderPermission>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<FolderPermission> folderpermissions = _service.GetFolderPermissions();
                    if (folderpermissions == null)
                        folderpermissions = new List<FolderPermission>();
                    list = folderpermissions.ToList();
                    Add(FolderPermissionKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleFolderPermission(FolderPermission folderpermission)
        {
            try
            {
                List<FolderPermission> folderpermissions = DataCache.FolderPermissions;
                var folderpermissiondetails = folderpermissions.Where(x => x.Id == folderpermission.Id);
                if (folderpermissiondetails != null && folderpermissiondetails.Count() > 0)
                {
                    DataCache.FolderPermissions.Remove(folderpermissiondetails.First());
                    DataCache.FolderPermissions.Add(folderpermission);
                }
                else
                {
                    DataCache.FolderPermissions.Add(folderpermission);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemovFolderPermissionfromCache(FolderPermission folderpermission)
        {
            try
            {
                List<FolderPermission> folderpermissions = DataCache.FolderPermissions;
                var folderpermissiondetails = folderpermissions.Where(x => x.Id == folderpermission.Id);
                if (folderpermissiondetails != null && folderpermissiondetails.Count() > 0)
                {
                    DataCache.FolderPermissions.Remove(folderpermissiondetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearFolderPermissions()
        {
            Clear(FolderPermissionKey);
        }

        public static void RefreshFolderPermissions()
        {
            Clear(FolderPermissionKey);
            var x = FolderPermissions;
        }
        #endregion

        #region Files
        public static List<File> Files
        {
            get
            {
                var list = cache[FilesKey] as List<File>;
                if (list == null || list.Count == 0)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    //IEnumerable<File> files = _service.GetFiles().Result;
                    IEnumerable<File> files = _service.GetFilesusingWebClient();
                    if (files == null)
                        files = new List<File>();
                    list = files.ToList();
                    Add(FilesKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearFiles()
        {
            Clear(FilesKey);
        }

        public static void RefreshFiles()
        {
            Clear(FilesKey);
            var x = Files;
        }

        public static void RefreshSingleFile(File file)
        {
            try
            {
                List<File> files = DataCache.Files;
                var filedetails = files.Where(x => x.Id == file.Id);
                if (filedetails != null && filedetails.Count() > 0)
                {
                    DataCache.Files.Remove(filedetails.First());
                    DataCache.Files.Add(file);
                }
                else
                {
                    DataCache.Files.Add(file);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveFilefromCache(File file)
        {
            try
            {
                List<File> files = DataCache.Files;
                var filedetails = files.Where(x => x.Id == file.Id);
                if (filedetails != null && filedetails.Count() > 0)
                {
                    DataCache.Files.Remove(filedetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FilePermissions
        public static List<FilePermission> FilePermissions
        {
            get
            {
                var list = cache[FilePermissionKey] as List<FilePermission>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<FilePermission> filepermissions = _service.GetFilePrmissions();
                    if (filepermissions == null)
                        filepermissions = new List<FilePermission>();
                    list = filepermissions.ToList();
                    Add(FilePermissionKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleFilePermission(FilePermission filepermission)
        {
            try
            {
                List<FilePermission> filepermissions = DataCache.FilePermissions;
                var filepermissiondetails = filepermissions.Where(x => x.Id == filepermission.Id);
                if (filepermissiondetails != null && filepermissiondetails.Count() > 0)
                {
                    DataCache.FilePermissions.Remove(filepermissiondetails.First());
                    DataCache.FilePermissions.Add(filepermission);
                }
                else
                {
                    DataCache.FilePermissions.Add(filepermission);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveFilePermissionfromCache(FilePermission filepermission)
        {
            try
            {
                List<FilePermission> filepermissions = DataCache.FilePermissions;
                var filepermissiondetailss = filepermissions.Where(x => x.Id == filepermission.Id);
                if (filepermissiondetailss != null && filepermissiondetailss.Count() > 0)
                {
                    DataCache.FilePermissions.Remove(filepermissiondetailss.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearFilePermissions()
        {
            Clear(FilePermissionKey);
        }

        public static void RefreshFilePermissions()
        {
            Clear(FilePermissionKey);
            var x = FilePermissions;
        }
        #endregion

        #region WorkFlows
        public static List<WorkFlowMaster> WorkFlows
        {
            get
            {
                var list = cache[WorkFlowKey] as List<WorkFlowMaster>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<WorkFlowMaster> workflows = _service.GetWorkFlowsusingWebClient();
                    if (workflows == null)
                        workflows = new List<WorkFlowMaster>();
                    list = workflows.ToList();
                    Add(WorkFlowKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleWorkFlow(WorkFlowMaster workflow)
        {
            try
            {
                List<WorkFlowMaster> workflows = DataCache.WorkFlows;
                var workflowdetails = workflows.Where(x => x.Id == workflow.Id);
                if (workflowdetails != null && workflowdetails.Count() > 0)
                {
                    DataCache.WorkFlows.Remove(workflowdetails.First());
                    DataCache.WorkFlows.Add(workflow);
                }
                else
                {
                    DataCache.WorkFlows.Add(workflow);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveWorkFlowfromCache(WorkFlowMaster workflow)
        {
            try
            {
                List<WorkFlowMaster> workflows = DataCache.WorkFlows;
                var workflowdetails = workflows.Where(x => x.Id == workflow.Id);
                if (workflowdetails != null && workflowdetails.Count() > 0)
                {
                    DataCache.WorkFlows.Remove(workflowdetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearWorkFlows()
        {
            Clear(WorkFlowKey);
        }

        public static void RefreshWorkFlows()
        {
            Clear(WorkFlowKey);
            var x = WorkFlows;
        }
        #endregion

        #region DataRoomWorkFlowUsers
        public static List<DataRoomWorkFlowUser> DataRoomWorkFlowUsers
        {
            get
            {
                var list = cache[DataRoomWorkFlowUserKey] as List<DataRoomWorkFlowUser>;
                if (list == null || list.Count() == 0)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<DataRoomWorkFlowUser> workflows = _service.GetDataRoomWorkFlowUsersusingWebClient();
                    if (workflows == null)
                        workflows = new List<DataRoomWorkFlowUser>();
                    list = workflows.ToList();
                    Add(DataRoomWorkFlowUserKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleDataRoomWorkFlowUser(DataRoomWorkFlowUser dataRoomWorkFlowUser)
        {
            try
            {
                List<DataRoomWorkFlowUser> workflowusers = DataCache.DataRoomWorkFlowUsers;
                var workflowdetails = workflowusers.Where(x => 
                x.Id == dataRoomWorkFlowUser.Id
                && x.UserId == dataRoomWorkFlowUser.UserId
                && x.IsActive == true
                && x.RoleId == dataRoomWorkFlowUser.RoleId);
                if (workflowdetails != null && workflowdetails.Count() > 0)
                {
                    DataCache.DataRoomWorkFlowUsers.Remove(workflowdetails.First());
                    DataCache.DataRoomWorkFlowUsers.Add(dataRoomWorkFlowUser);
                }
                else
                {
                    DataCache.DataRoomWorkFlowUsers.Add(dataRoomWorkFlowUser);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveDataRoomWorkFlowUserfromCache(DataRoomWorkFlowUser dataRoomWorkFlowUser)
        {
            try
            {
                List<DataRoomWorkFlowUser> workflowusers = DataCache.DataRoomWorkFlowUsers;
                var workflowdetails = workflowusers.Where(x => x.Id == dataRoomWorkFlowUser.Id
                && x.UserId == dataRoomWorkFlowUser.UserId
                && x.IsActive == true
                && x.RoleId == dataRoomWorkFlowUser.RoleId);
                if (workflowdetails != null && workflowdetails.Count() > 0)
                {
                    DataCache.DataRoomWorkFlowUsers.Remove(workflowdetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearDataRoomWorkFlowUsers()
        {
            Clear(DataRoomWorkFlowUserKey);
        }

        public static void RefreshDataRoomWorkFlowUsers()
        {
            Clear(DataRoomWorkFlowUserKey);
            var x = DataRoomWorkFlowUsers;
        }
        #endregion

        #region ToDoTasks
        public static List<ToDoTask> ToDoTasks
        {
            get
            {
                var list = cache[TaskKey] as List<ToDoTask>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<ToDoTask> todotasks = _service.GetToDoTasksusingWebClient();
                    if (todotasks == null)
                        todotasks = new List<ToDoTask>();
                    list = todotasks.ToList();
                    Add(TaskKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleToDoTask(ToDoTask toDoTask)
        {
            try
            {
                List<ToDoTask> todotasks = DataCache.ToDoTasks;
                var todotaskdetails = todotasks.Where(x => x.Id == toDoTask.Id);
                if (todotaskdetails != null && todotaskdetails.Count() > 0)
                {
                    DataCache.ToDoTasks.Remove(todotaskdetails.First());
                    DataCache.ToDoTasks.Add(toDoTask);
                }
                else
                {
                    DataCache.ToDoTasks.Add(toDoTask);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveToDoTaskfromCache(ToDoTask toDoTask)
        {
            try
            {
                List<ToDoTask> todotasks = DataCache.ToDoTasks;
                var todotaskdetails = todotasks.Where(x => x.Id == toDoTask.Id);
                if (todotaskdetails != null && todotaskdetails.Count() > 0)
                {
                    DataCache.ToDoTasks.Remove(todotaskdetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearToDoTasks()
        {
            Clear(TaskKey);
        }

        public static void RefreshToDoTasks()
        {
            Clear(TaskKey);
            var x = ToDoTasks;
        }
        #endregion

        #region Settings
        public static Setting Settings
        {
            get
            {
                var list = cache[SettingKey] as Setting;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    Setting setting = _service.GetSettingusingWebClient();
                    if (setting == null)
                        setting = new Setting();
                    list = setting;
                    Add(SettingKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearSettings()
        {
            Clear(SettingKey);
        }

        public static void RefreshSettings()
        {
            Clear(SettingKey);
            var x = Settings;
        }
        #endregion

        #region ItemTrackerControls
        public static List<ItemTrackerControl> ItemTrackerControls
        {
            get
            {
                var list = cache[ItemTrackerControlKey] as List<ItemTrackerControl>;
                if (list == null)
                {
                    try
                    {
                        DataCacheLoadService _service = new DataCacheLoadService();
                        list = _service.GetItemTrackerControls();
                    }
                    catch (Exception ex)
                    {
                        list = null;
                    }
                    Add(ItemTrackerControlKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearItemTrackerControls()
        {
            Clear(ItemTrackerControlKey);
        }

        public static void RefreshItemTrackerControls()
        {
            Clear(ItemTrackerControlKey);
            var x = ItemTrackerControls;
        }

        public static void RefreshSingleItemTrackerControl(ItemTrackerControl itemTrackerControl)
        {
            try
            {
                List<ItemTrackerControl> itemTrackerControls = DataCache.ItemTrackerControls;
                var itemTrackerControlDetails = itemTrackerControls.Where(x => x.Id == itemTrackerControl.Id);
                if (itemTrackerControlDetails != null && itemTrackerControlDetails.Count() > 0)
                {
                    DataCache.ItemTrackerControls.Remove(itemTrackerControlDetails.First());
                    DataCache.ItemTrackerControls.Add(itemTrackerControl);
                }
                else
                {
                    DataCache.ItemTrackerControls.Add(itemTrackerControl);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveItemTrackerControlfromCache(ItemTrackerControl itemTrackerControl)
        {
            try
            {
                List<ItemTrackerControl> itemTrackerControls = DataCache.ItemTrackerControls;
                var itemTrackerControlDetails = itemTrackerControls.Where(x => x.Id == itemTrackerControl.Id);
                if (itemTrackerControlDetails != null && itemTrackerControlDetails.Count() > 0)
                {
                    DataCache.ItemTrackerControls.Remove(itemTrackerControlDetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ItemTrackerData
        public static List<ItemTrackerData> ItemTrackerData
        {
            get
            {
                var list = cache[ItemTrackerDataKey] as List<ItemTrackerData>;
                if (list == null)
                {
                    try
                    {
                        DataCacheLoadService _service = new DataCacheLoadService();
                        list = _service.GetItemTrackerData();
                    }
                    catch (Exception ex)
                    {
                        list = null;
                    }
                    Add(ItemTrackerDataKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearItemTrackerData()
        {
            Clear(ItemTrackerDataKey);
        }

        public static void RefreshItemTrackerData()
        {
            Clear(ItemTrackerDataKey);
            var x = ItemTrackerData;
        }

        public static void RefreshSingleItemTrackerData(ItemTrackerData itemTrackerData)
        {
            try
            {
                List<ItemTrackerData> itemTrackerDatas = DataCache.ItemTrackerData;
                var itemTrackerDataDetails = itemTrackerDatas.Where(x => x.Id == itemTrackerData.Id);
                if (itemTrackerDataDetails != null && itemTrackerDataDetails.Count() > 0)
                {
                    DataCache.ItemTrackerData.Remove(itemTrackerDataDetails.First());
                    DataCache.ItemTrackerData.Add(itemTrackerData);
                }
                else
                {
                    DataCache.ItemTrackerData.Add(itemTrackerData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveItemTrackerDatafromCache(ItemTrackerData itemTrackerData)
        {
            try
            {
                List<ItemTrackerData> itemTrackerDatas = DataCache.ItemTrackerData;
                var itemTrackerDataDetails = itemTrackerDatas.Where(x => x.Id == itemTrackerData.Id);
                if (itemTrackerDataDetails != null && itemTrackerDataDetails.Count() > 0)
                {
                    DataCache.ItemTrackerData.Remove(itemTrackerDataDetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ItemTrackerMetaData
        public static List<ItemTrackerMetaData> ItemTrackerMetaData
        {
            get
            {
                var list = cache[ItemTrackerMetaDataKey] as List<ItemTrackerMetaData>;
                if (list == null)
                {
                    try
                    {
                        DataCacheLoadService _service = new DataCacheLoadService();
                        list = _service.GetItemTrackerMetaData();
                    }
                    catch (Exception ex)
                    {
                        list = null;
                    }
                    Add(ItemTrackerMetaDataKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void ClearItemTrackerMetaData()
        {
            Clear(ItemTrackerMetaDataKey);
        }

        public static void RefreshItemTrackerMetaData()
        {
            Clear(ItemTrackerMetaDataKey);
            var x = ItemTrackerMetaData;
        }

        public static void RefreshSingleItemTrackerMetaData(ItemTrackerMetaData itemTrackerMetaData)
        {
            try
            {
                List<ItemTrackerMetaData> itemTrackerMetaDatas = DataCache.ItemTrackerMetaData;
                var itemTrackerMetaDataDetails = itemTrackerMetaDatas.Where(x => x.Id == itemTrackerMetaData.Id);
                if (itemTrackerMetaDataDetails != null && itemTrackerMetaDataDetails.Count() > 0)
                {
                    DataCache.ItemTrackerMetaData.Remove(itemTrackerMetaDataDetails.First());
                    DataCache.ItemTrackerMetaData.Add(itemTrackerMetaData);
                }
                else
                {
                    DataCache.ItemTrackerMetaData.Add(itemTrackerMetaData);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemoveItemTrackerMetaDatafromCache(ItemTrackerMetaData itemTrackerMetaData)
        {
            try
            {
                List<ItemTrackerMetaData> itemTrackerMetaDatas = DataCache.ItemTrackerMetaData;
                var itemTrackerMetaDataDetails = itemTrackerMetaDatas.Where(x => x.Id == itemTrackerMetaData.Id);
                if (itemTrackerMetaDataDetails != null && itemTrackerMetaDataDetails.Count() > 0)
                {
                    DataCache.ItemTrackerMetaData.Remove(itemTrackerMetaDataDetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ItemTrackerPermissions
        public static List<ItemTrackerPermission> ItemTrackerPermissions
        {
            get
            {
                var list = cache[ItemTrackerPermissionKey] as List<ItemTrackerPermission>;
                if (list == null)
                {
                    DataCacheLoadService _service = new DataCacheLoadService();
                    IEnumerable<ItemTrackerPermission> folderpermissions = _service.GetItemTrackerPermissions();
                    if (folderpermissions == null)
                        folderpermissions = new List<ItemTrackerPermission>();
                    list = folderpermissions.ToList();
                    Add(ItemTrackerPermissionKey, list, DateTime.Now.AddHours(24));
                }
                return list;
            }
        }

        public static void RefreshSingleItemTrackerPermission(ItemTrackerPermission itemTrackerPermission)
        {
            try
            {
                List<ItemTrackerPermission> itemtrackerpermissions = DataCache.ItemTrackerPermissions;
                var itemtrackerpermissiondetails = itemtrackerpermissions.Where(x => x.Id == itemTrackerPermission.Id);
                if (itemtrackerpermissiondetails != null && itemtrackerpermissiondetails.Count() > 0)
                {
                    DataCache.ItemTrackerPermissions.Remove(itemtrackerpermissiondetails.First());
                    DataCache.ItemTrackerPermissions.Add(itemTrackerPermission);
                }
                else
                {
                    DataCache.ItemTrackerPermissions.Add(itemTrackerPermission);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void RemovItemTrackerPermissionfromCache(ItemTrackerPermission itemTrackerPermission)
        {
            try
            {
                List<ItemTrackerPermission> itemtrackerpermissions = DataCache.ItemTrackerPermissions;
                var itemtrackerpermissiondetails = itemtrackerpermissions.Where(x => x.Id == itemTrackerPermission.Id);
                if (itemtrackerpermissiondetails != null && itemtrackerpermissiondetails.Count() > 0)
                {
                    DataCache.ItemTrackerPermissions.Remove(itemtrackerpermissiondetails.First());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ClearItemTrackerPermissions()
        {
            Clear(ItemTrackerPermissionKey);
        }

        public static void RefreshItemTrackerPermissions()
        {
            Clear(ItemTrackerPermissionKey);
            var x = ItemTrackerPermissions;
        }
        #endregion

        public static void RefreshCache()
        {
            try
            {
                DataCache.RefreshRoles();
                DataCache.RefreshUsers();
                DataCache.RefreshDataRooms();
                DataCache.RefreshDataRoomPermissions();
                DataCache.RefreshUserRoleMappings();
                DataCache.RefreshFolders();
                DataCache.RefreshCompanies();
                DataCache.RefreshWorkFlows();
                DataCache.RefreshDataRoomWorkFlowUsers();
                if (IsFileCacheEnabled == "Y")
                {
                    DataCache.RefreshFiles();
                }
                DataCache.RefreshItemTrackerControls();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        static void Add(string key, object value, DateTimeOffset expiration, CacheItemPriority priority = CacheItemPriority.Default)
        {
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = expiration;
            policy.Priority = priority;
            var item = new CacheItem(key, value);
            cache.Add(item, policy);
        }
        static void Clear(string key)
        {
            cache.Remove(key);
        }


    }
}