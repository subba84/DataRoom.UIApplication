using System.Web.Mvc;

namespace DataRooms.UI.Areas.RecycleBin
{
    public class RecycleBinAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RecycleBin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "recyclebin", new { action = "List", controller = "Recycle", id = UrlParameter.Optional });
            context.MapRoute(null, "recyclebin/delete", new { action = "Delete", controller = "Recycle", id = UrlParameter.Optional });
            context.MapRoute(null, "recyclebin/restore", new { action = "Restore", controller = "Recycle", id = UrlParameter.Optional });
            context.MapRoute(null, "recyclebin/empty", new { action = "EmptyRecycleBin", controller = "Recycle", id = UrlParameter.Optional });
        }
    }
}