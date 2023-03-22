using DataRooms.UI.Areas.Explorer.Model;
using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.WebApiHelpers
{
    public class ItemTrackerService
    {
        static HttpClient _client = new HttpClient();
        private string _authToken = string.Empty;

        public ItemTrackerService(string token)
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

        public async Task<int> SaveItemTrackerControl(ItemTrackerControl itemTrackerControl)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/createconfigcontrol";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerControl);
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

        public async Task<int> UpdateItemTrackerControl(ItemTrackerControl itemTrackerControl)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/updateconfigcontrol";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerControl);
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

        public async Task DeleteItemTrackerControl(ItemTrackerControl itemTrackerControl)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/deleteconfigcontrol";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerControl);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SaveItemTrackerData(ItemTrackerData itemTrackerData)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/createitemtrackerdata";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerData);
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

        public async Task<int> UpdateItemTrackerData(ItemTrackerData itemTrackerData)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/updateitemtrackerdata";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerData);
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

        public async Task DeleteItemTrackerData(ItemTrackerData itemTrackerData)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/deleteitemtrackerdata";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerData);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SaveItemTracker(ItemTrackerMetaData itemTracker)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/createitemtracker";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTracker);
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

        public async Task<int> UpdateItemTracker(ItemTrackerMetaData itemTracker)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/updateitemtracker";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTracker);
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

        public async Task DeleteItemTracker(ItemTrackerMetaData itemTracker)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/deleteitemtracker";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTracker);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SaveItemTrackerPermission(ItemTrackerPermission itemTrackerPermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/saveitemtrackerpermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerPermission);
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

        public async Task<int> UpdateItemTrackerPermission(ItemTrackerPermission itemTrackerPermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/updateitemtrackerpermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerPermission);
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

        public async Task DeleteItemTrackerPermission(ItemTrackerPermission itemTrackerPermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/deleteitemtrackerpermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerPermission);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveItemTrackerHistory(ItemTrackerHistory itemTrackerHistory)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/saveitemtrackerhistory";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, itemTrackerHistory);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<int>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return 0;
        }

        public async Task<IEnumerable<ItemTrackerHistory>> GetItemTrackerHistory(int itemtrackerid,string rowguid)
        {
            try
            {
                string url = _client.BaseAddress + "ManageItemTracker/getitemtrackerhistory/" + itemtrackerid + "/" + rowguid;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ItemTrackerHistory>>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
    }
}