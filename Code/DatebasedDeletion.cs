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
    public class DatebasedDeletion
    {
        public async Task DeleteFiles()
        {
            try
            {
                DataCacheLoadService dataCacheLoadService = new DataCacheLoadService();
                List<File> filestobeDeleted = new List<File>();
                var workSpacePath = ConfigurationManager.AppSettings["WorkspacePath"];
                var files = DataCache.Files.Where(x => x.IsActive == false && x.DeletedBy != null);
                if(files!=null && files.Count() > 0)
                {
                    foreach(var file in files.ToList())
                    {
                        var fileDeletionDate = file.DeletedOn;
                        var todayData = DateTime.Now.Date;
                        if (fileDeletionDate != null)
                        {
                            if((fileDeletionDate.Value.Date - todayData).Days >= 30)
                            {
                                var filePath = System.IO.Path.Combine(workSpacePath, file.RelativePath);
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                                // Delete Files....
                                file.IsDeleted = true;
                                file.DeletedOn = DateTime.Now;
                                file.DeletorName = "System";
                                filestobeDeleted.Add(file);
                                DataCache.RemoveFilefromCache(file);
                            }
                        }
                    }
                }

                if (filestobeDeleted.Count > 0)
                {
                    await dataCacheLoadService.DeleteFiles(filestobeDeleted);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}