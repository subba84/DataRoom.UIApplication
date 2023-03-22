using DataRooms.UI.Code;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Settings.Controllers
{
    [SessionExpire]
    public class ManageSettingController : Controller
    {
        public ManageSettingController()
        {
        }
        public ActionResult SettingsPage()
        {
            return View();
        }

        public ActionResult RefreshDataCache()
        {
            try
            {
                DataCache.RefreshCache();
                TempData["Notification"] = "DataCache Refreshed Successfully";
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("SettingsPage");
        }

        public ActionResult Error()
        {
            TempData["Error"] = System.Web.HttpContext.Current.Session["ErrorInfo"] as Exception;
            return View();
        }

        public async Task<JsonResult> ADSync()
        {
            try
            {
                int currentroleid = Convert.ToInt32(Session["CurrentRoleId"]);
                if (currentroleid == AppRole.SuperAdmin)
                {
                    await ADHelper.ADSync(0);
                }
                else
                {
                    int companyid = Convert.ToInt32(Session["CompanyId"]);
                    await ADHelper.ADSync(companyid);
                }
                TempData["Notification"] = "AD Sync Successfully Done";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("",JsonRequestBehavior.AllowGet);
        }
    }
}