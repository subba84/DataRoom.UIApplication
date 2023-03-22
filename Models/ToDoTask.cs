using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class ToDoTask
    {
        [Key]
        public int Id { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public int FolderId { get; set; }
        public string FolderName { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
        public string TaskCategory { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int UserRoleId { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}