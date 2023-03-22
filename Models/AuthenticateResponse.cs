using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Token { get; set; }
        public DateTime? Expiration { get; set; }
        public IEnumerable<UserRoleMapping> AssignedRoles { get; set; }
    }
}