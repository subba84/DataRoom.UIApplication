using DataRooms.UI.Code;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Login.Controllers
{
    public class AuthenticateController : Controller
    {
        private AuthService _service = new AuthService();
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");

        [HttpGet]
        public ActionResult Login(string message=null)
        {
            //if(message==null)
            TempData["Notification"] = message;
            int isActivated = Convert.ToInt32(ConfigurationManager.AppSettings["ActivateKey"]);
            if (isActivated == 0)
            {
                return RedirectToAction("ConfigureDatabase", "DatabaseSetup",new { area = "Activation"});
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(AuthenticateRequest model)
        {
            var usernotification = "";AuthenticateResponse response = null;
            if (ModuleAvailable())
            {
                // Check for User Existance
                var userDetails = DataCache.Users.Where(x => x.UserName == model.Username);
                if(userDetails == null || userDetails.Count() == 0)
                {
                    usernotification = "User not found";
                    return RedirectToAction("Login", "Authenticate", new { area = "Login", message = usernotification });
                }
                else
                {
                    User user = userDetails.First();
                    if(user.UserName == "admin")
                    {
                        response = await _service.CheckAuth(model);
                    }
                    else
                    {
                        var companyADDetails = DataCache.ADConfiguration.Where(x => x.Id == user.CompanyId);
                        if (companyADDetails != null && companyADDetails.Count() > 0)
                        {
                            var ad = companyADDetails.First();
                            if (ad.IsADSync == "Y")
                            {
                                bool isAuth = ADHelper.ADAuth(model.Username, model.Password, ad);
                                if (isAuth)
                                {
                                    model.IsADAuth = "Y";
                                    response = await _service.CheckAuth(model);
                                }
                                else
                                {
                                    usernotification = "Invalid Credentials";
                                    return RedirectToAction("Login", "Authenticate", new { area = "Login", message = usernotification });
                                }
                            }
                            else
                            {
                                response = await _service.CheckAuth(model);
                            }
                        }
                        else
                        {
                            response = await _service.CheckAuth(model);
                        }
                    }
                }
                if (response != null)
                {
                    FillSessions(response);
                    LogService _logger = new LogService(response.Token);
                    await _logger.LogActivity(response.CompanyId, ActivityCategory.Login, response.FirstName + " logged into the system");
                    var workflowuser = DataCache.DataRoomWorkFlowUsers.Where(x => x.UserId == response.Id && (x.RoleId == AppRole.Reviewer || x.RoleId == AppRole.Approver || x.RoleId == AppRole.Initiator) && x.IsActive == true);
                    int currentroleid = Convert.ToInt32(Session["CurrentRoleId"]);
                    if (workflowuser != null && workflowuser.Count() > 0)
                    {
                        return RedirectToAction("Index", "TaskDashboard", new { area = "Home" });
                    }
                    if (currentroleid == AppRole.Admin)
                    {
                        return RedirectToAction("AdminDashboard", "Admin", new { area = "Home" });
                    }
                    else if (currentroleid == AppRole.SuperAdmin)
                    {
                        return RedirectToAction("SuperAdminDashboard", "Dashboard", new { area = "Home" });
                    }
                    return RedirectToAction("Index", "TaskDashboard", new { area = "Home" });
                }
                var notification = "Invalid Credentials";
                return RedirectToAction("Login", "Authenticate", new { area = "Login", message = notification });
            }
            else
            {
                var notification = "No Active Modules Available";
                return RedirectToAction("Login", "Authenticate", new { area = "Login", message = notification });
            }
            
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        public JsonResult GetUserbyEmailId(string emailid)
        {
            try
            {
                var userDetails = DataCache.Users.Where(x => x.EmailId == emailid);
                if(userDetails!=null && userDetails.Count() > 0)
                {
                    return Json(userDetails.First(), JsonRequestBehavior.AllowGet);
                }
                return Json(new User(), JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> UpdatePassword(int id,string password)
        {
            try
            {
                string token = Convert.ToString(Session["AuthToken"]);
                string username = Convert.ToString(Session["UserName"]);
                LogService _logger = new LogService(token);
                User user = DataCache.Users.Single(x => x.Id == id);
                await _logger.LogActivity(user.CompanyId, ActivityCategory.Logout, username + " updated password");
                await _service.UpdatePassword(id, password);
                TempData["Notification"] = "Password updated successfully. Please login with new password";
                return RedirectToAction("Login");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
             

        public async Task<ActionResult> Logout()
        {
            if (Session["UserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Authenticate", new { area = "Login" });
            }
            else
            {
                string token = Convert.ToString(Session["AuthToken"]);
                string username = Convert.ToString(Session["UserName"]);
                int id = Convert.ToInt32(Session["UserId"]);
                User user = DataCache.Users.Single(x => x.Id == id);
                LogService _logger = new LogService(token);
                await _logger.LogActivity(user.CompanyId, ActivityCategory.Logout, (username == "" ? "user" : username) + " logged out of the system");
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Authenticate", new { area = "Login" });
            }
        }

        public void FillSessions(AuthenticateResponse response)
        {
            try
            {
                Session["UserName"] = response.FirstName;
                Session["UserId"] = response.Id;
                Session["AuthToken"] = response.Token;
                Session["CompanyId"] = response.CompanyId;
                Session["CompanyName"] = response.CompanyName;
                if (response.AssignedRoles!=null && response.AssignedRoles.Count() > 0)
                {
                    Session["Roles"] = response.AssignedRoles;
                    Session["CurrentRoleId"] = response.AssignedRoles.FirstOrDefault().RoleId;
                    Session["CurrentRole"] = response.AssignedRoles.FirstOrDefault().RoleName;
                }
                else
                {
                    Session["Roles"] = new List<UserRoleMapping>();
                    Session["CurrentRoleId"] = AppRole.User;
                    Session["CurrentRole"] = "User";
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ChangeRole(int roleid)
        {
            try
            {
                IEnumerable<UserRoleMapping> assignedRoles = Session["Roles"] as IEnumerable<UserRoleMapping>;
                if(assignedRoles!=null && assignedRoles.Count() > 0)
                {
                    var selectedRole = assignedRoles.Where(x => x.RoleId == roleid).FirstOrDefault();
                    if(selectedRole!=null && selectedRole.Id > 0)
                    {
                        Session["CurrentRoleId"] = selectedRole.RoleId;
                        Session["CurrentRole"] = selectedRole.RoleName;
                    }
                }
                return RedirectToAction("Home", "Dashboard", new { area = "Home" });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public bool ModuleAvailable()
        {
            try
            {
                LicenseInfo license = DataCache.Module;
                DateTime fromDate = DateTime.MinValue;
                DateTime toDate = DateTime.MinValue;
                try
                {
                    var format = new CultureInfo("en-GB");
                    fromDate = DateTime.Parse(license.Column4, new CultureInfo("en-US", false));
                    toDate = DateTime.Parse(license.Column5, new CultureInfo("en-US", false));
                    logger.Debug(fromDate);
                    logger.Debug(toDate);
                }
                catch (Exception ex)
                {
                    var format = new CultureInfo("en-US");
                    fromDate = Convert.ToDateTime(license.Column4, format);
                    toDate = Convert.ToDateTime(license.Column5, format);
                    logger.Debug("ex - " +fromDate);
                    logger.Debug("ex - " + toDate);
                }

                if (Convert.ToDateTime(fromDate).Date <= DateTime.Now.Date && DateTime.Now.Date <= Convert.ToDateTime(toDate).Date)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return false;
        }
    }
}