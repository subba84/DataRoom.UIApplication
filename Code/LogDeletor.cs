using DataRooms.UI.Code.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.Code
{
    public class LogDeletor
    {
        //public async Task DeleteLogs()
        //{
        //    try
        //    {
        //        var logspath = DataCache.Settings.LogsPath;
        //        XMLLogService xMLLog = new XMLLogService();
        //        DateTime currentDate = DateTime.Now.Date;
        //        var datarooms = DataCache.DataRooms.Where(x => x.IsLogsRequired == true);
        //        List<int> activityids = new List<int>();
        //        if (datarooms != null && datarooms.Count() > 0)
        //        {
        //            foreach (var dataroom in datarooms.ToList())
        //            {
        //                int dataroomRetentionPeriod = Convert.ToInt32(dataroom.RetentionPeriod);
        //                var logs = xMLLog.GetActivityLogs();
        //                if(logs!=null && logs.Count() > 0)
        //                {
        //                    foreach(var log in logs)
        //                    {
        //                        var creationDate = log.CreatedOn.Date;
        //                        if ((currentDate - creationDate).TotalDays >= dataroomRetentionPeriod)
        //                        {
        //                            activityids.Add(log.Id);
        //                        }
        //                    }
        //                }
                        
        //            }
        //        }
        //        await xMLLog.DeleteLogsofDataRoombyActivityIds(activityids);
        //    }
        //    catch (Exception ex)
        //    {
        //        //throw ex;
        //    }
        //}
    }
}