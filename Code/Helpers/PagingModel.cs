using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI
{
    public class PagingModel
    {
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string SearchString { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

        public int[] PageSizes { get; set; }
        public PagingModel()
        {
            //SearchString = "empty";
            SortColumn = "CreatedOn";
            SortOrder = "asc";
            CurrentPage = 1;
            RecordsPerPage = 10;
            PageSizes = new int[] { 10,50,100,500,1000};
        }
    }
}