using System.Web.Mvc;

namespace DataRooms.UI.Areas.Logging
{
    public class LoggingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Logging";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "logs/list", new { action = "List", controller = "ManageLog", id = UrlParameter.Optional });
            context.MapRoute(null, "datalogs", new { action = "GetDataLogsbyActivityLogId", controller = "ManageLog", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroomlogs/list", new { action = "List", controller = "DataRoomLog", id = UrlParameter.Optional });
            context.MapRoute(null, "folderlogs/list", new { action = "List", controller = "FolderLog", id = UrlParameter.Optional });
            context.MapRoute(null, "filelogs/list", new { action = "List", controller = "FileLog", id = UrlParameter.Optional });
        }
    }
}