using DataRooms.UI.Code.Helpers;
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
    public class ManageADConfigController : Controller
    {
        private readonly UserService _service;
        public ManageADConfigController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new UserService(token);
        }
        [HttpGet]
        public ActionResult Edit()
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var adconfig = DataCache.ADConfiguration.Where(x=>x.CompanyId == companyId);
            ADInfo aDInfo = new ADInfo();
            if(adconfig!=null && adconfig.Count() > 0)
            {
                aDInfo = adconfig.First();
            }
            JsonResult jr = Json(new
            {
                HTML = this.RenderPartialView(@"~\Areas\Settings\Views\Shared\_editADConfig.cshtml", aDInfo)
            });
            jr.MaxJsonLength = int.MaxValue;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }

        [HttpPost]
        public async Task<JsonResult> Edit(ADInfo aDInfo)
        {
            try
            {
                aDInfo.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                aDInfo.CompanyName = Convert.ToString(Session["CompanyName"]);
                if (aDInfo.Id == 0)
                {
                    await _service.SaveADInfo(aDInfo);
                }
                else
                {
                    await _service.UpdateADInfo(aDInfo);
                }
                DataCache.RefreshADConfiguration();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("");
        }
    }
}