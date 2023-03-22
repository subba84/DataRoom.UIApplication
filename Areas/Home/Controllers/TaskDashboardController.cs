using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Home.Controllers
{
    [SessionExpire]
    public class TaskDashboardController : Controller
    {
        int loggedInUser = 0;
        string loggedInUserName = string.Empty;
        public TaskDashboardController()
        {
            loggedInUser = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
            loggedInUserName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]);
        }
        public ActionResult Index()
        {
            var reviewDetails = DataCache.ToDoTasks.Where(x => x.UserId == loggedInUser && x.UserRoleId == AppRole.Reviewer);
            ViewBag.ReviewCount = reviewDetails.Count();
            var approvalDetails = DataCache.ToDoTasks.Where(x => x.UserId == loggedInUser && x.UserRoleId == AppRole.Approver);
            ViewBag.ApprovalCount = approvalDetails.Count();
            return View();
        }
    }
}