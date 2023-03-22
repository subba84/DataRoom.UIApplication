using System.Web.Mvc;

namespace DataRooms.UI.Areas.Masters
{
    public class MastersAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Masters";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "workflows", new { action = "WorkFlows", controller = "Master", id = UrlParameter.Optional });
            context.MapRoute(null, "editworkflow", new { action = "EditWorkFlow", controller = "Master", id = UrlParameter.Optional });
            context.MapRoute(null, "deleteworkflow", new { action = "DeleteWorkFlow", controller = "Master", id = UrlParameter.Optional });
        }
    }
}