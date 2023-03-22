using DataRooms.UI.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.TaskApproval.Model
{
    public class CustomReviewModel : PagingModel
    {
        public IPagedList<ToDoTask> PagedTaks { get; set; }
        public File File { get; set; }
        public int TaskId { get; set; }
        public IEnumerable<AuditLog> AuditHistory { get; set; }
    }
}