using DataRooms.UI.Areas.Admin.Model;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Admin.Controllers
{
    [SessionExpire]
    public class FileCheckInController : Controller
    {
        public LogService _logger { get; set; }
        private FileService _fileService { get; set; }

        public FileCheckInController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _logger = new LogService(token);
            _fileService = new FileService(token);
        }

        [HttpGet]
        public ActionResult List()
        {
            var model = new FileCheckInModel();
            PrepareList(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult List(FileCheckInModel model)
        {
            PrepareList(model);
            return View(model);
        }

        public void PrepareList(FileCheckInModel model)
        {
            IEnumerable<File> files = null;
            if (!string.IsNullOrEmpty(model.SearchString))
            {
                files = DataCache.Files.Where(x => x.IsActive == true && x.IsCheckOut == true && (x.FileName.ToLower().Contains(model.SearchString.ToLower())
                || x.FolderName.ToLower().Contains(model.SearchString.ToLower())
                || x.DataRoomName.ToLower().Contains(model.SearchString.ToLower())));
            }
            else
            {
                files = DataCache.Files.Where(x => x.IsActive == true && x.IsCheckOut == true);
            }
            IPagedList<File> pagedFiles = files.ToPagedList(model.CurrentPage, model.RecordsPerPage);
            model.PagedCheckedInFiles = pagedFiles;
        }

        public async Task<ActionResult> CheckInFile(int id)
        {
            try
            {
                int activityLogId = 0;
                var originalfile = DataCache.Files.Single(x => x.Id == id);
                var modifiedfile = originalfile.Clone();
                modifiedfile.IsCheckOut = null;
                modifiedfile.CheckOutBy = null;
                modifiedfile.CheckOutByName = null;
                modifiedfile.CheckOutOn = null;
                modifiedfile.IsCheckIn = true;
                modifiedfile.CheckInBy = Convert.ToInt32(Session["UserId"]);
                modifiedfile.CheckInByName = Convert.ToString(Session["UserName"]);
                modifiedfile.CheckInOn = DateTime.Now;
                modifiedfile.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                modifiedfile.ModifierName = Convert.ToString(Session["UserName"]);
                modifiedfile.ModifiedOn = DateTime.Now;
                await _fileService.UpdateFile(modifiedfile);
                new Thread(() => DataCache.RefreshSingleFile(modifiedfile)).Start();
                activityLogId = await _logger.LogActivity(modifiedfile.CompanyId,"File Check-In", "File - " + modifiedfile.FileName + " check-in into the system by " + modifiedfile.ModifierName + " (Admin)", dataroomid: modifiedfile.DataRoomId, folderid: modifiedfile.FolderId, fileid: modifiedfile.Id, dataroomname: modifiedfile.DataRoomName, foldername: modifiedfile.FolderName, filename: modifiedfile.FileName);
                await _logger.LogDataDifference(activityLogId, originalfile, modifiedfile, modifiedfile.DataRoomId);
                TempData["Notification"] = "File Check-In Cancelled Successfully";
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("List");
        }
    }
}