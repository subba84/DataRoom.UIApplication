using DataRooms.UI.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Code
{
    public class CloudStorageManager : IFileStorageManager
    {
        S3Helper _s3Helper = null;
        public CloudStorageManager(string accesskey,string securitykey,string region,string bucketname)
        {
            _s3Helper = new S3Helper(accesskey, securitykey, region, bucketname);
        }
        public void CopyFile(string sourcepath, string destinationpath)
        {
            try
            {
                _s3Helper.CopyFile(sourcepath, destinationpath);
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
                //_s3Helper.CopyFolder(sourcepath, destinationpath);
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
                _s3Helper.DeleteFile(path);
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
                _s3Helper.DeleteFolder(path);
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
                return _s3Helper.GetFile(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void MoveFile(string sourcepath, string destinationpath)
        {
            try
            {
                _s3Helper.MoveFile(sourcepath, destinationpath);
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
                //_s3Helper.MoveFolder(sourcepath, destinationpath);
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
                //_s3Helper.savef(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateFilefromTemp(string tempfilepath,string filepath)
        {
            try
            {
                _s3Helper.CreateFilefromTemp(tempfilepath, filepath);
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
                _s3Helper.CreateFolder(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ArchivalFolderCreation(string path)
        {
            try
            {
               // _s3Helper.CreateFolder(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}