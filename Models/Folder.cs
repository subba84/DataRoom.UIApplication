using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }
        public string FolderName { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public int ParentFolderId { get; set; }
        public string ParentFolderName { get; set; }
        public string FolderDescription { get; set; }
        public string RelativePath { get; set; }
        public string Guid { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifierName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? DeletedBy { get; set; }
        public string DeletorName { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; }
    }
}