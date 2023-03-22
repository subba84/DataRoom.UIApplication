//using DataRooms.UI.Models;
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
    public class DataRoomService
    {
        static HttpClient _client = new HttpClient();
        private string _authToken = string.Empty;

        public DataRoomService(string token)
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

        public async Task<int> SaveDataRoom(Models.DataRoom dataroom)
        {
            try
            {
                string url = _client.BaseAddress + "ManageDataroom/savedataroom";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, dataroom);
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

        public async Task UpdateDataRoom(Models.DataRoom user)
        {
            try
            {
                string url = _client.BaseAddress + "managedataroom/updatedataroom";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PutAsJsonAsync(url, user);
                if (response.IsSuccessStatusCode)
                {
                    //await response.Content.ReadFromJsonAsync<string>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteDataRoom(Models.DataRoom dataroom)
        {
            try
            {
                string url = _client.BaseAddress + "managedataroom/deletedataroom";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, dataroom);
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