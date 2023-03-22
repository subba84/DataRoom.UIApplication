using DataRooms.UI.Areas.Users.Models;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Users.Controllers
{
    [SessionExpire]
    public class ManageUserController : Controller
    {
        private UserService _service { get; set; }
        private LogService _logger { get; set; }
        private UserRoleMappingService _mappingService { get; set; }
        public ManageUserController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _service = new UserService(token);
            _mappingService = new UserRoleMappingService(token);
            _logger = new LogService(token);
        }
        
        
        [HttpGet]
        public ActionResult List()
        {
            try
            {
                CustomUserRoleMapping model = new CustomUserRoleMapping();
                GetAllUsersAsync(model);
                
                return View(model);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult List(CustomUserRoleMapping model)
        {
            try
            {
                GetAllUsersAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetAllUsersAsync(CustomUserRoleMapping model)
        {
            try
            {
                var compantId = Convert.ToInt32(Session["CompanyId"]);
                var roleId = Convert.ToInt32(Session["CurrentRoleId"]);
                IEnumerable<DataRooms.UI.Models.User> users = null;
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    if(roleId == AppRole.SuperAdmin)
                    {
                        users = DataCache.Users.Where(x => x.FullName.ToLower().Contains(model.SearchString.ToLower()));
                    }
                    else
                    {
                        users = DataCache.Users.Where(x => x.CompanyId == compantId && x.FullName.ToLower().Contains(model.SearchString.ToLower()));
                    }
                    
                }
                else
                {
                    if (roleId == AppRole.SuperAdmin)
                    {
                        users = DataCache.Users;
                    }
                    else
                    {
                        users = DataCache.Users.Where(x => x.CompanyId == compantId);
                    }
                }
                IPagedList<DataRooms.UI.Models.User> pagedusers = null;
                switch (model.SortColumn)
                {
                    case "CreatedOn":
                        if (model.SortOrder.Equals("desc"))
                            pagedusers = users.OrderByDescending
                                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            pagedusers = users.OrderBy
                                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "UserName":
                        if (model.SortOrder.Equals("desc"))
                            pagedusers = users.OrderByDescending
                                    (m => m.UserName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            pagedusers = users.OrderBy
                                    (m => m.UserName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                }

                model.PagedUsers = pagedusers;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                var user = new User();
                bool isAdmin = false;
                string adAuth = "";
                int companyId = Convert.ToInt32(Session["CompanyId"]);
                if (companyId > 0)
                {
                    var adDetails = DataCache.ADConfiguration.Where(x => x.CompanyId == companyId);
                    if (adDetails != null && adDetails.Count() > 0)
                    {
                        if (adDetails.First().IsADSync == "Y")
                        {
                            adAuth = "Y";
                        }
                    }
                }
                if (id > 0)
                {
                    user = DataCache.Users.Single(x => x.Id == id);
                    var userrolemapping = DataCache.UserRoleMappings.Where(x => x.UserId == user.Id && x.RoleId == AppRole.Admin && x.IsActive == true);
                    if(userrolemapping!=null && userrolemapping.Count() > 0)
                    {
                        isAdmin = true;
                    }
                    if(adAuth == "Y")
                    {
                        user.Password = string.Empty;
                    }
                }
                ViewBag.IsAdmin = isAdmin;
                JsonResult jr = Json(new
                {
                    ADAuth = adAuth,
                    HTML = this.RenderPartialView(@"~\Areas\Users\Views\Shared\_edituser.cshtml", user)
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

        [HttpPost]
        public async Task<ActionResult> Edit(User user,bool IsAdmin = false)
        {
            try
            {
                user.CompanyName = string.Empty;
                if(!string.IsNullOrEmpty(user.EmailId))
                user.EmailId = user.EmailId.ToLower();
                if (user.CompanyId > 0)
                {
                    user.CompanyName = DataCache.Companies.First(x => x.Id == user.CompanyId).CompanyName;
                }
                else
                {
                    user.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                    user.CompanyName = Convert.ToString(Session["CompanyName"]);
                }
                if(user.Id > 0)
                {
                    int activityid = 0;
                    var originaluser = DataCache.Users.Single(x => x.Id == user.Id);
                    user.ModifiedBy = Convert.ToInt32(Session["UserId"]);
                    user.ModifierName = Convert.ToString(Session["UserName"]);
                    user.ModifiedOn = DateTime.Now;
                    await _service.UpdateUser(user);
                    activityid = await _logger.LogActivity(user.CompanyId,"User Modification", user.FullName + " Modified");
                    var d = _logger.LogDataDifference(activityid, originaluser, user);
                }
                else
                {
                    if(user.CompanyId == 0)
                    {
                        user.CompanyId = Convert.ToInt32(Session["CompanyId"]);
                        user.CompanyName = Convert.ToString(Session["CompanyName"]);
                    }
                    user.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    user.CreatorName = Convert.ToString(Session["UserName"]);
                    user.CreatedOn = DateTime.Now;
                    user.Id = await _service.SaveUser(user);
                    await _logger.LogActivity(user.CompanyId,"User Creation", user.FullName + " Created");
                }
                new Thread(() => DataCache.RefreshSingleUser(user)).Start();

                bool isAdmin = false;// Checking wether the combinaiton already exists or not
                var userrolemapping = DataCache.UserRoleMappings.Where(x => x.UserId == user.Id && x.RoleId == AppRole.Admin && x.IsActive == true);
                if (userrolemapping != null && userrolemapping.Count() > 0)
                {
                    isAdmin = true;
                }
                if (IsAdmin == true)
                {
                    if (!isAdmin) 
                    {
                        UserRoleMapping mapping = new UserRoleMapping();
                        mapping.UserId = user.Id;
                        mapping.UserName = user.UserName;
                        mapping.RoleId = AppRole.Admin;
                        mapping.RoleName = "Admin";
                        mapping.IsActive = true;
                        mapping.CreatedBy = Convert.ToInt32(Session["Id"]);
                        mapping.CreatorName = Convert.ToString(Session["Name"]);
                        mapping.CreatedOn = DateTime.Now;
                        mapping.CompanyId = user.CompanyId;
                        mapping.CompanyName = user.CompanyName;
                        await _mappingService.AddUserRoleMappingAsync(new List<UserRoleMapping> { mapping });
                        new Thread(() => DataCache.RefreshUserRoleMappings()).Start();
                    }
                }
                else
                {
                    await DeleteAdminRole(user);
                }

                TempData["Notification"] = "User Saved Successfully";
                return RedirectToAction("List","ManageUser",new { area = "Users"});
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteAdminRole(User user)
        {
            try
            {
                var adminroleofuser = DataCache.UserRoleMappings.Where(x => x.UserId == user.Id && x.RoleId == AppRole.Admin);
                if(adminroleofuser!=null && adminroleofuser.Count() > 0)
                {
                    await _mappingService.DeleteUserRoleMappingAsync(adminroleofuser.First());
                    new Thread(() => DataCache.RemoveUserRoleMappingfromCache(adminroleofuser.First())).Start();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var user = DataCache.Users.Single(x => x.Id == id);
                await _service.DeleteUser(user);
                new Thread(() => DataCache.RemoveUserfromCache(user)).Start();
                await DeleteAdminRole(user);
                await _logger.LogActivity(user.CompanyId,"User Deletion", user.FullName + " Deleted");
                TempData["Notification"] = "User Deleted Successfully";
                return RedirectToAction("List", "ManageUser", new { area = "Users" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult CheckCompanyADAuthorNot(int companyid)
        {
            var adDetails = DataCache.ADConfiguration.Where(x => x.CompanyId == companyid);
            if (adDetails != null && adDetails.Count() > 0)
            {
                if (adDetails.First().IsADSync == "Y")
                {
                    return Json("Y",JsonRequestBehavior.AllowGet);
                }
            }
            return Json("N", JsonRequestBehavior.AllowGet);
        }
    }
}