using DataRooms.UI.Areas.Explorer.Model;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using NLog;
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
    public class DataCacheLoadService
    {
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        static HttpClient _client = new HttpClient();
        string secretkey = ConfigurationManager.AppSettings["SecretKey"];
        public DataCacheLoadService()
        {
            if (_client.BaseAddress == null)
            {
                _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiUrl"]);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                _client.Timeout = TimeSpan.FromMinutes(30);
            }
        }

        public IEnumerable<Company> GetCompaniesAsync()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getcompanies";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Company>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<User> GetUsersAsync()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getusers";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<User>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<User>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<RoleMaster> GetRolesAsync()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getroles";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<RoleMaster>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<RoleMaster>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<UserRoleMapping> GetUserRoleMappingsAsync()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getuserrolemappings";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<UserRoleMapping>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<UserRoleMapping>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<PermissionMaster> GetPermissions()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getpermissions";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<PermissionMaster>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<PermissionMaster>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<DataRoomPermission> GetDataRoomPermissions()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getdataroompermissions";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<DataRoomPermission>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<DataRoomPermission>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<FolderPermission> GetFolderPermissions()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getfolderpermissions";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<FolderPermission>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<FolderPermission>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<FilePermission> GetFilePrmissions()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getfilepermissions";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<FilePermission>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<FilePermission>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<DataRoom> GetDataRooms()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getdatarooms";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<DataRoom>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<DataRoom>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<Folder> GetFolders()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getfolders";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Folder>>(responseString);
                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = await _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return await response.Content.ReadFromJsonAsync<IEnumerable<Folder>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task<IEnumerable<File>> GetFiles()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getfiles";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.GetAsync(url);
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

        public async Task UpdateFiles(IEnumerable<File> files)
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/updatefiles";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, files);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<IEnumerable<File>>();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteFiles(IEnumerable<File> files)
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/deletefiles";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, files);
                if (response.IsSuccessStatusCode)
                {
                    //return await response.Content.ReadFromJsonAsync<IEnumerable<File>>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<File> GetFilesusingWebClient()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getfiles";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<File>>(responseString);// JsonConvert.<IEnumerable<File>>();


                //_client.DefaultRequestHeaders.Clear();
                //HttpResponseMessage response = _client.GetAsync(url);
                //if (response.IsSuccessStatusCode)
                //{
                //    return response.Content.ReadFromJsonAsync<IEnumerable<File>>();
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<WorkFlowMaster> GetWorkFlowsusingWebClient()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getworkflows";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<WorkFlowMaster>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<DataRoomWorkFlowUser> GetDataRoomWorkFlowUsersusingWebClient()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getworkflowusers";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<DataRoomWorkFlowUser>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<ToDoTask> GetToDoTasksusingWebClient()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/gettodotasks";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ToDoTask>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public Setting GetSettingusingWebClient()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getsettings";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Setting>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public HostDetails GetHostInformation()
        {
            try
            {
                string url = _client.BaseAddress + "Activation/gethostdetails";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                var hostDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<HostDetails>(responseString);
                hostDetails.HostName = EncryptionHelper.Decrypt(hostDetails.HostName, secretkey);
                hostDetails.IpAddress = EncryptionHelper.Decrypt(hostDetails.IpAddress, secretkey);
                hostDetails.EmailId = EncryptionHelper.Decrypt(hostDetails.EmailId, secretkey);

                return hostDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public LicenseInfo GetAvailableModules()
        {
            try
            {
                string url = _client.BaseAddress + "activation/getlicenseinfo";               
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                var license = Newtonsoft.Json.JsonConvert.DeserializeObject<LicenseInfo>(responseString);
                license.Column1 = EncryptionHelper.Decrypt(license.Column1, secretkey);
                license.Column2 = EncryptionHelper.Decrypt(license.Column2, secretkey);
                license.Column3 = EncryptionHelper.Decrypt(license.Column3, secretkey);
                license.Column4 = EncryptionHelper.Decrypt(license.Column4, secretkey);
                license.Column5 = EncryptionHelper.Decrypt(license.Column5, secretkey);
                return license;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public List<EmailConfiguration> GetEmailConfigurations()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getemailconfigs";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmailConfiguration>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public List<ADInfo> GetADInfo()
        {
            List<ADInfo> adInfos = new List<ADInfo>();
            try
            {
                string url = _client.BaseAddress + "datacacheload/getadinfo";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                var adinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ADInfo>>(responseString);
                if(adinfo != null && adinfo.Count > 0)
                {
                    foreach(var item in adinfo)
                    {
                        ADInfo info = new ADInfo();
                        info.Id = item.Id;
                        info.CompanyId = item.CompanyId;
                        info.IsADSync = EncryptionHelper.Decrypt(item.IsADSync, secretkey);
                        info.DomainName = EncryptionHelper.Decrypt(item.DomainName, secretkey);
                        info.IPAddress = EncryptionHelper.Decrypt(item.IPAddress, secretkey);
                        info.CompanyName = EncryptionHelper.Decrypt(item.CompanyName, secretkey);
                        adInfos.Add(info);
                    }
                }
                return adInfos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public async Task SyncADUsers(IEnumerable<User> users)
        {
            try
            {
                string url = _client.BaseAddress + "DataCacheLoad/syncadusers";
                _client.DefaultRequestHeaders.Clear();
                HttpResponseMessage response = await _client.PostAsJsonAsync(url, users);
                if (response.IsSuccessStatusCode)
                {
                    //await response.Content.ReadFromJsonAsync<string>();
                }
            }
            catch (Exception ex)
            {         
                logger.Error("Error in DataCacheLoadService--" + ex.Message);
                throw ex;
            }
        }

        public List<ItemTrackerControl> GetItemTrackerControls()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getitemtrackercontrols";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemTrackerControl>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public List<ItemTrackerData> GetItemTrackerData()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getitemtrackerdata";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemTrackerData>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public List<ItemTrackerMetaData> GetItemTrackerMetaData()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getitemtrackermetadata";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemTrackerMetaData>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public List<ItemTrackerPermission> GetItemTrackerPermissions()
        {
            try
            {
                string url = _client.BaseAddress + "datacacheload/getitemtrackerpermissions";
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemTrackerPermission>>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
    }
}