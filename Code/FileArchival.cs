using DataRooms.UI.Code.Helpers;
using DataRooms.UI.Models;
using DataRooms.UI.WebApiHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DataRooms.UI.Code
{
    public class FileArchival
    {
        public async Task ArchiveFiles()
        {
            try
            {
                DataCacheLoadService dataCacheLoadService = new DataCacheLoadService();
                //var workSpacePath = ConfigurationManager.AppSettings["WorkspacePath"];
                var archivalPath = string.Empty;// ConfigurationManager.AppSettings["ArchivalPath"];
                List<File> filestobeArchived = new List<File>();
                DateTime currentDate = DateTime.Now.Date;
                var datarooms = DataCache.DataRooms.Where(x=>x.IsArchivalRequired == true);
                if(datarooms!=null && datarooms.Count() > 0)
                {
                    foreach(var dataroom in datarooms.ToList())
                    {
                        S3Helper s3Helper = null;
                        if (dataroom.ArchivalType == "AWS")
                        {
                            s3Helper = new S3Helper(dataroom.ArchivalAWSAccessKey, dataroom.ArchivalAWSSecurityKey,dataroom.ArchivalAWSRegion,dataroom.ArchivalPath);
                        }
                        
                        archivalPath = System.IO.Path.Combine(dataroom.RelativePath,"Archive");                        
                        int dataroomArchivalPeriod = Convert.ToInt32(dataroom.ArchivalPeriod);
                        var files = DataCache.Files.Where(x=> x.DataRoomId == dataroom.Id && x.IsArchived == false && x.IsDeleted == false);
                        if(files!=null && files.Count() > 0)
                        {
                            foreach(var file in files.ToList())
                            {
                                var fileCreationDate = dataroom.ArchivalBasedOn == "Created Date" ? file.CreatedOn.Date : (file.ModifiedOn == null ? file.CreatedOn.Date : file.ModifiedOn.Value.Date);// (file.ModifiedOn != null ? file.ModifiedOn.Value.Date : file.CreatedOn.Date);
                                if((currentDate - fileCreationDate).TotalDays >= dataroomArchivalPeriod)
                                {
                                    // Move Fle to Archival Path
                                    var filePath = file.RelativePath;
                                    var archivalFilePath = System.IO.Path.Combine(archivalPath, file.FileName);

                                    if (dataroom.ArchivalType == "AWS")
                                    {
                                        s3Helper.MoveFile(filePath, archivalFilePath);
                                    }
                                    else
                                    {
                                        System.IO.File.Move(filePath, archivalFilePath);
                                    }
                                    // Update File as Archived....
                                    file.IsArchived = true;
                                    file.ArchivedOn = DateTime.Now;
                                    filestobeArchived.Add(file);
                                    DataCache.RefreshSingleFile(file);
                                }
                            }
                        }
                    }
                }

                if(filestobeArchived.Count > 0)
                {
                    await dataCacheLoadService.UpdateFiles(filestobeArchived);
                }
            }
            catch(Exception ex)
            {
                //throw ex;
            }
        }
    }
}