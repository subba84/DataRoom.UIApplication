using System.Web.Mvc;

namespace DataRooms.UI.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "filecheckin/list", new { action = "List", controller = "FileCheckIn", id = UrlParameter.Optional });
            context.MapRoute(null, "filecheckin/checkin", new { action = "CheckInFile", controller = "FileCheckIn", id = UrlParameter.Optional });
        }
    }
}