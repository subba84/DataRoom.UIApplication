using System.Web.Mvc;

namespace DataRooms.UI.Areas.Company
{
    public class CompanyAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Company";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "companies/list", new { action = "List", controller = "ManageCompany", id = UrlParameter.Optional });
            context.MapRoute(null, "companies/edit", new { action = "Edit", controller = "ManageCompany", id = UrlParameter.Optional });
            context.MapRoute(null, "companies/delete", new { action = "Delete", controller = "ManageCompany", id = UrlParameter.Optional });
        }
    }
}