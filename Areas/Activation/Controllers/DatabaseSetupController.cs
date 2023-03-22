using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Activation.Controllers
{
    public class DatabaseSetupController : Controller
    {
        string secretkey = ConfigurationManager.AppSettings["SecretKey"];

        [HttpGet]
        public ActionResult ConfigureDatabase()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ConfigureDatabase(DBConfigureData model)
        {
            ActivationService _service = new ActivationService();
            model.EncryptedPublicIp = EncryptionHelper.Encrypt(model.PublicIp, secretkey);
            model.EncryptedEmailId = EncryptionHelper.Encrypt(model.EmailId, secretkey);
            model.EncryptedDomainName = EncryptionHelper.Encrypt(model.DomainName, secretkey);
            bool isDataBaseCreationSuccessful = await _service.ConfigureDataBase(model);
            if(isDataBaseCreationSuccessful)
              return RedirectToAction("Index","Activate",new { area = "Activation"});
            else
              return View();
        }
    }
}