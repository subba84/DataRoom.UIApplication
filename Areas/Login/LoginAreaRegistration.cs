using System.Web.Mvc;

namespace DataRooms.UI.Areas.Login
{
    public class LoginAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Login";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "login", new { action = "Login", controller = "Authenticate", id = UrlParameter.Optional });
            context.MapRoute(null, "logout", new { action = "Logout", controller = "Authenticate", id = UrlParameter.Optional });
            context.MapRoute(null, "userbyemail", new { action = "GetUserbyEmailId", controller = "Authenticate", id = UrlParameter.Optional });
            context.MapRoute(null, "resetpassword", new { action = "ResetPassword", controller = "Authenticate", id = UrlParameter.Optional });
            context.MapRoute(null, "updatepassword", new { action = "UpdatePassword", controller = "Authenticate", id = UrlParameter.Optional });
            context.MapRoute(null, "changerole", new { action = "ChangeRole", controller = "Authenticate", id = UrlParameter.Optional });
        }
    }
}