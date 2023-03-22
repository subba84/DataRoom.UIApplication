using Amazon.S3.Model;
using DataRooms.UI.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DataRooms.UI.Areas.Files.Controllers
{
    public class ExternalFileDownloadController : Controller
    {
        
        FileEncryption encryption;
        FileManager fileManager;
        public ExternalFileDownloadController()
        {
            encryption = new FileEncryption();
            fileManager = new FileManager(string.Empty, 0);
        }
        public void DownloadFile(int fileid)
        {
            try
            {
                var file = DataCache.Files.Single(x => x.Id == fileid);
                string fullfilepath = file.RelativePath;
                //byte[] filebyteArray = System.IO.File.ReadAllBytes(fullfilepath);
                fileManager = new FileManager(string.Empty, file.CompanyId);
                byte[] filebyteArray = fileManager.GetFileByteArray(fullfilepath);
                if (filebyteArray != null)
                {
                    string tempfilepath = Server.MapPath("~/Temp/") + file.FileName;
                    System.IO.File.WriteAllBytes(tempfilepath, filebyteArray);
                    var decryptedfilepath = Server.MapPath("~/Temp/EncryptionTemp/") + file.FileName;
                    encryption.DecryptFile(tempfilepath, decryptedfilepath);
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition", "attachment;filename=" + file.FileName);
                    byte[] data = System.IO.File.ReadAllBytes(decryptedfilepath);
                    response.BinaryWrite(data);
                    response.End();
                }
                else
                {
                    throw new Exception("File not found");
                }
            }
            catch (Exception ex)
            {
               throw new Exception("File not found");
            }
        }
    }
}