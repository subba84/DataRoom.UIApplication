using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataRooms.UI.Models
{
    public class FilePermission
    {
        [Key]
        public int Id { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool HasFullControl { get; set; }
        public bool HasRead { get; set; }
        public bool HasWrite { get; set; }
        public bool HasDelete { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifierName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int DeletedBy { get; set; }
        public string DeletorName { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
