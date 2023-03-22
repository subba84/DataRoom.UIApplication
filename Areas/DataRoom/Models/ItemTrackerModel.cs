using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Web;

namespace DataRooms.UI.Areas.DataRoom.Models
{
    public class ItemTrackerModel
    {
        public int Id { get; set; }
        public bool IsMandatory { get; set; }
        public string ControlType { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int ItemTrackerId { get; set; }
        public string ItemTrackerName { get; set; }
        public string ItemTrackerGuid { get; set; }
        public List<StructuralData> ControlData { get; set; }
        public List<ItemTrackerControl> DataRoomItemTrackerControls { get; set; }
        public List<ItemTrackerPermission> ItemTrackerPermissions { get; set; }
        public List<ItemTrackerData> DataRoomItemTrackerData { get; set; }
    }

    public class StructuralData
    {
        public string ControlName { get; set; }
        public string ControlData { get; set; }
        public string ControlReferenceData { get; set; }
    }
}