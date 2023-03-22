using Amazon.S3.IO;
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

namespace DataRooms.UI.Code
{
    public class FileManager
    {
        public string _token;
        public string _workspacepath = string.Empty;
        public DataRoomService _dataroomService;
        public LogService _logger;
        public FileEncryption encryption;
        public IFileStorageManager _storageManager;
        //private S3Helper _s3Helper;
        public FileManager(string token,int companyId)
        {
            _token = token;
            //_workspacepath = DataCache.Settings.StoragePath;
            _dataroomService = new DataRoomService(_token);
            _logger = new LogService(token);
            encryption = new FileEncryption();
            //_s3Helper = new S3Helper();
            if(companyId > 0)
            {
                var company = DataCache.Companies.Single(x => x.Id == companyId);
                _workspacepath = company.StoragePath;
                if (company.StorageCategory == "AWS")
                {
                    _storageManager = new CloudStorageManager(company.CloudAccessKey, company.CloudSecurityKey, company.AWSRegion, company.StoragePath);
                }
                else
                {
                    _storageManager = new LocalStaorageManager();
                }
            }
            else
            {

            }
        }

        public void CreateCompanyFolderinWorkSpace(Company company)
        {
            try
            {
                var companyPath = company.RelativePath;
                _storageManager.SaveFolder(companyPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCompanyFolderinWorkSpace(Company company)
        {
            try
            {
                var companyPath = company.RelativePath;
                _storageManager.DeleteFolder(companyPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SaveDataRoomtoWorkSpace(string companyName, string dataroomname)
        {
            try
            {
                string relativePath = companyName + "/" + dataroomname;
                _storageManager.SaveFolder(relativePath);
                string archivalPath = companyName + "/" + dataroomname + "/Archive";
                _storageManager.ArchivalFolderCreation(archivalPath);
                return relativePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteDataRoomfromWorkSpace(string relativePath)
        {
            try
            {
                _storageManager.DeleteFolder(relativePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SaveFoldertoWorkSpace(string foldername, int parentfolderid, int dataroomid)
        {
            string relativePath = string.Empty;
            try
            {
                string fullpath = string.Empty;
                if (parentfolderid == 0)
                {
                    var dataroom = DataCache.DataRooms.Single(x => x.Id == dataroomid);
                    fullpath = dataroom.RelativePath + "/" + foldername;
                }
                else
                {
                    fullpath = BuildFullFolderPath(parentfolderid, foldername);
                }
                relativePath = fullpath;
                _storageManager.SaveFolder(fullpath);
                return relativePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteFolderfromWorkSpace(string relativePath)
        {
            try
            {
                _storageManager.DeleteFolder(relativePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CopyFilefromTemptoWorkSpace(string tempfilepath, int folderid, int dataroomid, string filename)
        {
            try
            {
                string relativePath = string.Empty;
                string fullpath = string.Empty;
                if (folderid == 0)
                {
                    var dataroom = DataCache.DataRooms.Single(x => x.Id == dataroomid);
                    fullpath = dataroom.RelativePath + "/" + filename;
                }
                else
                {
                    var folder = DataCache.Folders.Single(x => x.Id == folderid);
                    fullpath = BuildFullFolderPath(folderid, string.Empty) + "/" + filename;
                }
                relativePath = fullpath;
                _storageManager.CreateFilefromTemp(tempfilepath, relativePath);
                return relativePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteFilefromWorkSpace(string relativepath)
        {
            try
            {
                _storageManager.DeleteFile(relativepath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string SaveFiletoWorkSpace(HttpPostedFileBase file, int folderid, int dataroomid,string fileVersion="")
        {
            try
            {
                var dataroom = DataCache.DataRooms.Single(x => x.Id == dataroomid);
                var folder = folderid > 0 ? DataCache.Folders.Single(x => x.Id == folderid) : new Folder();
                string folderPath = folderid > 0 ? System.IO.Path.Combine(_workspacepath, folder.RelativePath) : _workspacepath;
                string relativePath = string.Empty;
                if(!string.IsNullOrEmpty(fileVersion))
                    relativePath = folderid > 0 ? System.IO.Path.Combine(folder.RelativePath, fileVersion) : System.IO.Path.Combine(dataroom.RelativePath, fileVersion);
                else
                    relativePath = folderid > 0 ? System.IO.Path.Combine(folder.RelativePath, file.FileName) : System.IO.Path.Combine(dataroom.RelativePath, file.FileName);
                string tempPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "/Temp/" + file.FileName;
                file.SaveAs(tempPath);
                string encrypttempPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "/Temp/EncryptionTemp/" + file.FileName;                
                encryption.EncryptFile(tempPath, encrypttempPath);
                _storageManager.CreateFilefromTemp(encrypttempPath, relativePath);
                return relativePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GetFileByteArray(string filepath)
        {
            try
            {
                return _storageManager.GetFile(filepath,"");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public async Task UpdateDataRoomSize(int activitylogid, int dataroomid, int filesize, bool isaddingtodataroom)
        {
            try
            {
                var originaldataroom = DataCache.DataRooms.Single(x => x.Id == dataroomid);
                var modifieddataroom = originaldataroom.Clone();
                double dataroomsize = originaldataroom.DataRoomSize == "" ? 0 : Convert.ToDouble(originaldataroom.DataRoomSize);
                double result = DataCache.Files.Where(x => x.IsActive == true && x.DataRoomId == dataroomid).Sum(x => (string.IsNullOrEmpty(x.FileSize) ? 0 : Convert.ToDouble(x.FileSize))); //Math.Abs(isaddingtodataroom ?  (dataroomsize + filesize) : (dataroomsize - filesize));
                modifieddataroom.DataRoomSize = result.ToString();
                modifieddataroom.ModifiedBy = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
                modifieddataroom.ModifierName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
                modifieddataroom.ModifiedOn = DateTime.Now;
                await _dataroomService.UpdateDataRoom(modifieddataroom);
                new Thread(() => DataCache.RefreshSingleDataRoom(modifieddataroom)).Start();
                await _logger.LogDataDifference(activitylogid, originaldataroom, modifieddataroom, dataRoomId: modifieddataroom.Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public bool IsFileExists(int fileid)
        {
            try
            {
                var file = DataCache.Files.Single(x => x.Id == fileid);
                var encryptedfilepath = System.IO.Path.Combine(_workspacepath, file.RelativePath);
                if (System.IO.File.Exists(encryptedfilepath))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsFileVersionExists(string relativePath)
        {
            try
            {
                var encryptedfilepath = System.IO.Path.Combine(_workspacepath, relativePath);
                if (System.IO.File.Exists(encryptedfilepath))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsFolderExists(int folderid)
        {
            try
            {
                var folder = DataCache.Folders.Single(x => x.Id == folderid);
                var folderpath = System.IO.Path.Combine(_workspacepath, folder.RelativePath);
                if (System.IO.Directory.Exists(folderpath))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string BuildFullFilePath(int fileid)
        {
            List<string> folderpaths = new List<string>();
            string fullpath = string.Empty;
            try
            {
                File file = DataCache.Files.Single(x => x.Id == fileid);
                Folder folder = DataCache.Folders.Single(x => x.Id == file.FolderId);
                do
                {
                    folderpaths.Add(folder.FolderName);
                    folder = DataCache.Folders.Single(x => x.Id == folder.ParentFolderId);
                }
                while (folder.ParentFolderId != 0);
                fullpath = file.CompanyName + "/" + file.DataRoomName + "/" + string.Join("/", folderpaths.ToArray().Reverse()) + "/" + file.FileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fullpath;
        }

        public string BuildFullFolderPath(int parentfolderid, string foldername)
        {
            List<string> folderpaths = new List<string>();
            string fullpath = string.Empty;
            try
            {
                Folder folder = DataCache.Folders.Single(x => x.Id == parentfolderid);
                DataRoom dataRoom = DataCache.DataRooms.Single(x => x.Id == folder.DataRoomId);
                do
                {
                    folderpaths.Add(folder.Id.ToString());
                    if (folder.ParentFolderId > 0)
                        folder = DataCache.Folders.Single(x => x.Id == folder.ParentFolderId);
                    if (folder.ParentFolderId == 0)
                        folderpaths.Add(folder.Id.ToString());
                }
                while (folder.ParentFolderId != 0);
                if (!string.IsNullOrEmpty(foldername))
                    fullpath = dataRoom.RelativePath + "/" + string.Join("/", folderpaths.ToArray().Distinct().Reverse()) + "/" + foldername;
                else
                    fullpath = dataRoom.RelativePath + "/" + string.Join("/", folderpaths.ToArray().Distinct().Reverse());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fullpath;
        }

        public string BuildFullDataRoomPath(string companyName, string dataRoomName)
        {
            return companyName + "/" + dataRoomName;
        }

        public void CopyFile(string sourcefilepath,string destinationfilepath)
        {
            try
            {
                _storageManager.CopyFile(sourcefilepath, destinationfilepath);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void CopyFolder(string sourcepath, string destinationpath)
        {
            try
            {
                _storageManager.CopyFolder(sourcepath, destinationpath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MoveFile(string sourcefilepath, string destinationfilepath)
        {
            try
            {
                _storageManager.MoveFile(sourcefilepath, destinationfilepath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string BuildBreadCrumbs(int folderid)
        {
            try
            {
                string fullpath = string.Empty;
                List<string> folderpaths = new List<string>();
                try
                {
                    Folder folder = DataCache.Folders.Single(x => x.Id == folderid);
                    
                    do
                    {
                        folderpaths.Add("<a href='#' class='breadcrumbpart' data-dataroomid='" + folder.DataRoomId + "' data-folderid='" + folder.Id + "'>" + folder.FolderName + "</a>");
                        if (folder.ParentFolderId > 0)
                            folder = DataCache.Folders.Single(x => x.Id == folder.ParentFolderId);
                        if (folder.ParentFolderId == 0)
                            folderpaths.Add("<a href='#' class='breadcrumbpart' data-dataroomid='" + folder.DataRoomId + "' data-folderid='" + folder.Id + "'>" + folder.FolderName + "</a>");
                    }
                    while (folder.ParentFolderId != 0);
                    folderpaths.Add("<a href='#' class='breadcrumbpart' data-dataroomid='" + folder.DataRoomId + "' data-folderid='0'>" + folder.DataRoomName + "</a>");

                    fullpath = string.Join("/", folderpaths.Distinct().Reverse());



                    //if (!string.IsNullOrEmpty(foldername))
                    //    fullpath = folder.CompanyName + "/" + folder.DataRoomName + "/" + string.Join("/", folderpaths.ToArray().Distinct().Reverse()) + "/" + foldername;
                    //else
                    //    fullpath = folder.CompanyName + "/" + folder.DataRoomName + "/" + string.Join("/", folderpaths.ToArray().Distinct().Reverse());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return fullpath;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}