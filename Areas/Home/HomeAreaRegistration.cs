using System.Web.Mvc;

namespace DataRooms.UI.Areas.Home
{
    public class HomeAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Home";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "home", new { action = "Home", controller = "Dashboard", id = UrlParameter.Optional });
            context.MapRoute(null, "admindashboard", new { action = "AdminDashboard", controller = "Admin", id = UrlParameter.Optional });
            context.MapRoute(null, "sizechart", new { action = "GetDataRoomsforSizeChart", controller = "Admin", id = UrlParameter.Optional });
            context.MapRoute(null, "lastuploaded", new { action = "GetLastModifiedFilesbyUser", controller = "Admin", id = UrlParameter.Optional });
            context.MapRoute(null, "task/dashboard", new { action = "Index", controller = "TaskDashboard", id = UrlParameter.Optional });
            context.MapRoute(null, "superadmin/dashboard", new { action = "SuperAdminDashboard", controller = "Dashboard", id = UrlParameter.Optional });
            context.MapRoute(null, "superadmin/getdashboarddata", new { action = "GetDataforSuperAdminDashboard", controller = "Dashboard", id = UrlParameter.Optional });
        }
    }
}