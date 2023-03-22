using DataRooms.UI.Models;
using Newtonsoft.Json;
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
    public class FileService
    {
        static HttpClient _client = new HttpClient();
        private string _authToken = string.Empty;

        public FileService(string token)
        {
            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiUrl"]);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                _client.Timeout = TimeSpan.FromMinutes(30);
            }
            _authToken = token;
        }

        public async Task<IEnumerable<File>> GetFiles(FilterModel model)
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getfiles";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<File>>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<string> GetWaitingWith(int fileid)
        {
            try
            {
                string url = _client.BaseAddress + "managefile/getwaitingwith/" + fileid;
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<int> UploadFile(File file)
        {
            try
            {
                string url = _client.BaseAddress + "managefile/uploadfile";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, file);
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

        public async Task<int> SaveFile(File file)
        {
            try
            {
                string url = _client.BaseAddress + "managefile/savefile";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, file);
                if (response.IsSuccessStatusCode)
                {
                    int responseid = await response.Content.ReadFromJsonAsync<int>();
                    return responseid;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public async Task UpdateFile(File file)
        {
            try
            {
                string url = _client.BaseAddress + "managefile/updatefile";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PutAsJsonAsync(url, file);
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

        public async Task DeleteFile(File file)
        {
            try
            {
                string url = _client.BaseAddress + "managefile/deletefile";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, file);
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

        public async Task SaveFileVersion(FileVersion file)
        {
            try
            {
                string url = _client.BaseAddress + "managefile/savefileversion";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PutAsJsonAsync(url, file);
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

        public async Task<IEnumerable<FileVersion>> GetFileVersions(int fileid)
        {
            try
            {
                string url = _client.BaseAddress + "managefile/getfileversions/" + fileid;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<FileVersion>>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<File> GetIndividualFile(int fileid)
        {
            try
            {
                string url = _client.BaseAddress + "managefile/getindividualfile/" + fileid;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<File>();
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