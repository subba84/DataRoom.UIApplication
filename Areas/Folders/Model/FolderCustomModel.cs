using DataRooms.UI.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Folders.Model
{
    public class FolderCustomModel : PagingModel
    {
        public int FolderId { get; set; }
        public Folder Folder { get; set; }
        public IPagedList<DataRooms.UI.Models.FolderPermission> PagedFolderPermissions { get; set; }
    }
}