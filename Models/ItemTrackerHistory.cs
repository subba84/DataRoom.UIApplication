using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class ItemTrackerHistory
    {
        [Key]
        public int Id { get; set; }
        public int ItemTrackerId { get; set; }
        public string ItemTrackerName { get; set; }
        public string ItemTrackerRowGuid { get; set; }
        public string ColumnName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}