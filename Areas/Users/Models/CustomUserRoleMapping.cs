using DataRooms.UI.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Areas.Users.Models
{
    public class CustomUserRoleMapping : PagingModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public List<UserRoleMapping> ExistedMappings { get; set; }
        public IEnumerable<RoleMaster> Roles { get; set; }
        public IPagedList<User> PagedUsers { get; set; }
        public IPagedList<UserRoleMapping> PagedUserRoleMappings { get; set; }
    }
}