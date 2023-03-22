using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataRooms.UI.Models;

namespace DataRooms.UI.Areas.RecycleBin.Model
{
    public class RecycleBinModel : PagingModel
    {
        public IEnumerable<DataRooms.UI.Models.DataRoom> DataRooms { get; set; }
        public IEnumerable<Folder> Folders { get; set; }
        public IEnumerable<File> Files { get; set; }
    }
}