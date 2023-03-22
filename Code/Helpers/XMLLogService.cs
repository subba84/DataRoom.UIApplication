using DataRooms.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace DataRooms.UI.Code.Helpers
{
    public class XMLLogService
    {
        string _activityLogFilePath;
        string _dataLogFilePath;
        public XMLLogService(string logspath)
        {
            _activityLogFilePath = logspath + "//ActivityLogXML.xml"; 
            _dataLogFilePath = logspath + "//DataLogXML.xml";
        }

        static IEnumerable<XElement> ActivityLogStreamRootChildDoc(string uri)
        {
            using (XmlReader reader = XmlReader.Create(uri))
            {
                reader.MoveToContent();

                // Parse the file and return each of the nodes.
                while (!reader.EOF)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "ActivityLog")
                    {
                        XElement el = XElement.ReadFrom(reader) as XElement;
                        if (el != null)
                            yield return el;
                    }
                    else
                    {
                        reader.Read();
                    }
                }
            }
        }

        static IEnumerable<XElement> DataLogStreamRootChildDoc(string uri)
        {
            using (XmlReader reader = XmlReader.Create(uri))
            {
                reader.MoveToContent();

                // Parse the file and return each of the nodes.
                while (!reader.EOF)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "DataLog")
                    {
                        XElement el = XElement.ReadFrom(reader) as XElement;
                        if (el != null)
                            yield return el;
                    }
                    else
                    {
                        reader.Read();
                    }
                }
            }
        }

        public static Task<XDocument> LoadAsync(string path, LoadOptions loadOptions = LoadOptions.PreserveWhitespace)
        {
            return Task.Run(() =>
            {
                using (var stream = System.IO.File.OpenText(path))
                {
                    return XDocument.Load(stream, loadOptions);
                }
            });
        }

        public IEnumerable<ActivityLog> GetActivityLogs()
        {
            try
            {
                IEnumerable<ActivityLog> fileDetails = (from item in ActivityLogStreamRootChildDoc(_activityLogFilePath)
                                                        select new ActivityLog
                                                        {
                                                            Id = Convert.ToInt32(item.Element("Id").Value),
                                                            ActivityCategory = Convert.ToString(item.Element("ActivityCategory").Value),
                                                            ActivityDescription = Convert.ToString(item.Element("ActivityDescription").Value),
                                                            DataRoomId = !item.Element("DataRoomId").IsEmpty ? Convert.ToInt32(item.Element("DataRoomId").Value) : 0,
                                                            DataRoomName = Convert.ToString(item.Element("DataRoomName").Value),
                                                            FolderId = !item.Element("FolderId").IsEmpty ? Convert.ToInt32(item.Element("FolderId").Value) : 0,
                                                            FolderName = Convert.ToString(item.Element("FolderName").Value),
                                                            FileId = !item.Element("FileId").IsEmpty ? Convert.ToInt32(item.Element("FileId").Value) : 0,
                                                            FileName = Convert.ToString(item.Element("FileName").Value),
                                                            IsActive = Convert.ToBoolean(item.Element("IsActive").Value),
                                                            CreatedBy = Convert.ToInt32(item.Element("CreatedBy").Value),
                                                            CreatorName = Convert.ToString(item.Element("CreatorName").Value),
                                                            CreatedOn = Convert.ToDateTime(item.Element("CreatedOn").Value),
                                                            ModifiedBy = !item.Element("ModifiedBy").IsEmpty ? Convert.ToInt32(item.Element("ModifiedBy").Value) : 0,
                                                            ModifierName = Convert.ToString(item.Element("ModifierName").Value),
                                                            ModifiedOn = Convert.ToString(item.Element("ModifiedOn").Value) == "" ? DateTime.MinValue : Convert.ToDateTime(item.Element("ModifiedOn").Value),
                                                            DeletedBy = !item.Element("DeletedBy").IsEmpty ? Convert.ToInt32(item.Element("DeletedBy").Value) : 0,
                                                            DeletorName = Convert.ToString(item.Element("DeletorName").Value),
                                                            DeletedOn = Convert.ToString(item.Element("DeletedOn").Value) == "" ? DateTime.MinValue : Convert.ToDateTime(item.Element("DeletedOn").Value)
                                                        }).OrderByDescending(x=>x.Id);
                return fileDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public IEnumerable<DataLog> GetDataLogsbyActivityId(int activityid)
        {
            try
            {
                IEnumerable<DataLog> fileDetails = (from item in DataLogStreamRootChildDoc(_dataLogFilePath)
                                                    where Convert.ToInt32(item.Element("ActivityLogId").Value) == activityid
                                                    select new DataLog
                                                    {
                                                        Id = Convert.ToInt32(item.Element("Id").Value),
                                                        ActivityLogId = Convert.ToInt32(item.Element("ActivityLogId").Value),
                                                        OriginalData = Convert.ToString(item.Element("OriginalData").Value),
                                                        ModifiedData = Convert.ToString(item.Element("ModifiedData").Value),
                                                        Changes = Convert.ToString(item.Element("Changes").Value),
                                                        TableName = Convert.ToString(item.Element("TableName").Value),
                                                        IsActive = Convert.ToBoolean(item.Element("IsActive").Value),
                                                        CreatedBy = Convert.ToInt32(item.Element("CreatedBy").Value),
                                                        CreatorName = Convert.ToString(item.Element("CreatorName").Value),
                                                        CreatedOn = Convert.ToDateTime(item.Element("CreatedOn").Value),
                                                        ModifiedBy = !item.Element("ModifiedBy").IsEmpty ? Convert.ToInt32(item.Element("ModifiedBy").Value) : 0,
                                                        ModifierName = Convert.ToString(item.Element("ModifierName").Value),
                                                        ModifiedOn = Convert.ToString(item.Element("ModifiedOn").Value) == "" ? DateTime.MinValue : Convert.ToDateTime(item.Element("ModifiedOn").Value),
                                                        DeletedBy = !item.Element("DeletedBy").IsEmpty ? Convert.ToInt32(item.Element("DeletedBy").Value) : 0,
                                                        DeletorName = Convert.ToString(item.Element("DeletorName").Value),
                                                        DeletedOn = Convert.ToString(item.Element("DeletedOn").Value) == "" ? DateTime.MinValue : Convert.ToDateTime(item.Element("DeletedOn").Value)
                                                    });

                return fileDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> CreateActivityLog(ActivityLog log)
        {
            try
            {
                XDocument xmlDoc = await LoadAsync(_activityLogFilePath);
                var count = xmlDoc.Descendants("ActivityLog").Count();
                int autoid = count + 1;
                xmlDoc.Element("ActivityLogs").Add(new XElement("ActivityLog",
                    new XElement("Id", autoid),
                    new XElement("ActivityCategory", log.ActivityCategory),
                    new XElement("ActivityDescription", log.ActivityDescription),
                    new XElement("DataRoomId", log.DataRoomId),
                    new XElement("DataRoomName", log.DataRoomName),
                    new XElement("FolderId", log.FolderId),
                    new XElement("FolderName", log.FolderName),
                    new XElement("FileId", log.FileId),
                    new XElement("FileName", log.FileName),
                    new XElement("IsActive", log.IsActive),
                    new XElement("CreatedBy", log.CreatedBy),
                    new XElement("CreatorName", log.CreatorName),
                    new XElement("CreatedOn", log.CreatedOn),
                    new XElement("ModifiedBy", log.ModifiedBy),
                    new XElement("ModifierName", log.ModifierName),
                    new XElement("ModifiedOn", log.ModifiedOn),
                    new XElement("DeletedBy", log.DeletedBy),
                    new XElement("DeletorName", log.DeletorName),
                    new XElement("DeletedOn", log.DeletedOn)));
                xmlDoc.Save(_activityLogFilePath);
                return autoid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateDataLog(DataLog log)
        {
            try
            {
                XDocument xmlDoc = await LoadAsync(_dataLogFilePath);
                xmlDoc.Element("DataLogs").Add(new XElement("DataLog",
                    new XElement("Id", log.Id),
                    new XElement("ActivityLogId", log.ActivityLogId),
                    new XElement("DataRoomId", log.DataRoomId),
                    new XElement("TableName", log.TableName),
                    new XElement("OriginalData", log.OriginalData),
                    new XElement("ModifiedData", log.ModifiedData),
                    new XElement("Changes", log.Changes),
                    new XElement("IsActive", log.IsActive),
                    new XElement("CreatedBy", log.CreatedBy),
                    new XElement("CreatorName", log.CreatorName),
                    new XElement("CreatedOn", log.CreatedOn),
                    new XElement("ModifiedBy", log.ModifiedBy),
                    new XElement("ModifierName", log.ModifierName),
                    new XElement("ModifiedOn", log.ModifiedOn),
                    new XElement("DeletedBy", log.DeletedBy),
                    new XElement("DeletorName", log.DeletorName),
                    new XElement("DeletedOn", log.DeletedOn)));
                xmlDoc.Save(_dataLogFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteAllLogsofDataRoom(int dataroomid)
        {
            try
            {
                XDocument xmlDoc = await LoadAsync(_activityLogFilePath);
                var fileDetails = (from item in ActivityLogStreamRootChildDoc(_activityLogFilePath)
                                   where Convert.ToInt32(item.Element("DataRoomId").Value) == dataroomid
                                   select item);
                if (fileDetails != null && fileDetails.Count() > 0)
                {
                    foreach (var item in fileDetails)
                    {
                        item.Remove();
                    }
                }
                xmlDoc.Save(_activityLogFilePath);

                var datalogs = (from datalog in xmlDoc.Descendants("DataLog")
                                where Convert.ToInt32(datalog.Element("DataRoomId").Value) == dataroomid
                                select datalog);
                if (datalogs != null && datalogs.Count() > 0)
                {
                    foreach (var dataLog in datalogs)
                    {
                        dataLog.Remove();
                    }
                }
                xmlDoc.Save(_dataLogFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteLogsofDataRoombyActivityIds(List<int> activityids)
        {
            try
            {
                XDocument xmlDoc = await LoadAsync(_activityLogFilePath);
                var fileDetails = (from item in ActivityLogStreamRootChildDoc(_activityLogFilePath)
                                   where activityids.Contains(Convert.ToInt32(item.Element("ActivityLogId").Value))
                                   select item);
                if (fileDetails != null && fileDetails.Count() > 0)
                {
                    foreach (var item in fileDetails)
                    {
                        item.Remove();
                    }
                }
                xmlDoc.Save(_activityLogFilePath);

                var datalogs = (from datalog in xmlDoc.Descendants("DataLog")
                                where activityids.Contains(Convert.ToInt32(datalog.Element("ActivityLogId").Value))
                                select datalog);
                if (datalogs != null && datalogs.Count() > 0)
                {
                    foreach (var dataLog in datalogs)
                    {
                        dataLog.Remove();
                    }
                }
                xmlDoc.Save(_dataLogFilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
