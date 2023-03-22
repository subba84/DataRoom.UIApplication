using DataRooms.UI.Areas.Users.Models;
using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Users.Controllers
{
    [SessionExpire]
    public class ManageUserRoleController : Controller
    {
        private LogService _logger { get; set; }
        private UserRoleMappingService _userRoleMappingService { get; set; }
        private UserService _userService { get; set; }
        public int companyId;
        public ManageUserRoleController()
        {
            var token = Convert.ToString(System.Web.HttpContext.Current.Session["AuthToken"]);
            _logger = new LogService(token);
            _userRoleMappingService = new UserRoleMappingService(token);
            _userService = new UserService(token);
            companyId = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyId"]);
        }

       
        public JsonResult GetAllRoles()
        {
            try
            {
                IEnumerable<RoleMaster> roles = DataCache.Roles;
                return Json(roles,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetAllUsers(string searchString,int compid=0)
        {
            try
            {               
                if(companyId == 0)
                {
                    companyId = compid;
                }
                IEnumerable<User> users = DataCache.Users.Where(x=> x.CompanyId == companyId && x.UserName.ToLower().Contains(searchString.ToLower()));
                users = users.Where(x=>x.FullName.ToLower().Contains(searchString.ToLower()));
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult List()
        {
            try
            {
                CustomUserRoleMapping model = new CustomUserRoleMapping();
                GetAllUserRoleMappingsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult List(CustomUserRoleMapping model)
        {
            try
            {
                GetAllUserRoleMappingsAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetAllUserRoleMappingsAsync(CustomUserRoleMapping model)
        {
            try
            {
                IEnumerable<DataRooms.UI.Models.UserRoleMapping> userrolemappings = null;
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    userrolemappings = DataCache.UserRoleMappings.Where(x => x.UserName.ToLower().Contains(model.SearchString.ToLower())
                                                                          || x.RoleName.ToLower().Contains(model.SearchString.ToLower()));
                }
                else
                {
                    userrolemappings = DataCache.UserRoleMappings;
                }
                IPagedList<DataRooms.UI.Models.UserRoleMapping> pageduserrolemappings = null;
                switch (model.SortColumn)
                {
                    case "CreatedOn":
                        if (model.SortOrder.Equals("desc"))
                            pageduserrolemappings = userrolemappings.OrderByDescending
                                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            pageduserrolemappings = userrolemappings.OrderBy
                                    (m => m.CreatedOn).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "UserName":
                        if (model.SortOrder.Equals("desc"))
                            pageduserrolemappings = userrolemappings.OrderByDescending
                                    (m => m.UserName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            pageduserrolemappings = userrolemappings.OrderBy
                                    (m => m.UserName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                    case "RoleName":
                        if (model.SortOrder.Equals("desc"))
                            pageduserrolemappings = userrolemappings.OrderByDescending
                                    (m => m.RoleName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        else
                            pageduserrolemappings = userrolemappings.OrderBy
                                    (m => m.RoleName).ToPagedList(model.CurrentPage, model.RecordsPerPage);
                        break;
                }

                model.PagedUserRoleMappings = pageduserrolemappings;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int userid)
        {
            try
            {
                var user = new CustomUserRoleMapping();
                List<UserRoleMapping> existedMappings = new List<UserRoleMapping>();
                if (userid > 0)
                {
                    var userMappings = DataCache.UserRoleMappings.Where(x=>x.UserId == userid);
                    existedMappings = userMappings.ToList();
                    user.UserId = userid;
                    user.UserName = existedMappings[0].UserName;
                    user.IsActive = true;
                }
                user.Roles = DataCache.Roles;
                user.ExistedMappings = new List<UserRoleMapping>();
                foreach (var role in user.Roles)
                {
                    UserRoleMapping mapping = new UserRoleMapping();
                    mapping.RoleId = role.Id;
                    mapping.RoleName = role.RoleName;
                    if (existedMappings != null && existedMappings.Select(x => x.RoleId).Contains(mapping.RoleId))
                    {
                        mapping.UserId = existedMappings[0].UserId;
                        mapping.UserName = existedMappings[0].UserName;
                    }
                    user.ExistedMappings.Add(mapping);
                }

                JsonResult jr = Json(new
                {
                    HTML = this.RenderPartialView(@"~\Areas\Users\Views\Shared\_edituserrolemapping.cshtml", user)
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
        public async Task<ActionResult> Edit(CustomUserRoleMapping userRoleMappingList)
        {
            try
            {
                // Deleting existing roles
                var rolemappings = DataCache.UserRoleMappings.Where(x => x.Id == userRoleMappingList.UserId);
                if(rolemappings!=null && rolemappings.Count() > 0)
                {
                    foreach(var item in rolemappings)
                    {
                        await _userRoleMappingService.DeleteUserRoleMappingAsync(item);
                    }
                }
                
                // Insert new ones.
                List<UserRoleMapping> mappings = new List<UserRoleMapping>();
                if (userRoleMappingList != null)
                {
                    foreach(var role in userRoleMappingList.Roles)
                    {
                        if(role.Id > 0)
                        {
                            UserRoleMapping mapping = new UserRoleMapping();
                            mapping.UserId = userRoleMappingList.UserId;
                            mapping.UserName = userRoleMappingList.UserName;
                            mapping.RoleId = role.Id;
                            mapping.RoleName = role.RoleName;
                            mapping.IsActive = true;
                            mapping.CreatedBy = Convert.ToInt32(Session["UserId"]);
                            mapping.CreatorName = Convert.ToString(Session["UserName"]);
                            mapping.CreatedOn = DateTime.Now;
                            mappings.Add(mapping);
                        }
                    }
                }
                await _userRoleMappingService.AddUserRoleMappingAsync(mappings);
                new System.Threading.Thread(() => DataCache.RefreshUserRoleMappings()).Start();
                await _logger.LogActivity(mappings[0].CompanyId,"User Role Map Creation", mappings[0].UserName + " assigned with " + string.Join(",", mappings.Select(x => x.RoleName)) + " Roles");
                TempData["Notification"] = "User-Role Mapping Created Successfully";
                return RedirectToAction("List", "ManageUserRole", new { area = "Users" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<ActionResult> Delete(int id)
        {
            try
            {                
                var rolemapping = DataCache.UserRoleMappings.Single(x => x.Id == id);
                await _userRoleMappingService.DeleteUserRoleMappingAsync(rolemapping);
                await _logger.LogActivity(rolemapping.CompanyId, "User Role Map Deletion", rolemapping.UserName + " un-assigned with " + rolemapping.RoleName + " Role");
                TempData["Notification"] = "User-Role Mapping Deleted Successfully";
                return RedirectToAction("List", "ManageUserRole", new { area = "Users" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}