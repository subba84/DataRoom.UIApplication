using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI
{
    public class FilterModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public int CurrentPage { get; set; }
        public int RecordsPerPage { get; set; }
        public string SearchString { get; set; }
        public string TableName { get; set; }
        public string WhereCondition { get; set; }
        public string Sort { get; set; }
    }
}