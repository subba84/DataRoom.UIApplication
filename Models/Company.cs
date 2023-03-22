using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string RelativePath { get; set; }
        public string Guid { get; set; }
        public string StorageCategory { get; set; }
        public string CloudAccessKey { get; set; }
        public string CloudSecurityKey { get; set; }
        public string StoragePath { get; set; }
        public string AWSRegion { get; set; }
        public bool IsLogsRequired { get; set; }
        public string LogsStoragePath { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifierName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? DeletedBy { get; set; }
        public string DeletorName { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string Comments { get; set; }
        public bool IsExternalSharingEnabled { get; set; }
        public string SharingUrl { get; set; }
    }
}