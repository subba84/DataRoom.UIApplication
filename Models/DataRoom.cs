using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class DataRoom
    {
        [Key]
        public int Id { get; set; }
        public string DataRoomName { get; set; }
        public string Description { get; set; }
        public string DataRoomSize { get; set; }
        public string RelativePath { get; set; }
        public string Guid { get; set; }
        public bool? IsArchivalRequired { get; set; }
        public bool? IsDeletionRequired { get; set; }
        public bool? IsLogsRequired { get; set; }
        public string RetentionPeriod { get; set; }
        public string ArchivalPeriod { get; set; }
        public string DeletionPeriod { get; set; }
        public bool IsWorkFlowRequired { get; set; }
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
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int WorkFlowId { get; set; }
        public string WorkFlowName { get; set; }
        public string ArchivalBasedOn { get; set; }
        public string RetentionBasedOn { get; set; }
        public string DeletionBasedOn { get; set; }
        public string ArchivalType { get; set; }
        public string ArchivalPath { get; set; }
        public string ArchivalAWSAccessKey { get; set; }
        public string ArchivalAWSSecurityKey { get; set; }
        public string ArchivalAWSRegion { get; set; }
        public bool IsLogsRequiredforCompany { get; set; }
    }
}