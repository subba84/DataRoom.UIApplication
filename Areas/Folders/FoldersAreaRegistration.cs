using System.Web.Mvc;

namespace DataRooms.UI.Areas.Folders
{
    public class FoldersAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Folders";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "folder/edit", new { action = "Edit", controller = "ManageFolder", id = UrlParameter.Optional });
            context.MapRoute(null, "folder/delete", new { action = "Delete", controller = "ManageFolder", id = UrlParameter.Optional });
            context.MapRoute(null, "folderpermission/getdatarooms", new { action = "GetDataRooms", controller = "ManageFolderPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "folderpermission/getfolders", new { action = "GetFoldersbasedonDataRoom", controller = "ManageFolderPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "folderpermission/getusers", new { action = "GetUsersBasedonDataRoom", controller = "ManageFolderPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "folderpermission/list", new { action = "List", controller = "ManageFolderPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "folderpermission/edit", new { action = "Edit", controller = "ManageFolderPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "folderpermission/delete", new { action = "Delete", controller = "ManageFolderPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "folderpermission/getfolderpermissions", new { action = "GetFolderPermissions", controller = "ManageFolderPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "folder/getworkflowsforfolder", new { action = "GetDataRoomWorkFlows", controller = "ManageFolder", id = UrlParameter.Optional });
            context.MapRoute(null, "folder/assignworkflowtofolder", new { action = "AssignWorkFlowtoFolder", controller = "ManageFolder", id = UrlParameter.Optional });
        }
    }
}