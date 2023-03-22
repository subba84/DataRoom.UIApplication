using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Explorer.Model
{
    public class FilewithPermission : File
    {
        public FilePermission FilePermission { get; set; }
        public bool IsFileExists { get; set; }
    }
}