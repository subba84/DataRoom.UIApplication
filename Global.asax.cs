
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DataRooms.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Initialise();
            // DataCache Load
            int isActivated = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ActivateKey"]);
            if (isActivated == 1)
            {
                DataCache.Load();
            }
            (new Scheduler()).Start();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();
            if (exception != null)
            {
                Session["ErrorInfo"] = exception;
                // clear error on server
                Server.ClearError();
                Response.Redirect("/ManageSetting/Error");
            }
        }
    }
}
