using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DataRooms.UI.WebApiHelpers
{
    public class UserRoleMappingService
    {
        static HttpClient _client = new HttpClient();
        private string _authToken = string.Empty;

        public UserRoleMappingService(string token)
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

        public async Task<int> AddUserRoleMappingAsync(List<UserRoleMapping> userrolemappings)
        {
            try
            {
                string url = _client.BaseAddress + "manageuserrolemapping/saveuserrole";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, userrolemappings);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public async Task<int> UpdateUserRoleMappingAsync(UserRoleMapping userrolemapping)
        {
            try
            {
                string url = _client.BaseAddress + "manageuserrolemapping/updateuserrole";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, userrolemapping);
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

        public async Task<int> DeleteUserRoleMappingAsync(UserRoleMapping userrolemapping)
        {
            try
            {
                string url = _client.BaseAddress + "manageuserrolemapping/deleteuserrole";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, userrolemapping);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<int>();
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
