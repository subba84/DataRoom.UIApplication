using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class RoleMaster
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}