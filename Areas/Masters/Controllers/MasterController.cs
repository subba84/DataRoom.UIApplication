using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Masters.Controllers
{
    [SessionExpire]
    public class MasterController : Controller
    {
        public WorkFlowService _service;
        public LogService _logger;
        public MasterController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new WorkFlowService(token);
            _logger = new LogService(token);
        }
        public ActionResult WorkFlows()
        {
            int companyId = Convert.ToInt32(Session["CompanyId"]);
            var workFlows = DataCache.WorkFlows.Where(x=>x.CompanyId == companyId);
            if (workFlows == null || workFlows.Count() == 0)
                workFlows = new List<WorkFlowMaster>();
            return View(workFlows);
        }

        [HttpGet]
        public ActionResult EditWorkFlow(int id)
        {
            try
            {
                var workflow = id > 0 ? DataCache.WorkFlows.Where(x => x.Id == id).First() : new WorkFlowMaster();
                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Masters\Views\Shared\_editworkflow.cshtml", workflow)
                });
                jr.MaxJsonLength = int.MaxValue;
                jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return jr;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditWorkFlow(WorkFlowMaster model)
        {
            int activityid;
            try
            {
                if(model.Id > 0)
                {
                    var original = DataCache.WorkFlows.Where(x => x.Id == model.Id).First();
                    model.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    model.ModifierName = Convert.ToString(Session["UserName"]);
                    model.ModifiedOn = DateTime.Now;
                    model.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    model.CompanyName = Convert.ToString(Session["CompanyName"]);
                    model.IsActive = true;
                    await _service.UpdateWorkFlow(model);
                    activityid = await _logger.LogActivity(original.CompanyId,"Work Flow Modification", "Work Flow - " + model.WorkFlowName + " have been updated by " + model.ModifierName);
                    await _logger.LogDataDifference(activityid, original, model);                    
                }
                else
                {
                    model.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    model.CreatorName = Convert.ToString(Session["UserName"]);
                    model.CreatedOn = DateTime.Now;
                    model.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    model.CompanyName = Convert.ToString(Session["CompanyName"]);
                    model.IsActive = true;
                    model.Id = await _service.SaveWorkFlow(model);
                    activityid = await _logger.LogActivity(model.CompanyId,"Work Flow Creation", "Work Flow - " + model.WorkFlowName + " have been created by " + model.ModifierName);
                    await _logger.LogDataDifference(activityid, new WorkFlowMaster(), model);
                }
                new Thread(() => DataCache.RefreshSingleWorkFlow(model)).Start();
                TempData["Notification"] = "Work Flow have been saved successfully";
                return RedirectToAction("WorkFlows");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> DeleteWorkFlow(int id)
        {
            try
            {
                var workflow = DataCache.WorkFlows.Single(x => x.Id == id);
                await _service.DeleteWorkFlow(workflow);
                await _logger.LogActivity(workflow.CompanyId,"WorkFlow Deletion", workflow.WorkFlowName + " have been deleted by " + Convert.ToString(Session["UserName"]));
                new Thread(() => DataCache.RemoveWorkFlowfromCache(workflow)).Start();
                TempData["Notification"] = "Work Flow Deleted Successfully";
                return RedirectToAction("WorkFlows", "Master", new { area = "Masters" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}