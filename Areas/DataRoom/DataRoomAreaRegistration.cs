using System.Web.Mvc;

namespace DataRooms.UI.Areas.DataRoom
{
    public class DataRoomAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DataRoom";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "datarooms/getuserdatarooms", new { action = "List", controller = "ManageUserDataRoom", id = UrlParameter.Optional });
            context.MapRoute(null, "datarooms/getalldatarooms", new { action = "GetAllDataRooms", controller = "ManageDataRoom", id = UrlParameter.Optional });
            context.MapRoute(null, "datarooms/list", new { action = "List", controller = "ManageDataRoom", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroom/edt", new { action = "Edit", controller = "ManageDataRoom", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroom/delete", new { action = "Delete", controller = "ManageDataRoom", id = UrlParameter.Optional });

            context.MapRoute(null, "dataroompermissions/list", new { action = "List", controller = "ManageDataRoomPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroompermission/edt", new { action = "Edit", controller = "ManageDataRoomPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroompermission/delete", new { action = "Delete", controller = "ManageDataRoomPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroompermission/getdataroompermissions", new { action = "GetDataRoomPermissions", controller = "ManageDataRoomPermission", id = UrlParameter.Optional });

            
            context.MapRoute(null, "dataroomworkflowusers", new { action = "GetDataRoomWorkFlowUsers", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroom/workflowusers", new { action = "GetUsersBasedonDataRoomforWorkFlow", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroomworkflowusers/edit", new { action = "Edit", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroomworkflowusers/delete", new { action = "Delete", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });
            context.MapRoute(null, "checkforInitiatorRole", new { action = "CheckforInitiatorRole", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });
            context.MapRoute(null, "dataroomworkflows", new { action = "GetWorkFlowsbyDataRoomId", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });
            context.MapRoute(null, "savedataroomworkflows", new { action = "SaveDataRoomWorkFlow", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });


            context.MapRoute(null, "deletedataroomworkflow", new { action = "DeleteDataRoomWorkFlow", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });
            context.MapRoute(null, "editworkflowpart", new { action = "EditWorkFlow", controller = "ManageDataRoomWorkFlowUser", id = UrlParameter.Optional });

            context.MapRoute(null, "saveitemtrackercontrol", new { action = "SaveItemTrackerControl", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "getitemtrackercontrol", new { action = "GetItemTrackerControlsbasedonDataRoomFolderTracker", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "deleteitemtrackercontrol", new { action = "DeleteItemTrackerControl", controller = "ManageItemTracker", id = UrlParameter.Optional });

            context.MapRoute(null, "saveitemtrackerdata", new { action = "SaveItemTrackerData", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "getitemtrackerdata", new { action = "GetItemTrackerDatabasedonDataRoom", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "getitemtrackerform", new { action = "GetItemTrackerEditFormPartbasedonDataRoom", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "getmasterdatabasedonparent", new { action = "GetMasterDatabasedonParent", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "getitemtrackerdatalistpart", new { action = "GetItemTrackerDatabasedonDataRoomFolderItemTracker", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "deleteitemtrackerdata", new { action = "DeleteItemTrackerData", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "deleteitemtracker", new { action = "DeleteItemTracker", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "deleteitemtrackerpermission", new { action = "DeleteItemTrackerPermission", controller = "ManageItemTrackerPermission", id = UrlParameter.Optional });

            context.MapRoute(null, "saveitemtracker", new { action = "CreateItemTrackerMetaData", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "downloaditemtrackerfile", new { action = "DownloadItemTrackerFile", controller = "ManageItemTracker", id = UrlParameter.Optional });

            context.MapRoute(null, "edititemtrackerpermission", new { action = "EditItemTrackerPermission", controller = "ManageItemTrackerPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "saveitemtrackerpermission", new { action = "SaveItemTrackerPermission", controller = "ManageItemTrackerPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "getitemtrackerpermission", new { action = "GetItemtrackerPermissions", controller = "ManageItemTrackerPermission", id = UrlParameter.Optional });
            context.MapRoute(null, "getitemtrackerhistory", new { action = "GetItemTrackerHistory", controller = "ManageItemTracker", id = UrlParameter.Optional });
            context.MapRoute(null, "downloaditemtrackerdata", new { action = "DownloadItemTrackerData", controller = "ManageItemTracker", id = UrlParameter.Optional });

            context.MapRoute(null, "createitemtracker", new { action = "ItemTrackerCreation", controller = "ManageItemTracker", id = UrlParameter.Optional });
        }
    }
}