using DataRooms.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.WebApiHelpers
{
    public class ActivationService
    {
        static HttpClient _client = new HttpClient();
        public ActivationService()
        {
            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiUrl"]);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async Task<bool> ConfigureDataBase(DBConfigureData model)
        {
            try
            {
                string url = _client.BaseAddress + "activation/checkandcreatedatabase";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        public async Task<HostDetails> GetHostInformation()
        {
            try
            {
                string url = _client.BaseAddress + "activation/gethostdetails";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<HostDetails>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<Module> GetAvailableModules()
        {
            try
            {
                string url = _client.BaseAddress + "activation/getlicenseinfo";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Module>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<List<Module>> GetModulefromApi(HostDetails hostDetails)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://partner.databricks.online/");//https://partner.databricks.online/
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            List<Module> modules = null;
            try
            {
                string domainname = string.Empty;
                string ipaddess = string.Empty;
                var url = "api/powebapi/get";
                client.DefaultRequestHeaders.Add("DomainName", hostDetails.HostName);
                client.DefaultRequestHeaders.Add("PublicIp", hostDetails.IpAddress);
                // Get the modules
                string modulesJsonstring = string.Empty;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + url);
                if (response.IsSuccessStatusCode)
                {
                    modulesJsonstring = await response.Content.ReadAsStringAsync();
                }
                modules = JsonConvert.DeserializeObject<List<Module>>(modulesJsonstring);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return modules;
        }

        public async Task SaveCustomerActivationLog(string emailid)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://partner.databricks.online/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var url = "api/powebapi/savecustomeractivationlog?email=" + emailid;
                // Get the modules
                string modulesJsonstring = string.Empty;
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + url);
                if (response.IsSuccessStatusCode)
                {
                    await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task<int> SaveLicenseInfo(LicenseInfo model)
        {
            try
            {
                string url = _client.BaseAddress + "activation/savelicenseinfo";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        
    }
}