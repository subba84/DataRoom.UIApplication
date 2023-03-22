using DataRooms.UI.Areas.DataRoom.Models;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.DataRoom.Controllers
{
    [SessionExpire]
    public class ManageUserDataRoomController : Controller
    {
        public ManageUserDataRoomController()
        {
        }

        [HttpGet]
        public ActionResult List()
        {
            var model = new DataRoomCustomModel();
            model.PermissionCategory = "All";
            GetAllDataRoomsbyUser(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataRoomCustomModel model)
        {
            GetAllDataRoomsbyUser(model);
            return View(model);
        }

        public void GetAllDataRoomsbyUser(DataRoomCustomModel model)
        {
            try
            {
                model.SortColumn = string.IsNullOrEmpty(model.SortColumn) ? "CreatedOn" : model.SortColumn;
                int userId = Convert.ToInt32(Session["UserId"]);
                List<DataRooms.UI.Models.DataRoomPermission> datarooms = new List<DataRoomPermission>();
                IEnumerable<DataRoomPermission> permissions = null;
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    model.CurrentPage = 1;
                }
                permissions = DataCache.DataRoomPermissions.Where(x => x.UserId == userId);
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    permissions = permissions.Where(x => x.DataRoomName.ToLower().Contains(model.SearchString.ToLower()));
                }
                var activeDataRooms = new List<DataRoomPermission>();
                if(permissions!=null && permissions.Count() > 0)
                {
                    foreach(var permission in permissions)
                    {
                        var dataroom = DataCache.DataRooms.Where(x => x.Id == permission.DataRoomId && x.IsActive == true);
                        if(dataroom!=null && dataroom.Count() > 0)
                        {
                            datarooms.Add(permission);
                        }
                    }
                }

                //datarooms = permissions.OrderBy(m => m.DataRoomName);
                //permissions = DataCache.DataRoomPermissions.Where(x => x.UserId == userId).OrderByDescending
                //            (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //if (!string.IsNullOrEmpty(model.SearchString))
                //{
                //    permissions = permissions.Where(x => x.DataRoomName.ToLower().Contains(model.SearchString.ToLower())).OrderByDescending
                //            (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //}
                //datarooms = permissions.OrderByDescending
                //                    (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //switch (model.PermissionCategory)
                //{
                //    case "FullControl":
                //        permissions = DataCache.DataRoomPermissions.Where(x => x.UserId == userId && x.HasFullControl).OrderByDescending
                //            (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //    case "Read":
                //        permissions = DataCache.DataRoomPermissions.Where(x => x.UserId == userId && x.HasRead).OrderByDescending
                //            (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //    case "Write":
                //        permissions = DataCache.DataRoomPermissions.Where(x => x.UserId == userId && x.HasWrite).OrderByDescending
                //            (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //    case "Delete":
                //        permissions = DataCache.DataRoomPermissions.Where(x => x.UserId == userId && x.HasDelete).OrderByDescending
                //            (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //    case "All":
                //        permissions = DataCache.DataRoomPermissions.Where(x => x.UserId == userId).OrderByDescending
                //            (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //}

                //switch (model.SortColumn)
                //{
                //    case "CreatedOn":
                //        if (model.SortOrder.Equals("desc"))
                //            datarooms = permissions.OrderByDescending
                //                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        else
                //            datarooms = permissions.OrderBy
                //                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //    case "DataRoomName":
                //        if (model.SortOrder.Equals("desc"))
                //            datarooms = permissions.OrderByDescending
                //                    (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        else
                //            datarooms = permissions.OrderBy
                //                    (m => m.DataRoomName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //    case "UserName":
                //        if (model.SortOrder.Equals("desc"))
                //            datarooms = permissions.OrderByDescending
                //                    (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        else
                //            datarooms = permissions.OrderBy
                //                    (m => m.CreatorName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //    case "Default":
                //        datarooms = permissions.OrderByDescending
                //            (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                //        break;
                //}

                model.DataRoomPermissions = datarooms;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}