using DataRooms.UI.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Files.Models
{
    public class FileCustomModel : PagingModel
    {
        public int FileId { get; set; }
        public File File { get; set; }
        public string WaitingWith { get; set; }
        public IEnumerable<AuditLog> AuditLogs { get; set; }
        public IPagedList<FilePermission> PagedFilePermissions { get; set; }
    }
}