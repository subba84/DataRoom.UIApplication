using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class WorkFlowMaster
    {
        [Key]
        public int Id { get; set; }
        public string WorkFlowName { get; set; }
        public bool IsInitiationRequired { get; set; }
        public bool IsSingleInitiator { get; set; }
        public bool IsMultipleInitiator { get; set; }
        public bool IsReviewRequired { get; set; }
        public bool IsSingleReviewerRequired { get; set; }
        public bool IsMultipleReviewersRequired { get; set; }
        public bool IsApprovalRequired { get; set; }
        public bool IsSingleApproverRequired { get; set; }
        public bool IsMultipleApproversRequired { get; set; }
        public bool IsSingleReviewSufficient { get; set; }
        public bool IsSignleApprovalSufficient { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifierName { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? DeletedBy { get; set; }
        public string DeletorName { get; set; }
        public DateTime? DeletedOn { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int DataRoomId { get; set; }
        public string DataRoomName { get; set; }
    }
}