using System.Web.Mvc;

namespace DataRooms.UI.Areas.Users
{
    public class UsersAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Users";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "users/list", new { action = "List", controller = "ManageUser", id = UrlParameter.Optional });
            context.MapRoute(null, "user/edt", new { action = "Edit", controller = "ManageUser", id = UrlParameter.Optional });
            context.MapRoute(null, "user/delete", new { action = "Delete", controller = "ManageUser", id = UrlParameter.Optional });

            context.MapRoute(null, "userroles/getallusersforsearch", new { action = "GetAllUsers", controller = "ManageUserRole", id = UrlParameter.Optional });
            context.MapRoute(null, "userroles/getallroles", new { action = "GetAllRoles", controller = "ManageUserRole", id = UrlParameter.Optional });
            context.MapRoute(null, "userroles/list", new { action = "List", controller = "ManageUserRole", id = UrlParameter.Optional });
            context.MapRoute(null, "userroles/edt", new { action = "Edit", controller = "ManageUserRole", id = UrlParameter.Optional });
            context.MapRoute(null, "userroles/delete", new { action = "Delete", controller = "ManageUserRole", id = UrlParameter.Optional });
            context.MapRoute(null, "manageuser/checkcompanyadauth", new { action = "CheckCompanyADAuthorNot", controller = "ManageUser", id = UrlParameter.Optional });
        }
    }
}