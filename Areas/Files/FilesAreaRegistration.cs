using System.Web.Mvc;

namespace DataRooms.UI.Areas.Files
{
    public class FilesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Files";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "file/download", new { action = "DownloadFile", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/downloadfileversion", new { action = "DownloadFileVersion", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/edit", new { action = "Edit", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/list", new { action = "GetFilesbyRoomandFolder", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/delete", new { action = "Delete", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "filepermission/list", new { action = "List", controller = "ManageFilePermission", id = UrlParameter.Optional });
            context.MapRoute(null, "filepermission/edit", new { action = "Edit", controller = "ManageFilePermission", id = UrlParameter.Optional });
            context.MapRoute(null, "filepermission/delete", new { action = "Delete", controller = "ManageFilePermission", id = UrlParameter.Optional });
            context.MapRoute(null, "filepermission/getfilesbasedondataroomandfolder", new { action = "GetFilesbasedonDataRoomandFolder", controller = "ManageFilePermission", id = UrlParameter.Optional });
            context.MapRoute(null, "file/checkout", new { action = "CheckOutFile", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/checkin", new { action = "CheckInFile", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/savefileintemp", new { action = "SaveFilesinTemp", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/permissionlist", new { action = "GetFilePermissions", controller = "ManageFilePermission", id = UrlParameter.Optional });
            context.MapRoute(null, "file/viewfile", new { action = "ViewFile", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/editfilechanges", new { action = "EditFileforSubmission", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/checkfilecontent", new { action = "CheckforFileContent", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/checkfileversioncontent", new { action = "CheckforFileVersionContent", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/checkuploadedflecount", new { action = "GetUploadedFilesCount", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/sharefile", new { action = "ShareFile", controller = "ManageFile", id = UrlParameter.Optional });
            
            context.MapRoute(null, "externalfiledownload", new { action = "DownloadFile", controller = "ExternalFileDownload", id = UrlParameter.Optional });

            context.MapRoute(null, "savefiletoserver", new { action = "SaveFiletoServer", controller = "ManageFile", id = UrlParameter.Optional });
            context.MapRoute(null, "file/checkworkflowusers", new { action = "CheckforWorkFlowUsers", controller = "ManageFile", id = UrlParameter.Optional });
        }
    }
}