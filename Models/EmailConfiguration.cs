using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class EmailConfiguration
    {
        [Key]
        public int Id { get; set; }
        public string MailType { get; set; }
        public string Protocal { get; set; }
        public string SenderAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ServerAddress { get; set; }
        public string PortNumber { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifierName { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}