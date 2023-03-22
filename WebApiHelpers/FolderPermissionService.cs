using DataRooms.UI.Models;
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
    public class FolderPermissionService
    {
        static HttpClient _client = new HttpClient();
        private string _authToken = string.Empty;

        public FolderPermissionService(string token)
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

        public async Task<int> SaveFolderPermission(FolderPermission folderpermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFolderPermission/savefolderpermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, folderpermission);
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

        public async Task SaveFolderPermissions(List<FolderPermission> folderpermissions)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFolderPermission/savefolderpermissions";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, folderpermissions);
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

        public async Task UpdateFolderPermission(FolderPermission folderpermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFolderPermission/updatefolderpermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PutAsJsonAsync(url, folderpermission);
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

        public async Task DeleteFolderPermission(FolderPermission folderPermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFolderPermission/deletefolderpermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, folderPermission);
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

        public async Task DeleteRangeFolderPermissions(IEnumerable<FolderPermission> folderPermissions)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFolderPermission/deleterangefolderpermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, folderPermissions);
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