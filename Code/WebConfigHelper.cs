using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace DataRooms.UI.Code
{
    public static class WebConfigHelper
    {
        public static void UpdateKeyValue()
        {
            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            string key = "ActivateKey";
            string value = "1";
            config.AppSettings.Settings[key].Value = value;
            config.Save();
        }
    }
}