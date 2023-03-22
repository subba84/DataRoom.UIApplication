using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRooms.UI.Code
{
    public interface IFileStorageManager
    {
        void SaveFolder(string path);
        void DeleteFolder(string path);
        void CopyFolder(string sourcepath, string destinationpath);
        void MoveFolder(string sourcepath, string destinationpath);
        void SaveFile(string path);
        void DeleteFile(string path);
        void CopyFile(string sourcepath, string destinationpath);
        void MoveFile(string sourcepath, string destinationpath);
        Byte[] GetFile(string path, string version);
        void CreateFilefromTemp(string tempfilepath, string filepath);
        void ArchivalFolderCreation(string path);
    }
}
