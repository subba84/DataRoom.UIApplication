using DataRooms.UI.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Admin.Model
{
    public class FileCheckInModel : PagingModel
    {
        public IPagedList<File> PagedCheckedInFiles { get; set; }
    }
}