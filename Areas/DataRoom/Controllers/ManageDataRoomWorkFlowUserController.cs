using DataRooms.UI.Areas.DataRoom.Models;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.DataRoom.Controllers
{
    [SessionExpire]
    public class ManageDataRoomWorkFlowUserController : Controller
    {
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        public DataRoomWorkFlowUserService _service;
        public LogService _logger;
        public WorkFlowService _workflowservice;
        int userId;
        public ManageDataRoomWorkFlowUserController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new DataRoomWorkFlowUserService(token);
            _workflowservice = new WorkFlowService(token);
            _logger = new LogService(token);
            userId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var user = DataCache.DataRoomWorkFlowUsers.Single(x => x.Id == id);
                return Json(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetUsersBasedonDataRoomforWorkFlow(string searchString, int dataroomid)
        {
            try
            {
                var dataroomusers = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == dataroomid && x.IsActive == true).Select(x => x.UserId).ToList();
                int companyId = Convert.ToInt32(Session["CompanyId"]);
                var users = DataCache.Users.Where(x => dataroomusers.Contains(x.Id) &&  x.CompanyId == companyId && x.FullName.ToLower().Contains(searchString.ToLower()));
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult CheckforInitiatorRole(int folderid)
        {
            try
            {
                
                if (folderid != 0)
                {
                    var folder = DataCache.Folders.First(x => x.Id == folderid);                    
                    if (folder.WorkFlowId == 0)
                    {
                        return Json("True", JsonRequestBehavior.AllowGet);
                    }                    
                    // Recently Included Approver Role as well because approver can also add files...08/09/2022
                    var dataroomusers = DataCache.DataRoomWorkFlowUsers.Where(x => x.DataRoomId == folder.DataRoomId && x.WorkFlowId == folder.WorkFlowId && x.IsActive == true && (x.RoleId == AppRole.Initiator || x.RoleId == AppRole.Approver) && x.UserId == userId);                    
                    if (dataroomusers != null && dataroomusers.Count() > 0)
                    {
                        return Json("True", JsonRequestBehavior.AllowGet);
                    }
                    return Json("False", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("True", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(DataRooms.UI.Models.DataRoomWorkFlowUser dataRoomWorkFlowUser)
        {
            int activityid = 0;
            try
            {
                if (dataRoomWorkFlowUser.Id > 0)
                {
                    var original = DataCache.DataRoomWorkFlowUsers.Single(x => x.Id == dataRoomWorkFlowUser.Id);
                    dataRoomWorkFlowUser.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    dataRoomWorkFlowUser.ModifierName = Convert.ToString(Session["UserName"]);
                    dataRoomWorkFlowUser.ModifiedOn = DateTime.Now;
                    dataRoomWorkFlowUser.IsActive = dataRoomWorkFlowUser.IsActive;
                    await _service.UpdateDataRoomWorkFlowUser(dataRoomWorkFlowUser);
                    activityid = await _logger.LogActivity(dataRoomWorkFlowUser.CompanyId,"SharBox Work Flow User Creation", "User - " + dataRoomWorkFlowUser.UserName + " with role - " + dataRoomWorkFlowUser.RoleName + " have been modified", dataroomid: dataRoomWorkFlowUser.DataRoomId, dataroomname: dataRoomWorkFlowUser.DataRoomName);
                    await _logger.LogDataDifference(activityid, original, dataRoomWorkFlowUser, dataRoomId: original.Id);
                    DataCache.RefreshSingleDataRoomWorkFlowUser(dataRoomWorkFlowUser);
                    
                }
                else
                {
                    dataRoomWorkFlowUser.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    dataRoomWorkFlowUser.CreatorName = Convert.ToString(Session["UserName"]);
                    dataRoomWorkFlowUser.CreatedOn = DateTime.Now;
                    int id = await _service.SaveDataRoomWorkFlowUser(dataRoomWorkFlowUser);
                    dataRoomWorkFlowUser.Id = id;
                    activityid = await _logger.LogActivity(dataRoomWorkFlowUser.CompanyId,"SharBox Work Flow User Creation", "User - " + dataRoomWorkFlowUser.UserName + " with role - " + dataRoomWorkFlowUser.RoleName + " have been created", dataroomid: dataRoomWorkFlowUser.DataRoomId, dataroomname: dataRoomWorkFlowUser.DataRoomName);                   
                    DataCache.RefreshSingleDataRoomWorkFlowUser(dataRoomWorkFlowUser);                    
                }

                var dataroompermissions = GetIndividualDataRoomWorkFlowUsers(dataRoomWorkFlowUser.DataRoomId);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_singledataroomworkflowuserslist.cshtml", dataroompermissions)
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

        public async Task<ActionResult> Delete(int id)
        {
            int activityid = 0;
            try
            {
                var dataroomworkflowuser = DataCache.DataRoomWorkFlowUsers.Single(x => x.Id == id);
                await _service.DeleteDataRoomWorkFlowUser(dataroomworkflowuser);
                DataCache.RemoveDataRoomWorkFlowUserfromCache(dataroomworkflowuser);
                activityid = await _logger.LogActivity(dataroomworkflowuser.CompanyId, "SharBox Work Flow User Creation", "User - " + dataroomworkflowuser.UserName + " with role - " + dataroomworkflowuser.RoleName + " have been created", dataroomid: dataroomworkflowuser.DataRoomId, dataroomname: dataroomworkflowuser.DataRoomName);
                var dataroompermissions = GetIndividualDataRoomWorkFlowUsers(dataroomworkflowuser.DataRoomId);
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_singledataroomworkflowuserslist.cshtml", dataroompermissions)
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

        public JsonResult GetDataRoomWorkFlowUsers(int dataroomid)
        {
            var dataroom = DataCache.DataRooms.Single(x => x.Id == dataroomid);
            var users = GetIndividualDataRoomWorkFlowUsers(dataroomid);
            JsonResult jr = Json(new
            {
                DataRoom = dataroom,
                HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_dataroomworkflowuserslist.cshtml", users)
            });
            jr.MaxJsonLength = int.MaxValue;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }

        public IEnumerable<DataRoomWorkFlowUser> GetIndividualDataRoomWorkFlowUsers(int dataroomid)
        {
            try
            {
                var users = DataCache.DataRoomWorkFlowUsers.Where(x => x.DataRoomId == dataroomid);
                if (users != null && users.Count() > 0)
                {
                    return users.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<DataRoomWorkFlowUser>();
        }

        public JsonResult EditWorkFlow(int workflowid)
        {
            try
            {
                DataRoomCustomModel model = new DataRoomCustomModel();
                var workflow = DataCache.WorkFlows.Single(x => x.Id == workflowid);
                model.DataRoomId = workflow.DataRoomId;
                model.DataRoomName = workflow.DataRoomName;
                model.WorkFlow = workflow;
                logger.Debug("EditWorkFlow ---->  model.DataRoomId--" + model.DataRoomId + " workflow.Id--- " + workflow.Id);
                var workflowusers = DataCache.DataRoomWorkFlowUsers.Where(x => x.DataRoomId == model.DataRoomId && x.WorkFlowId == workflow.Id && x.IsActive == true);
                logger.Debug("workflowusers--" + workflowusers.Count());
                if (workflowusers!=null && workflowusers.Count() > 0)
                {
                    model.WorkFlowUsers = workflowusers.ToList();
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_editpartworkflow.cshtml", model)
                });
                jr.MaxJsonLength = int.MaxValue;
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jr;
            }
            catch(Exception ex)
            {
                logger.Error("Error in EditWorkFlow" + ex.Message + "---" + ex.StackTrace);
                throw ex;
            }
        }

        public ActionResult GetWorkFlowsbyDataRoomId(int dataroomid)
        {
            try
            {
                DataRoomCustomModel model = new DataRoomCustomModel();
                var dataroom = DataCache.DataRooms.Single(x => x.Id == dataroomid);
                model.DataRoomId = dataroom.Id;
                model.DataRoomName = dataroom.DataRoomName;
                var workflows = DataCache.WorkFlows.Where(x => x.DataRoomId == dataroomid);
                model.WorkFlow = new WorkFlowMaster();
                model.WorkFlowUsers = new List<DataRoomWorkFlowUser>();
                if (workflows != null && workflows.Count() > 0)
                {
                    model.WorkFlows = workflows.ToList();
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_editdataroomworkflow.cshtml", model)
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

        public async Task<JsonResult> SaveDataRoomWorkFlow(DataRoomCustomModel model)
        {
            int activityid=0;
            try
            {                
                if (model.WorkFlow.Id > 0)
                {
                    var original = DataCache.WorkFlows.Where(x => x.Id == model.WorkFlow.Id).First();
                    model.WorkFlow.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    model.WorkFlow.ModifierName = Convert.ToString(Session["UserName"]);
                    model.WorkFlow.ModifiedOn = DateTime.Now;
                    model.WorkFlow.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    model.WorkFlow.CompanyName = Convert.ToString(Session["CompanyName"]);
                    model.WorkFlow.IsActive = true;
                    await _workflowservice.UpdateWorkFlow(model.WorkFlow);
                    activityid = await _logger.LogActivity(model.WorkFlow.CompanyId, "Work Flow Modification", "Work Flow - " + model.WorkFlow.WorkFlowName + " have been updated by " + model.WorkFlow.ModifierName);
                    await _logger.LogDataDifference<WorkFlowMaster>(activityid, original, model.WorkFlow, model.WorkFlow.DataRoomId);
                }
                else
                {
                    model.WorkFlow.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    model.WorkFlow.CreatorName = Convert.ToString(Session["UserName"]);
                    model.WorkFlow.CreatedOn = DateTime.Now;
                    model.WorkFlow.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    model.WorkFlow.CompanyName = Convert.ToString(Session["CompanyName"]);
                    model.WorkFlow.IsActive = true;
                    model.WorkFlow.Id = await _workflowservice.SaveWorkFlow(model.WorkFlow);
                    activityid = await _logger.LogActivity(model.WorkFlow.CompanyId, "Work Flow Creation", "Work Flow - " + model.WorkFlow.WorkFlowName + " have been created by " + model.WorkFlow.ModifierName);
                    await _logger.LogDataDifference<WorkFlowMaster>(activityid, new WorkFlowMaster(), model.WorkFlow, model.WorkFlow.DataRoomId);
                    
                }

                DataCache.RefreshSingleWorkFlow(model.WorkFlow);

                var existedusers = DataCache.DataRoomWorkFlowUsers.Where(x => x.DataRoomId == model.WorkFlow.DataRoomId && x.WorkFlowId == model.WorkFlow.Id);
                if(existedusers!=null && existedusers.Count() > 0)
                {
                    foreach(var user in existedusers.ToList())
                    {
                       await _service.DeleteDataRoomWorkFlowUser(user);
                        DataCache.RemoveDataRoomWorkFlowUserfromCache(user);
                    }
                }

                DataRoomCustomModel dataroommodel = new DataRoomCustomModel();
                dataroommodel.DataRoomId = model.DataRoomId;
                dataroommodel.DataRoomName = model.DataRoomName;
                
                if(model.WorkFlowUsers!=null && model.WorkFlowUsers.Count() > 0)
                {
                    foreach(var user in model.WorkFlowUsers)
                    {
                        if(user.IsActive != false)
                        {
                            user.WorkFlowId = model.WorkFlow.Id;
                            user.WorkFlowName = model.WorkFlow.WorkFlowName;
                            user.IsActive = true;
                            user.CompanyId = model.WorkFlow.CompanyId;
                            user.CompanyName = model.WorkFlow.CompanyName;
                            user.DataRoomId = model.WorkFlow.DataRoomId;
                            user.DataRoomName = model.WorkFlow.DataRoomName;
                            user.CreatedBy = Convert.ToInt32(Session["UserId"]);
                            user.CreatorName = Convert.ToString(Session["UserName"]);
                            user.CreatedOn = DateTime.Now;
                            await _service.SaveDataRoomWorkFlowUser(user);
                            DataCache.RefreshSingleDataRoomWorkFlowUser(user);
                        }
                    }
                }

                var workflows = DataCache.WorkFlows.Where(x => x.DataRoomId == model.WorkFlow.DataRoomId);
                if (workflows != null && workflows.Count() > 0)
                {
                    dataroommodel.WorkFlows = workflows.ToList();
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_dataroomworkflows.cshtml", dataroommodel)
                });
                jr.MaxJsonLength = int.MaxValue;
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("");
        }

        public async Task<JsonResult> DeleteDataRoomWorkFlow(int id)
        {
            int activityid;
            try
            {
                var workflow = DataCache.WorkFlows.Single(x => x.Id == id);
                DataRoomCustomModel dataroommodel = new DataRoomCustomModel();
                await _workflowservice.DeleteWorkFlow(workflow);
                activityid = await _logger.LogActivity(workflow.CompanyId, "WorkFlow Deletion", workflow.WorkFlowName + " have been deleted by " + Convert.ToString(Session["UserName"]));
                DataCache.RemoveWorkFlowfromCache(workflow);
                var workflowusers = DataCache.DataRoomWorkFlowUsers.Where(x => x.WorkFlowId == workflow.Id);
                if(workflowusers!=null && workflowusers.Count() > 0)
                {
                    foreach(var user in workflowusers.ToList())
                    {
                        DataCache.RemoveDataRoomWorkFlowUserfromCache(user);
                    }
                }
                dataroommodel.DataRoomId = workflow.DataRoomId;
                dataroommodel.DataRoomName = workflow.DataRoomName;
                var workflows = DataCache.WorkFlows.Where(x => x.DataRoomId == workflow.DataRoomId);
                if (workflows != null && workflows.Count() > 0)
                {
                    dataroommodel.WorkFlows = workflows.ToList();
                }
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\DataRoom\Views\Shared\_dataroomworkflows.cshtml", dataroommodel)
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