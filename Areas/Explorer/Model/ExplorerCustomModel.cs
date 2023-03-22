using DataRooms.UI.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Explorer.Model
{
    public class ExplorerCustomModel : PagingModel
    {
        public ExplorerCustomModel()
        {
            RecordsPerPage = 100;
        }
        public DataRooms.UI.Models.DataRoom DataRoom { get; set; }
        public DataRoomPermission DataRoomPermission { get; set; }
        public FolderPermission FolderPermission { get; set; }
        public Folder Folder { get; set; }
        public IEnumerable<DataRooms.UI.Models.DataRoom> DataRooms { get; set; }
        public IEnumerable<Folder> Folders { get; set; }        
        public IEnumerable<File> Files { get; set; }
        public IEnumerable<ItemTrackerMetaData> ItemTrackers { get; set; }
        public IEnumerable<ItemTrackerwithPermission> ItemTrackersswithPemrissions { get; set; }
        public IEnumerable<FolderwithPermission> FolderswithPemrissions { get; set; }
        public IEnumerable<FilewithPermission> FileswithPermissions { get; set; }
        public IEnumerable<DataRoomwithPermission> DataRoomswithPermissions { get; set; }
        public IPagedList<DataRoomContentModel> PagedFoldersandFiles { get; set; }
        public IPagedList<Folder> PagedFolders { get; set; }
        public IPagedList<File> PagedFiles { get; set; }
        public IPagedList<DataRooms.UI.Models.DataRoom> PagedDataRooms { get; set; }
        public bool IsParentFolder { get; set; }
        public string FolderTreeView { get; set; }
        public List<string> FolderTreeViews { get; set; }
        public bool IsFileCachingEnabled { get; set; }
    }

    public class DataRoomContentModel : PagingModel
    {
        public int Id { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public int ParentFolderId { get; set; }
        public string ParentFolderName { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public string Name { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ItemType { get; set; }
        public bool IsParentFolder { get; set; }
    }
}