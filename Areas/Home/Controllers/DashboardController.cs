using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Home.Controllers
{
    [SessionExpire]
    public class DashboardController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult SuperAdminDashboard()
        {
            return View();
        }

        public JsonResult GetDataforSuperAdminDashboard()
        {
            List<CompanyReport> reportData = new List<CompanyReport>();
            try
            {
                var companies = DataCache.Companies.Where(x=>x.IsActive == true);
                if(companies!=null && companies.Count() > 0)
                {
                    foreach(var company in companies.ToList())
                    {
                        CompanyReport comp = new CompanyReport();
                        comp.CompanyId = company.Id;
                        comp.CompanyName = company.CompanyName;
                        var usercount = DataCache.Users.Where(x => x.CompanyId == comp.CompanyId);
                        if(usercount!=null && usercount.Count() > 0)
                        {
                            comp.UserCount = usercount.Count();
                        }
                        else
                        {
                            comp.UserCount = 0;
                        }
                        var datarooms = DataCache.DataRooms.Where(x => x.IsActive == true);
                        double storage = 0;
                        if(datarooms!=null && datarooms.Count() > 0)
                        {
                            foreach(var dataroom in datarooms.ToList())
                            {
                                storage += Convert.ToDouble(dataroom.DataRoomSize);
                            }
                            comp.CompanyStorage = storage.ConvertSize("GB");
                        }
                        else
                        {
                            comp.CompanyStorage = "0";
                        }

                        reportData.Add(comp);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json(reportData, JsonRequestBehavior.AllowGet);
        }
    }

    public class CompanyReport
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int UserCount { get; set; }
        public string CompanyStorage { get; set; }
    }
}