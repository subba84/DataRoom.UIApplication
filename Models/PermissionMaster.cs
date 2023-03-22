using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class PermissionMaster
    {
        [Key]
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string PermissionCategory { get; set; }
        public bool IsActive { get; set; }
    }
}