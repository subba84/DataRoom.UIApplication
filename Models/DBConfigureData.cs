using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class DBConfigureData
    {
        public string DBHostName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string DomainName { get; set; }
        public string PublicIp { get; set; }
        public string EmailId { get; set; }
        public string EncryptedDomainName { get; set; }
        public string EncryptedPublicIp { get; set; }
        public string EncryptedEmailId { get; set; }
    }
}