using DataRooms.UI.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.WebApiHelpers
{
    public class CompanyService
    {
        static HttpClient _client = new HttpClient();
        private string _authToken = string.Empty;
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        public CompanyService(string token)
        {
            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiUrl"]);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }
            _authToken = token;
        }

        public async Task<int> SaveCompany(Company company)
        {
            try
            {
                string url = _client.BaseAddress + "ManageCompany/savecompany";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, company);
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

        public async Task UpdateCompany(Company company)
        {
            try
            {
                logger.Debug(JsonConvert.SerializeObject(company));
                string url = _client.BaseAddress + "managecompany/updatecompany";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PutAsJsonAsync(url, company);
                logger.Debug("response from service ---" + response);
                if (response.IsSuccessStatusCode)
                {
                    logger.Debug("update success---" + company.RelativePath);
                    //await response.Content.ReadFromJsonAsync<string>();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + "--" + ex.StackTrace);
                throw ex;
            }
        }

        public async Task DeleteCompany(Company company)
        {
            try
            {
                string url = _client.BaseAddress + "managecompany/deletecompany";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, company);
                if (response.IsSuccessStatusCode)
                {
                    //await response.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}