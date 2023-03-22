using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class Module
    {
        public string ModuleName { get; set; }
        public string ModuleCount { get; set; }
        public string LicenseStatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}