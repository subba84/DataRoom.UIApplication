using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Activation.Controllers
{
    public class ADActivationController : Controller
    {
        ActivationService _service;

        public ADActivationController()
        {
            _service = new ActivationService();
        }
        public ActionResult Index()
        {
            return View();
        }

        //public async Task<ActionResult> SaveADInfo(ADInfo model)
        //{
        //    try
        //    {
        //        HostDetails hostDetails = await _service.GetHostInformation();
        //        await _service.SaveADInfo(model);
        //        UpdateKeyValue(model.IsADSync);
        //        await _service.SaveCustomerActivationLog(hostDetails.EmailId);
        //        new Thread(() => DataCache.Load()).Start();
        //        return RedirectToAction("Login", "Authenticate", new { area = "Login" });
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public static void UpdateKeyValue(string isADSync)
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            string key = "ActivateKey";
            string value = "1";
            config.AppSettings.Settings[key].Value = value;
            config.Save();
            key = "IsADAuthentication";
            value = isADSync;
            config.AppSettings.Settings[key].Value = value;
            config.Save();
        }
    }
}