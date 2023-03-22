using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class ItemTrackerControl
    {
        [Key]
        public int Id { get; set; }
        public int ItemTrackerId { get; set; }
        public bool IsMandatory { get; set; }
        public string ControlType { get; set; }
        public string ControlName { get; set; }
        public bool IsVisible { get; set; }
        public int OrderNumber { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public string ControlMasterData { get; set; }
        public int ControlReferenceId { get; set; }
        public string ControlReferenceName { get; set; }
        public string ControlGuid { get; set; }
        public string ParentGuid { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifierName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int DeletedBy { get; set; }
        public string DeletorName { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string ItemTrackerGuid { get; set; }
    }
}