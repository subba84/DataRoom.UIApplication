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
    public class FileDeletion
    {
        public async Task DeleteFiles()
        {
            try
            {
                DataCacheLoadService dataCacheLoadService = new DataCacheLoadService();
                //var workSpacePath = ConfigurationManager.AppSettings["WorkspacePath"];
                List<File> filestobeDeleted = new List<File>();
                DateTime currentDate = DateTime.Now.Date;
                var datarooms = DataCache.DataRooms.Where(x => x.IsDeletionRequired == true);
                if (datarooms != null && datarooms.Count() > 0)
                {
                    foreach (var dataroom in datarooms.ToList())
                    {
                        int dataroomDeletionPeriod = Convert.ToInt32(dataroom.DeletionPeriod);
                        var files = DataCache.Files.Where(x => x.DataRoomId == dataroom.Id && x.IsDeleted == false);
                        if (files != null && files.Count() > 0)
                        {
                            foreach (var file in files)
                            {
                                var fileCreationDate = dataroom.DeletionBasedOn == "Created Date" ? file.CreatedOn.Date : (file.ModifiedOn == null ? file.CreatedOn.Date : file.ModifiedOn.Value.Date);// (file.ModifiedOn != null ? file.ModifiedOn.Value.Date : file.CreatedOn.Date);
                                //var fileCreationDate = (file.ModifiedOn != null ? file.ModifiedOn.Value.Date : file.CreatedOn.Date);
                                if ((currentDate - fileCreationDate).TotalDays >= dataroomDeletionPeriod)
                                {
                                    // delete file
                                    var filePath = file.RelativePath;
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(filePath);
                                    }

                                    // Update File as Archived....
                                    file.IsDeleted = true;
                                    file.DeletedOn = DateTime.Now;
                                    filestobeDeleted.Add(file);
                                    DataCache.RemoveFilefromCache(file);
                                }
                            }
                        }
                    }
                }

                if (filestobeDeleted.Count > 0)
                {
                    await dataCacheLoadService.UpdateFiles(filestobeDeleted);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}