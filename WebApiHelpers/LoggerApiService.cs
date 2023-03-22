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
    public class LoggerApiService
    {
        static HttpClient _client = new HttpClient();
        private string _authToken = string.Empty;

        public LoggerApiService(string token)
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

        public async Task<IEnumerable<ActivityLog>> GetActivityLogs(string sql)
        {
            try
            {
                string url = _client.BaseAddress + "logger/activitylogs/" + sql;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ActivityLog>>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<ActivityLog> GetActivityLog(int id)
        {
            try
            {
                string url = _client.BaseAddress + "logger/activitylog/" + id;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ActivityLog>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<int> SaveActivityLog(ActivityLog log)
        {
            try
            {
                string url = _client.BaseAddress + "logger/saveactivity";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, log);
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

        public async Task<int> SaveDataLog(DataLog log)
        {
            try
            {
                string url = _client.BaseAddress + "logger/savedatalog";
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, log);
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

        public async Task<IEnumerable<DataLog>> GetDataLogs(int activityid, string searchString)
        {
            try
            {
                string url = _client.BaseAddress + "logger/datalogs/" + activityid;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<DataLog>>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        

        //public string FormatLogs(DataLog log)
        //{
        //    try
        //    {
        //        System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(@"~/Models");
        //        System.IO.FileInfo[] Files = d.GetFiles("*.cs");
        //        foreach (System.IO.FileInfo file in Files)
        //        {
        //            if(file.Name == log.TableName)
        //            {
        //                Type t = Type.GetType(log.TableName);
        //                object myobject = t.InvokeMember(log.TableName, System.Reflection.BindingFlags.CreateInstance, null, null,null);
        //                var originalData = JsonConvert.DeserializeObject<myobject>(log.OriginalData);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}