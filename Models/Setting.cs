using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DataRooms.UI.Models
{
    public class Setting
    {
        [Key]
        public int Id { get; set; }
        public bool IsCloudEnabled { get; set; }
        public string StoragePath { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string AWSBucketName { get; set; }
        public string AWSBucketRegion { get; set; }
        public string LogsPath { get; set; }
    }
}