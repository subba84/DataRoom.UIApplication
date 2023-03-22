using System.Web.Mvc;

namespace DataRooms.UI.Areas.Activation
{
    public class ActivationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Activation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "dbsetup", new { action = "ConfigureDatabase", controller = "DatabaseSetup", id = UrlParameter.Optional });
            context.MapRoute(null, "activate", new { action = "Index", controller = "Activate", id = UrlParameter.Optional });
            context.MapRoute(null, "module", new { action = "Activate", controller = "Activate", id = UrlParameter.Optional });
            context.MapRoute(null, "adinfo", new { action = "Index", controller = "ADActivation", id = UrlParameter.Optional });
            context.MapRoute(null, "saveadinfo", new { action = "SaveADInfo", controller = "ADActivation", id = UrlParameter.Optional });
            context.MapRoute(null, "getavailablemodules", new { action = "GetAvailableModules", controller = "Activate", id = UrlParameter.Optional });
        }
    }
}