using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI
{
    public class LogService
    {
        public string _authToke;
        public LoggerApiService _service;
        public XMLLogService _xmlLogService;
        int companyId = 0;
        public LogService(string authToken)
        {
            _authToke = authToken;
            _service = new LoggerApiService(_authToke);
            if(System.Web.HttpContext.Current!=null)
                 companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
            if(companyId > 0)
            {
                var company = DataCache.Companies.First(x => x.Id == companyId);
                _xmlLogService = new XMLLogService(company.LogsStoragePath);
            }
        }
        public async Task<int> LogActivity(int companyid,string activitycategory, string activitydescription, int? dataroomid = null, int? folderid = null, int? fileid = null, string dataroomname = null, string foldername = null, string filename = null)
        {
            var isLogRequired = false;var isLogsRequiredforDataRoom = false;
            if(companyid > 0)
            {
                var company = DataCache.Companies.First(x => x.Id == companyid);
                if(company != null && company.Id > 0)
                {
                    if (_xmlLogService == null)
                        _xmlLogService = new XMLLogService(company.LogsStoragePath);
                    if (company.IsLogsRequired == true)
                    {
                        isLogRequired = true;
                    }
                }
            }
            if(dataroomid != null && dataroomid > 0)
            {
                var dataroom = DataCache.DataRooms.First(x => x.Id == dataroomid);                
                if (dataroom.IsLogsRequired == true)
                {
                    isLogsRequiredforDataRoom = true;
                }
            }
            else
            {
                isLogsRequiredforDataRoom = true;
            }
            if(isLogRequired == true && isLogsRequiredforDataRoom == true)
            {
                try
                {
                    ActivityLog log = new ActivityLog
                    {
                        ActivityCategory = activitycategory,
                        ActivityDescription = activitydescription,
                        DataRoomId = dataroomid,
                        DataRoomName = dataroomname,
                        FolderId = folderid,
                        FolderName = foldername,
                        FileId = fileid,
                        FileName = filename,
                        IsActive = true,
                        CreatedBy = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]),
                        CreatorName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]),
                        CreatedOn = DateTime.Now
                    };
                    return await _xmlLogService.CreateActivityLog(log);
                    //return await _service.SaveActivityLog(log);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return 0;
            }
        }

        public async Task LogDataDifference<T>(int? activitylogid, T original, T modified, int dataRoomId = 0)
        {
            try
            {
                if(activitylogid > 0)
                {
                    List<Variance> variances = original.DetailedCompare(modified);
                    if (variances != null && variances.Count() > 0)
                    {
                        DataLog log = new DataLog
                        {
                            ActivityLogId = (int)activitylogid,
                            DataRoomId = dataRoomId,
                            TableName = typeof(T).Name,
                            OriginalData = JsonConvert.SerializeObject(original),
                            ModifiedData = JsonConvert.SerializeObject(modified),
                            Changes = JsonConvert.SerializeObject(variances),
                            IsActive = true,
                            CreatedBy = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]),
                            CreatorName = Convert.ToString(System.Web.HttpContext.Current.Session["UserName"]),
                            CreatedOn = DateTime.Now
                        };
                        await _xmlLogService.CreateDataLog(log);
                        //await _service.SaveDataLog(log);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}