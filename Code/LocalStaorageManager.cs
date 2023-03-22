using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Code
{
    public class LocalStaorageManager : IFileStorageManager
    {
        private string workspacePath = DataCache.Settings.StoragePath;

        public void CopyFile(string sourcepath, string destinationpath)
        {
            try
            {
                //sourcepath = workspacePath + "/" + sourcepath;
                //destinationpath = workspacePath + "/" + destinationpath;
                if (System.IO.File.Exists(sourcepath))
                {
                    if (!System.IO.File.Exists(destinationpath))
                    {
                        System.IO.File.Copy(sourcepath, destinationpath);
                    }
                    else
                    {
                        System.IO.File.Delete(destinationpath);
                        GC.Collect();
                        System.IO.File.Copy(sourcepath, destinationpath);
                    }
                }
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
                if (System.IO.Directory.Exists(sourcepath))
                {
                    if (!System.IO.Directory.Exists(destinationpath))
                    {
                        System.IO.Directory.CreateDirectory(destinationpath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteFile(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteFolder(string path)
        {
            try
            {
                
                if (System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path,true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GetFile(string path, string version)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    return System.IO.File.ReadAllBytes(path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public void MoveFile(string sourcepath, string destinationpath)
        {
            try
            {
                //sourcepath = workspacePath + "/" + sourcepath;
                //destinationpath = workspacePath + "/" + destinationpath;
                if (System.IO.File.Exists(sourcepath))
                {
                    if (!System.IO.File.Exists(destinationpath))
                    {
                        System.IO.File.Move(sourcepath, destinationpath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MoveFolder(string sourcepath, string destinationpath)
        {
            try
            {
                if (System.IO.Directory.Exists(sourcepath))
                {
                    if (!System.IO.Directory.Exists(destinationpath))
                    {
                        System.IO.Directory.CreateDirectory(destinationpath);
                    }
                    System.IO.Directory.Delete(sourcepath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveFile(string path)
        {
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Create(path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveFolder(string path)
        {
            try
            {
                //path = workspacePath + "/" + path;
                if (!string.IsNullOrEmpty(path))
                {
                    path = path.Replace(@"\\", @"\");
                    path = path.Replace(@"//", @"/");
                    path = path.Replace(@"/", @"\");
                }
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void CreateFilefromTemp(string tempfilepath, string filepath)
        {
            try
            {
                //filepath = workspacePath + "/" + filepath;
                CopyFile(tempfilepath, filepath);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void ArchivalFolderCreation(string path)
        {
            try
            {
                //path = workspacePath + "/" + path;
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void CopyFolderStructure(int selectedfolderid,int targetfolderid,int dataroomid)
        //{
        //    try
        //    {
        //        var dataroom = DataCache.DataRooms.First(x => x.Id == dataroomid);
        //        var selectedfolder = DataCache.Folders.First(x=>x.Id == selectedfolderid);
        //        int parentfolderid = selectedfolder.Id;

        //        List<Models.Folder> folderstobecopied = new List<Models.Folder>();

        //        Action<Models.Folder> SetChildren = null;
        //        SetChildren = parent =>
        //        {
        //            var subfolders = DataCache.Folders
        //                .Where(childItem => childItem.ParentFolderId == parent.Id)
        //                .ToList();

        //            foreach (var item in subfolders)
        //            {
        //                // Add Folder to list
        //                folderstobecopied.Add(item);
        //            }

        //            //Recursively call the SetChildren method for each child.
        //            subfolders
        //                .ForEach(SetChildren);
        //        };

        //        //Initialize the hierarchical list to root level items
        //        List<Models.Folder> hierarchicalItems = DataCache.Folders
        //            .Where(rootItem => rootItem.ParentFolderId == selectedfolder.Id)
        //            .ToList();

        //        //Call the SetChildren method to set the children on each root level item.
        //        hierarchicalItems.ForEach(SetChildren);

        //        Models.Folder targetFolder = new Models.Folder();
        //        if(targetfolderid > 0)
        //        {
        //            targetFolder = DataCache.Folders.First(x=>x.Id == targetfolderid);
        //        }

        //        if(folderstobecopied!=null && folderstobecopied.Count() > 0)
        //        {
        //            foreach(var folder in folderstobecopied)
        //            {
        //                Models.Folder destFolder = new Models.Folder();
        //                destFolder.DataRoomId = dataroom.Id;
        //                destFolder.DataRoomName = dataroom.DataRoomName;
        //                destFolder.FolderName = folder.FolderName;
        //                destFolder.Guid = folder.Guid;
        //                destFolder.FolderDescription = folder.FolderDescription;
        //                destFolder.IsActive = true;
        //                destFolder.CreatedBy = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
        //                destFolder.CreatorName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        //                destFolder.CreatedOn = DateTime.Now;
        //                destFolder.CompanyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        //                destFolder.CompanyName = Convert.ToString(System.Web.HttpContext.Current.Session["CompanyName"]);
        //                destFolder.ParentFolderId = targetFolder.Id;
        //                destFolder.ParentFolderName = targetFolder.FolderName;
        //                destFolder.RelativePath = targetFolder.RelativePath + "/" + destFolder.FolderName;
        //                CopyFolder(selectedfolder.RelativePath, destFolder.RelativePath);

        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}