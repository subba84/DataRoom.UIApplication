using System.Web.Mvc;

namespace DataRooms.UI.Areas.Explorer
{
    public class ExplorerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Explorer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "allfoldersbyroomid", new { action = "GetAllFolders", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "explorer", new { action = "Index", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "explorer/getfoldercontent", new { action = "GetFolderContent", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "explorer/getpagedfoldercontent", new { action = "GetPagedFoldersandPagedFiles", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "explorer/gettreeviewforcopyandmove", new { action = "GetFoldersforCopyandMove", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "explorer/getfoldersbasedonparentid", new { action = "GetFoldersbasedonParentId", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "file/copy", new { action = "CopyFile", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "file/move", new { action = "MoveFile", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "folder/copy", new { action = "CopyFolder", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "folder/move", new { action = "MoveFolder", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "testlayout", new { action = "TestLayout", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "getfileversions", new { action = "GetFileVersions", controller = "FileExplorer", id = UrlParameter.Optional });
            context.MapRoute(null, "downloadfolder", new { action = "DownloadFolder", controller = "FileExplorer", id = UrlParameter.Optional });
            
        }
    }
}