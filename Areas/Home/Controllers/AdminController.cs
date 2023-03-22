using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Home.Controllers
{
    [SessionExpire]
    public class AdminController : Controller
    {
        public bool isFileCachingEnabled = false;
        public string FileCacheEnabled = "";
        public AdminController()
        {
            FileCacheEnabled = ConfigurationManager.AppSettings["IsFileCacheEnabled"];
            if (FileCacheEnabled == "Y")
            {
                isFileCachingEnabled = true;
            }
        }

        public ActionResult AdminDashboard()
        {
            return View();
        }

        public JsonResult GetDataRoomsforSizeChart()
        {
            try
            {
                List<DataRooms.UI.Models.DataRoom> rooms = new List<Models.DataRoom>();
                IEnumerable<DataRooms.UI.Models.DataRoom> datarooms = DataCache.DataRooms;
                if(datarooms!=null && datarooms.Count() > 0)
                {
                    foreach(var dataroom in datarooms)
                    {
                        double dataroomsize = string.IsNullOrEmpty(dataroom.DataRoomSize) ? 0 : Convert.ToDouble(dataroom.DataRoomSize);
                        //dataroom.DataRoomSize = Convert.ToString(Math.Round(dataroomsize.ConvertSize("MB"),2));
                        rooms.Add(new Models.DataRoom { DataRoomName = dataroom.DataRoomName, DataRoomSize = dataroomsize.ConvertSize("GB") });
                    }
                }
                return Json(rooms, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetLastModifiedFilesbyUser()
        {
            try
            {
                List<Models.File> filesList = new List<File>();
                IEnumerable<DataRooms.UI.Models.File> files = DataCache.Files.Where(x => x.IsArchived == false);
                IEnumerable<ItemTrackerMetaData> itemTrackers = DataCache.ItemTrackerMetaData.Where(x => x.IsActive == true);
                if (files != null && files.Count() > 0)
                {
                    filesList = files.OrderByDescending(x => x.CreatedOn).Take(10).ToList();//.OrderByDescending(x => x.CreatedOn).ToList();
                }
                if(itemTrackers!=null && itemTrackers.Count() > 0)
                {
                    itemTrackers = itemTrackers.OrderByDescending(x => x.CreatedOn).Take(10).ToList();
                    foreach (var item in itemTrackers)
                    {
                        Models.File file = new Models.File();
                        file.CreatedOn = item.CreatedOn;
                        file.FileName = item.ItemTrackerName;
                        file.CreatorName = item.CreatorName;
                        file.DataRoomName = item.DataRoomName;
                        filesList.Add(file);
                    }
                    filesList = filesList.OrderByDescending(x => x.CreatedOn).Take(10).ToList();
                }

                
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~/Areas/Home/Views/Shared/_lastuploadedfiles.cshtml", filesList)
                });
                jr.MaxJsonLength = int.MaxValue;
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}