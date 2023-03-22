using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class File
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FileSize { get; set; }
        public string Guid { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public string FileDescription { get; set; }
        public string RelativePath { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedOn { get; set; }
        public bool IsDeleted { get; set; }
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
        public bool? IsCheckIn { get; set; }
        public int? CheckInBy { get; set; }
        public string CheckInByName { get; set; }
        public DateTime? CheckInOn { get; set; }
        public bool? IsCheckOut { get; set; }
        public int? CheckOutBy { get; set; }
        public string CheckOutByName { get; set; }
        public DateTime? CheckOutOn { get; set; }
        public string FileVersion { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public bool? IsWorkFlowRequired { get; set; }
        public bool? IsApproved { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApproverName { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public string WaitingWithName { get; set; }
        public string Status { get; set; }
    }
}