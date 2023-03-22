using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public int FileId { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public int AuditorId { get; set; }
        public string AuditorName { get; set; }
        public DateTime? AuditOn { get; set; }
        public string Comments { get; set; }
    }
}