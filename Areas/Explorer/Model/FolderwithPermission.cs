using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Explorer.Model
{
    public class FolderwithPermission : Folder
    {
        public FolderPermission FolderPermission { get; set; }
        public bool IsFolderExists { get; set; }
    }
}