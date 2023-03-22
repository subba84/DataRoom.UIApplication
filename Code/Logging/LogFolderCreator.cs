using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace DataRooms.UI.Code.Logging
{
    public static class LogFolderCreator
    {
        public static void LogPathCreation(string logPath)
        {
            try
            {
                //string logFolderPath = System.IO.Path.Combine(logPath, "Logs");
                if (!System.IO.Directory.Exists(logPath))
                {
                    System.IO.Directory.CreateDirectory(logPath);
                }
                var activityLogFilePath = logPath + "/ActivityLogXML.xml";
                var dataLogFilePath = logPath + "/DataLogXML.xml";
                CreateXMLFile(activityLogFilePath, "ActivityLogs");
                CreateXMLFile(dataLogFilePath, "DataLogs");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static void CreateXMLFile(string filepath,string startElement)
        {
            try
            {
                if (!System.IO.File.Exists(filepath))
                {
                    using (XmlWriter writer = XmlWriter.Create(filepath))
                    {
                        writer.WriteStartElement(startElement);
                        writer.WriteEndElement();
                        writer.Flush();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}