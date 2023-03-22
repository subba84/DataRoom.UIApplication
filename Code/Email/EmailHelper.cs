using Castle.Components.DictionaryAdapter.Xml;
using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace DataRooms.UI.Code.Email
{
    public class EmailHelper
    {
        private readonly string appUrl;
        public EmailHelper()
        {
            appUrl = ConfigurationManager.AppSettings["AppUrl"];
        }
        public EmailObject GetMailSubjectandBodyforFileOperations(int fileid = 0, string statusflag = "", List<UI.Models.File> files=null)
        {
            EmailObject emailObject = new EmailObject();
            try
            {
                var file = DataCache.Files.First(x => x.Id == fileid);
                List<int> toUsers = new List<int>();
                // Get SharBox Admins
                var dataroom = DataCache.DataRooms.First(x => x.Id == file.DataRoomId);
                var dataroomadmins = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == dataroom.Id && x.HasFullControl == true && x.IsActive == true);
                if (dataroomadmins != null && dataroomadmins.Count() > 0)
                {
                    toUsers.AddRange(dataroomadmins.Select(x => x.UserId).Distinct().ToList());
                }
                // Get Folder Admins
                if (file.FolderId > 0)
                {
                    var folderusers = DataCache.FolderPermissions.Where(x => x.Id == file.FolderId && x.IsActive == true);
                    if (folderusers != null && folderusers.Count() > 0)
                    {
                        toUsers.AddRange(folderusers.Select(x => x.UserId).Distinct().ToList());
                    }
                }

                string subject = string.Empty;
                StringBuilder mailBody = new StringBuilder();
                switch (statusflag)
                {
                    case "BulkDocumentsAdded":
                        subject = "Multiple Files have been created under SharBox - " + file.DataRoomName + (file.FolderId > 0 ? ", Folder - " + file.FolderName : "");
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> Following Files have been created. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        //mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        //mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        if(files!=null && files.Count() > 0)
                        {
                            foreach(var fileItem in files)
                            {
                                mailBody.Append("<tr><td>File Name</td><td><b>" + fileItem.FileName + "</b></td></tr>");
                                //mailBody.Append("<tr><td><b>Created By</b></td><td>" + file.CreatorName + "</td></tr>");
                                //mailBody.Append("<tr><td><b>Created On</b></td><td>" + file.CreatedOn.ToApplicationFormat() + "</td></tr>");
                            }
                        }                        
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "DocumentAdded":
                        subject = "File - " + file.FileName + " have been created";
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> New File have been created. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Created By</b></td><td>" + file.CreatorName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Created On</b></td><td>" + file.CreatedOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "DocumentCheckOut":
                        subject = "File - " + file.FileName + " have been check out by " + file.CheckOutByName;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> File have been checked-out. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Check-Out By</b></td><td>" + file.CheckOutByName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Check-Out On</b></td><td>" + file.CheckOutOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "DocumentCheckIn":
                        subject = "File - " + file.FileName + " have been check in by " + file.CheckInByName;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> File have been checked-in. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Check-In By</b></td><td>" + file.CheckInByName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Check-In On</b></td><td>" + file.CheckInOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "DocumentDelete":
                        subject = "File - " + file.FileName + " have been deleted by " + file.DeletorName;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> File have been deleted. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Deleted By</b></td><td>" + file.DeletorName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Deleted On</b></td><td>" + file.DeletedOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                }

                emailObject.ToUserIds = toUsers;
                emailObject.MailSubject = subject;
                emailObject.MailBody = mailBody.ToString();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return emailObject;
        }

        public EmailObject GetMailSubjectandBodyforUserPermissions(int userid = 0, string objectType = "", int objectId = 0, string loggedInUser = "")
        {
            EmailObject emailObject = new EmailObject();
            try
            {
                var user = DataCache.Users.First(x => x.Id == userid);
                emailObject.ToUserIds = new List<int>();
                emailObject.ToUserIds.Add(user.Id);
                List<string> accesses = new List<string>();
                string subject = string.Empty;
                StringBuilder mailBody = new StringBuilder();
                switch (objectType)
                {
                    case "DataRoom":
                        var dataroom = DataCache.DataRooms.First(x => x.Id == objectId);
                        var permissions = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == dataroom.Id && x.IsActive == true);
                        if (permissions != null && permissions.Count() > 0)
                        {
                            foreach (var permission in permissions.ToList())
                            {
                                if (permission.HasFullControl == true)
                                {
                                    accesses.Add("Full Control");
                                    break;
                                }
                                else
                                {
                                    if (permission.HasRead == true)
                                    {
                                        accesses.Add("Read");
                                    }
                                    if (permission.HasWrite == true)
                                    {
                                        accesses.Add("Write");
                                    }
                                    if (permission.HasDelete == true)
                                    {
                                        accesses.Add("Delete");
                                    }
                                }
                            }
                        }
                        subject = "You have " + string.Join(",", accesses) + " to SharBox - " + dataroom.DataRoomName;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> " + subject + "</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + dataroom.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Access added/modified by </b></td><td>" + loggedInUser + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "Folder":
                        var folder = DataCache.Folders.First(x => x.Id == objectId);
                        var folderpermissions = DataCache.FolderPermissions.Where(x => x.FolderId == folder.Id && x.IsActive == true);
                        if (folderpermissions != null && folderpermissions.Count() > 0)
                        {
                            foreach (var permission in folderpermissions.ToList())
                            {
                                if (permission.HasFullControl == true)
                                {
                                    accesses.Add("Full Control");
                                    break;
                                }
                                else
                                {
                                    if (permission.HasRead == true)
                                    {
                                        accesses.Add("Read");
                                    }
                                    if (permission.HasWrite == true)
                                    {
                                        accesses.Add("Write");
                                    }
                                    if (permission.HasDelete == true)
                                    {
                                        accesses.Add("Delete");
                                    }
                                }
                            }
                        }
                        subject = "You have " + string.Join(",", accesses) + " to Folder - " + folder.FolderName + " in SharBox - " + folder.DataRoomName;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> " + subject + "</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + folder.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + folder.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Access added/modified by </b></td><td>" + loggedInUser + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "File":
                        var file = DataCache.Files.First(x => x.Id == objectId);
                        var filepermissions = DataCache.FilePermissions.Where(x => x.FileId == file.Id && x.IsActive == true);
                        if (filepermissions != null && filepermissions.Count() > 0)
                        {
                            foreach (var permission in filepermissions.ToList())
                            {
                                if (permission.HasFullControl == true)
                                {
                                    accesses.Add("Full Control");
                                    break;
                                }
                                else
                                {
                                    if (permission.HasRead == true)
                                    {
                                        accesses.Add("Read");
                                    }
                                    if (permission.HasWrite == true)
                                    {
                                        accesses.Add("Write");
                                    }
                                    if (permission.HasDelete == true)
                                    {
                                        accesses.Add("Delete");
                                    }
                                }
                            }
                        }
                        subject = "You have " + string.Join(",", accesses) + " to File - " + file.FileName + (file.FolderId > 0 ? (" in Folder - " + file.FolderName) : "") + " in SharBox - " + file.DataRoomName;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> " + subject + "</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        if (file.FolderId > 0)
                        {
                            mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        }
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Access added/modified by </b></td><td>" + loggedInUser + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "ItemTracker":
                        var itemtracker = DataCache.ItemTrackerMetaData.First(x => x.Id == objectId);
                        var itemtrackerpermissions = DataCache.ItemTrackerPermissions.Where(x => x.ItemTrackerId == itemtracker.Id && x.IsActive == true);
                        if (itemtrackerpermissions != null && itemtrackerpermissions.Count() > 0)
                        {
                            foreach (var permission in itemtrackerpermissions.ToList())
                            {
                                if (permission.HasFullControl == true)
                                {
                                    accesses.Add("Full Control");
                                    break;
                                }
                                else
                                {
                                    if (permission.HasRead == true)
                                    {
                                        accesses.Add("Read");
                                    }
                                    if (permission.HasWrite == true)
                                    {
                                        accesses.Add("Write");
                                    }
                                    if (permission.HasDelete == true)
                                    {
                                        accesses.Add("Delete");
                                    }
                                }
                            }
                        }
                        subject = "You have " + string.Join(",", accesses) + " to Item Tracker - " + itemtracker.ItemTrackerName + (itemtracker.FolderId > 0 ? (" in Folder - " + itemtracker.FolderName) : "") + " in SharBox - " + itemtracker.DataRoomName;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> " + subject + "</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>Item Tracker</b></td><td>" + itemtracker.ItemTrackerName + "</td></tr>");
                        if (itemtracker.FolderId > 0)
                        {
                            mailBody.Append("<tr><td><b>Folder</b></td><td>" + itemtracker.FolderName + "</td></tr>");
                        }
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + itemtracker.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Access added/modified by </b></td><td>" + loggedInUser + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                }

                emailObject.MailSubject = subject;
                emailObject.MailBody = mailBody.ToString();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return emailObject;
        }

        public EmailObject GetMailSubjectandBodyforWorkFlow(int toUserId = 0, string statusFlag = "", int fileId = 0,List<UI.Models.File> files=null)
        {
            EmailObject emailObject = new EmailObject();
            try
            {
                var user = DataCache.Users.First(x => x.Id == toUserId);
                emailObject.ToUserIds = new List<int>();
                emailObject.ToUserIds.Add(user.Id);
                var file = DataCache.Files.First(x => x.Id == fileId);
                var folder = DataCache.Folders.First(x => x.Id == file.FolderId);
                var workflow = DataCache.WorkFlows.First(x => x.Id == folder.WorkFlowId);
                string subject = string.Empty;
                StringBuilder mailBody = new StringBuilder();
                switch (statusFlag)
                {
                    case "BulkDocumentsSubmitted":
                        subject = "Multiple Files " + (file.FolderId > 0 ? " in Folder - " + file.FolderName : "") + " in SharBox - " + file.DataRoomName + " have been submitted and came for  your " + (workflow.IsApprovalRequired == true ? "review" : "approval");
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> Following Files have been submitted. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        //mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        //mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        if (files != null && files.Count() > 0)
                        {
                            foreach (var fileItem in files)
                            {
                                mailBody.Append("<tr><td>File Name</td><td><b>" + fileItem.FileName + "</b></td></tr>");
                                //mailBody.Append("<tr><td><b>Created By</b></td><td>" + file.CreatorName + "</td></tr>");
                                //mailBody.Append("<tr><td><b>Created On</b></td><td>" + file.CreatedOn.ToApplicationFormat() + "</td></tr>");
                            }
                        }
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "Submitted":
                        subject = "File - " + file.FileName + (file.FolderId > 0 ? " in Folder - " + file.FolderName : "") + " in SharBox - " + file.DataRoomName + " have been submitted and came for  your " + (workflow.IsApprovalRequired == true ? "review" : "approval");
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> " + subject + "</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Submitted by </b></td><td>" + file.CreatorName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Submitted On </b></td><td>" + file.CreatedOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "'>here</a> for more details</p>");
                        break;
                    case "Reviewed":
                        subject = "File - " + file.FileName + (file.FolderId > 0 ? " in Folder - " + file.FolderName : "") + " in SharBox - " + file.DataRoomName + " have been submitted and came for  your approval";
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> " + subject + "</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Submitted by </b></td><td>" + file.CreatorName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Submitted On </b></td><td>" + file.CreatedOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "'>here</a> for more details</p>");
                        break;
                    case "Approved":
                        subject = "File - " + file.FileName + (file.FolderId > 0 ? " in Folder - " + file.FolderName : "") + " in SharBox - " + file.DataRoomName + " have been approved";
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> " + subject + "</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Approved by </b></td><td>" + file.ApproverName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Approved On </b></td><td>" + file.ApprovedOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "'>here</a> for more details</p>");
                        break;
                    case "Rejected":
                        subject = "File - " + file.FileName + (file.FolderId > 0 ? " in Folder - " + file.FolderName : "") + " in SharBox - " + file.DataRoomName + " have been rejected";
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> " + subject + "</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + file.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + file.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>File</b></td><td>" + file.FileName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Rejected by </b></td><td>" + file.ModifierName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Rejected On </b></td><td>" + file.ModifiedOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "'>here</a> for more details</p>");
                        break;                        
                }
                emailObject.MailSubject = subject;
                emailObject.MailBody = mailBody.ToString();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return emailObject;
        }

        public EmailObject GetMailSubjectandBodyforFileShare(int fileId, string loggedInUser)
        {
            EmailObject emailObject = new EmailObject();
            try
            {
                var file = DataCache.Files.First(x => x.Id == fileId);
                DataRooms.UI.Models.Company company = new DataRooms.UI.Models.Company();
                var companyDetails = DataCache.Companies.Where(x => x.Id == file.CompanyId);
                if (companyDetails != null && companyDetails.Count() > 0)
                {
                    company = companyDetails.First();
                }
                string url = company.SharingUrl + "/externalfiledownload?fileid=" + file.Id;
                string subject = "File - " + file.FileName + " have been shared with you by " + loggedInUser;
                StringBuilder mailBody = new StringBuilder();
                mailBody.Append(subject);
                mailBody.Append("<br/><br/><br/>");
                mailBody.Append("Please click " + "<a href='" + url + "'> here</a>" + " to access the file");
                emailObject.MailSubject = subject;
                emailObject.MailBody = mailBody.ToString();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return emailObject;
        }

        public EmailObject GetMailSubjectandBodyforItemTrackerOperations(int itemtracker = 0, string statusflag = "",string loggedInUser="")
        {
            EmailObject emailObject = new EmailObject();
            try
            {
                var itemTracker = DataCache.ItemTrackerMetaData.First(x => x.Id == itemtracker);
                List<int> toUsers = new List<int>();
                // Get SharBox Admins
                var dataroom = DataCache.DataRooms.First(x => x.Id == itemTracker.DataRoomId);
                var dataroomadmins = DataCache.DataRoomPermissions.Where(x => x.DataRoomId == dataroom.Id && x.HasFullControl == true && x.IsActive == true);
                if (dataroomadmins != null && dataroomadmins.Count() > 0)
                {
                    toUsers.AddRange(dataroomadmins.Select(x => x.UserId).Distinct().ToList());
                }
                // Get Folder Admins
                if (itemTracker.FolderId > 0)
                {
                    var folderusers = DataCache.FolderPermissions.Where(x => x.Id == itemTracker.FolderId && x.IsActive == true);
                    if (folderusers != null && folderusers.Count() > 0)
                    {
                        toUsers.AddRange(folderusers.Select(x => x.UserId).Distinct().ToList());
                    }
                }

                string subject = string.Empty;
                StringBuilder mailBody = new StringBuilder();
                switch (statusflag)
                {
                    case "ItemTrackerAdded":
                        subject = "Item Tracker - " + itemTracker.ItemTrackerName + " have been created";
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> New Item Tracker have been created. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + itemTracker.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + itemTracker.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Item Tracker</b></td><td>" + itemTracker.ItemTrackerName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Created By</b></td><td>" + itemTracker.CreatorName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Created On</b></td><td>" + itemTracker.CreatedOn.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;                    
                    case "ItemTrackerDelete":
                        subject = "Item Tracker - " + itemTracker.ItemTrackerName + " have been deleted by " + loggedInUser;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> File have been deleted. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + itemTracker.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + itemTracker.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>ItemTracker</b></td><td>" + itemTracker.ItemTrackerName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Deleted By</b></td><td>" + loggedInUser + "</td></tr>");
                        mailBody.Append("<tr><td><b>Deleted On</b></td><td>" + DateTime.Now.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "ItemTrackerDataAdd":
                        subject = "Data have been added/modified to Item Tracker - " + itemTracker.ItemTrackerName + " by " + loggedInUser;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> Item Tracker Data have been added/modified. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + itemTracker.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + itemTracker.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>ItemTracker</b></td><td>" + itemTracker.ItemTrackerName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Added/Modified By</b></td><td>" + loggedInUser + "</td></tr>");
                        mailBody.Append("<tr><td><b>Added/Modified On</b></td><td>" + DateTime.Now.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                    case "ItemTrackerDataDelete":
                        subject = "Data have been deleted from Item Tracker - " + itemTracker.ItemTrackerName + " by " + loggedInUser;
                        mailBody.Append("<p> Hello,</p>");
                        mailBody.Append("<p style='padding:15px;'> Item Tracker Data have been deleted. Please find the details below.</p>");
                        mailBody.Append("<table style='border:1px solid black;border-collapse:collapse' border='1'>");
                        mailBody.Append("<tr><td><b>SharBox</b></td><td>" + itemTracker.DataRoomName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Folder</b></td><td>" + itemTracker.FolderName + "</td></tr>");
                        mailBody.Append("<tr><td><b>ItemTracker</b></td><td>" + itemTracker.ItemTrackerName + "</td></tr>");
                        mailBody.Append("<tr><td><b>Deleted By</b></td><td>" + loggedInUser + "</td></tr>");
                        mailBody.Append("<tr><td><b>Deleted On</b></td><td>" + DateTime.Now.ToApplicationFormat() + "</td></tr>");
                        mailBody.Append("</table>");
                        mailBody.Append("<p style='padding:15px;margin-top:20px;'> Click <a href='" + appUrl + "' style='cursor:pointer;color:blue'>here</a> for more details</p>");
                        break;
                }

                emailObject.ToUserIds = toUsers;
                emailObject.MailSubject = subject;
                emailObject.MailBody = mailBody.ToString();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return emailObject;
        }
    }

    public class SendEmail
    {
        EmailHelper emailHelper = new EmailHelper();

        //public System.Net.Mail.SmtpClient GetEmailConfiguration()
        //{
        //    var emailConfig = DataCache.EmailConfiguration.First();
        //    System.Net.Mail.SmtpClient mailServer = new System.Net.Mail.SmtpClient(emailConfig.ServerAddress, Convert.ToInt32(emailConfig.PortNumber));
        //    return mailServer;
        //}

        public void SendEmailtoUser(string emailType, int fileid = 0, int userid = 0, string statusFlag = "", string objectType = "", int objectId = 0, string loggedInUser = "",string toEmail = "",int itemtrackerid=0,List<UI.Models.File> files = null)
        {
            try
            {
                EmailObject emailObject = new EmailObject();
                switch (emailType)
                {
                    case "FileOperation":
                        emailObject = emailHelper.GetMailSubjectandBodyforFileOperations(fileid, statusFlag,files);
                        break;
                    case "UserPermission":
                        emailObject = emailHelper.GetMailSubjectandBodyforUserPermissions(userid, objectType, objectId, loggedInUser);
                        break;
                    case "WorkFlow":
                        emailObject = emailHelper.GetMailSubjectandBodyforWorkFlow(userid, statusFlag, fileid);
                        break;
                    case "FileShare":
                        emailObject = emailHelper.GetMailSubjectandBodyforFileShare(fileid, loggedInUser);
                        break;
                    case "ItemTrackerOperation":
                        emailObject = emailHelper.GetMailSubjectandBodyforItemTrackerOperations(itemtrackerid, statusFlag, loggedInUser);
                        break;
                }
                var emailConfig = DataCache.EmailConfiguration.First();
                System.Net.Mail.SmtpClient mailServer = new System.Net.Mail.SmtpClient(emailConfig.ServerAddress, Convert.ToInt32(emailConfig.PortNumber));
                if(emailType == "FileShare")
                {
                    SendingEmail(emailObject, mailServer, emailConfig, toEmail);
                }
                else
                {
                    if (emailObject.ToUserIds != null && emailObject.ToUserIds.Count() > 0)
                    {
                        foreach (var id in emailObject.ToUserIds.Distinct().ToList())
                        {
                            var userDetails = DataCache.Users.Where(x => x.Id == id);
                            if (userDetails != null && userDetails.Count() > 0)
                            {
                                var user = userDetails.First();
                                SendingEmail(emailObject, mailServer, emailConfig, user.EmailId);
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        public void SendingEmail(EmailObject emailObject, System.Net.Mail.SmtpClient mailServer,EmailConfiguration emailConfig,string toEmail)
        {
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(emailConfig.SenderAddress, toEmail);
                msg.Subject = emailObject.MailSubject;
                msg.Body = emailObject.MailBody;
                msg.IsBodyHtml = true;
                if (ConfigurationManager.AppSettings["IsEmailEnabled"] == "Y" && !string.IsNullOrEmpty(toEmail))
                {
                    mailServer.Send(msg);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }


    public class EmailObject
    {
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public List<int> ToUserIds { get; set; }
    }
}