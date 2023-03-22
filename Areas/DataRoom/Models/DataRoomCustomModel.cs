using DataRooms.UI.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.DataRoom.Models
{
    public class DataRoomCustomModel : PagingModel
    {
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public IEnumerable<WorkFlowMaster> WorkFlows { get; set; }
        public DataRooms.UI.Models.DataRoom DataRoom { get; set; }
        public IPagedList<DataRooms.UI.Models.DataRoomPermission> PagedDataRoomPermissions { get; set; }
        public IPagedList<DataRooms.UI.Models.DataRoom> PagedDataRooms { get; set; }

        public IEnumerable<DataRooms.UI.Models.DataRoomPermission> DataRoomPermissions { get; set; }
        public IEnumerable<DataRooms.UI.Models.DataRoom> DataRooms { get; set; }
        public string PermissionCategory { get; set; }
        public WorkFlowMaster WorkFlow { get; set; }
        public List<DataRoomWorkFlowUser> WorkFlowUsers { get; set; }
    }
}