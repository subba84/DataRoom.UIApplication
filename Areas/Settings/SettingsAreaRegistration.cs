using System.Web.Mvc;

namespace DataRooms.UI.Areas.Settings
{
    public class SettingsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Settings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "settings/list", new { action = "SettingsPage", controller = "ManageSetting", id = UrlParameter.Optional });
            context.MapRoute(null, "settings/refreshcache", new { action = "RefreshDataCache", controller = "ManageSetting", id = UrlParameter.Optional });
            context.MapRoute(null, "settings/customerror", new { action = "Error", controller = "ManageSetting", id = UrlParameter.Optional });
            context.MapRoute(null, "manageemailconfig/edit", new { action = "Edit", controller = "ManageEmailConfig", id = UrlParameter.Optional });
            context.MapRoute(null, "manageadconfig/edit", new { action = "Edit", controller = "ManageADConfig", id = UrlParameter.Optional });
            context.MapRoute(null, "settings/adsync", new { action = "ADSync", controller = "ManageSetting", id = UrlParameter.Optional });
        }
    }
}