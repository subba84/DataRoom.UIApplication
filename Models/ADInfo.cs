using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class ADInfo
    {
        [Key]
        public int Id { get; set; }
        public string IsADSync { get; set; }
        public string IPAddress { get; set; }
        public string DomainName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}