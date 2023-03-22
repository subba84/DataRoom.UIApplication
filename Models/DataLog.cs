using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class DataLog
    {
        [Key]
        public int Id { get; set; }
        public int ActivityLogId { get; set; }
        public int DataRoomId { get; set; }
        public string TableName { get; set; }
        public string OriginalData { get; set; }
        public string ModifiedData { get; set; }
        public string Changes { get; set; }
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
    }
}