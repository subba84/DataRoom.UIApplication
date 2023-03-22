using DataRooms.UI.Code.Helpers;
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
    public class ManageEmailConfigController : Controller
    {
        private readonly EmailConfigService _service;
        public ManageEmailConfigController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new EmailConfigService(token);
        }
        [HttpGet]
        public ActionResult Edit()
        {
            var emailconfigs = DataCache.EmailConfiguration;
            if(emailconfigs == null || emailconfigs.Count == 0)
            {
                emailconfigs.Add(new Models.EmailConfiguration { MailType = "OutBound" });
                emailconfigs.Add(new Models.EmailConfiguration { MailType = "Incoming" });
            }
            JsonResult jr = Json(new
            {
                HTML = this.RenderPartialView(@"~\Areas\Settings\Views\ManageEmailConfig\_editemailconfig.cshtml", emailconfigs)
            });
            jr.MaxJsonLength = int.MaxValue;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }

        [HttpPost]
        public async Task<JsonResult> Edit(List<Models.EmailConfiguration> EmailConfigurations)
        {
            try
            {
                if(EmailConfigurations[0].Id == 0)
                {
                    foreach(var item in EmailConfigurations)
                    {
                        item.CreatedBy = Convert.ToInt32(Session["UserId"]);
                        item.CreatorName = Convert.ToString(Session["UserName"]);
                        item.CreatedOn = DateTime.Now;
                        await _service.SaveEmailConfiguration(item);
                    }
                }
                else
                {
                    foreach (var item in EmailConfigurations)
                    {
                        item.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                        item.ModifierName = Convert.ToString(Session["UserName"]);
                        item.ModifiedOn = DateTime.Now;
                        await _service.UpdateEmailConfiguration(item);
                    }
                }
                DataCache.RefreshEmailConfiguration();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json("");
        }
    }
}