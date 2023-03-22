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
    public class FilePermissionService
    {
        static HttpClient _client = new HttpClient();
        private string _authToken = string.Empty;

        public FilePermissionService(string token)
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

        public async Task<int> SaveFilePermission(FilePermission filepermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFilePermission/savefilepermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, filepermission);
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

        public async Task UpdateFilePermission(FilePermission filepermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFilePermission/updatefilepermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PutAsJsonAsync(url, filepermission);
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

        public async Task DeleteFilePermission(FilePermission filePermission)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFilePermission/deletefilepermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, filePermission);
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

        public async Task DeleteRangeFilePermission(IEnumerable<FilePermission> filePermissions)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFilePermission/deleterangefilepermission";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, filePermissions);
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

        public async Task SaveFilePermissions(List<FilePermission> filePermissions)
        {
            try
            {
                string url = _client.BaseAddress + "ManageFilePermission/savefilepermissions";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, filePermissions);
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