using DataRooms.UI.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Logging.Models
{
    public class ActivityLogModel : PagingModel
    {      
        public IPagedList<ActivityLog> PagedActivityLogs { get; set; }
        public List<DataLogObject> DataLogs { get; set; }
    }

    public class DataLogObject
    {
        public Dictionary<string,string> OriginalData { get; set; }
        public Dictionary<string,string> ModifiedData { get; set; }
        public Dictionary<string,string> Changes { get; set; }
        public string TableName { get; set; }
    }
}