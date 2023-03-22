using System.Web.Mvc;

namespace DataRooms.UI.Areas.TaskApproval
{
    public class TaskApprovalAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TaskApproval";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "taskreview/list", new { action = "List", controller = "ManageReview", id = UrlParameter.Optional });
            context.MapRoute(null, "taskreview/edit", new { action = "Edit", controller = "ManageReview", id = UrlParameter.Optional });
            context.MapRoute(null, "taskapproval/list", new { action = "List", controller = "ManageApproval", id = UrlParameter.Optional });
            context.MapRoute(null, "taskapproval/edit", new { action = "Edit", controller = "ManageApproval", id = UrlParameter.Optional });
        }
    }
}