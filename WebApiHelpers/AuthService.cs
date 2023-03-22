using DataRooms.UI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.WebApiHelpers
{
    public class AuthService
    {
        static HttpClient _client = new HttpClient();
        private string _token = string.Empty;

        public AuthService()
        {
            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiUrl"]);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }            
        }

        public async Task<AuthenticateResponse> CheckAuth(AuthenticateRequest model)
        {
            try
            {
                string url = _client.BaseAddress + "Authenticate/login";
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
                if (response.IsSuccessStatusCode)
                {
                   return  await response.Content.ReadFromJsonAsync<AuthenticateResponse>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task UpdatePassword(int id, string password)
        {
            try
            {
                string url = _client.BaseAddress + "Authenticate/updatepassword";
                User user = new User { Id = id, Password = password };
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, user);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<User>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}