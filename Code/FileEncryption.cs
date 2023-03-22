using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DataRooms.UI.Code
{
    public class FileEncryption
    {
        private static Logger logger = LogManager.GetLogger("myAppLoggerRules");
        public void EncryptFile(string sourcepath,string destpath)
        {
            try
            {
                using (FileStream sourceStream = new FileStream(sourcepath, FileMode.OpenOrCreate))
                {
                    byte[] iv = new byte[16];
                    byte[] buffer = new byte[1024]; // Change to suitable size after testing performance

                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = Encoding.UTF8.GetBytes("AAECAwQFBgcICQoLDA0ODw==");
                        aes.IV = iv;
                        aes.Padding = PaddingMode.Zeros;
                        CryptoStream cs;
                        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                        using (FileStream destStream = new FileStream(destpath, FileMode.OpenOrCreate))
                        {
                            cs = new CryptoStream(destStream,
                                      encryptor,
                                     CryptoStreamMode.Write);
                            int i;
                            while ((i = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cs.Write(buffer, 0, i);
                            }
                            cs.FlushFinalBlock();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error("Exception occured in File Encryption..." + ex.Message + "----" + ex.StackTrace);
                throw ex;
            }
        }

        public void DecryptFile(string encryptedfilepath,string decryptedfilepath)
        {
            try
            {
                using (FileStream sourceStream = new FileStream(encryptedfilepath, FileMode.OpenOrCreate))
                {
                    byte[] iv = new byte[16];
                    byte[] buffer = new byte[1024]; // Change to suitable size after testing performance

                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = Encoding.UTF8.GetBytes("AAECAwQFBgcICQoLDA0ODw==");
                        aes.IV = iv;
                        aes.Padding = PaddingMode.None;
                        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                        using (FileStream destStream = new FileStream(decryptedfilepath, FileMode.OpenOrCreate))
                        {
                            CryptoStream cs = new CryptoStream(destStream,
                                 decryptor,
                                CryptoStreamMode.Write);
                            int i;
                            while ((i = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cs.Write(buffer, 0, i);
                            }

                            cs.FlushFinalBlock();
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                logger.Error("Exception occured in File Decryption..." + ex.Message + "----" + ex.StackTrace);
                throw ex;
            }
        }

        


        //private static void CopywithEncryption()
        //{
        //    string source = @"C:\DBT-DATA\File types\TESTPDf.pdf";
        //    string dest = @"C:\DBT-DATA\File types\TESTPDfencrypted.pdf";

        //    using (FileStream sourceStream = new FileStream(source, FileMode.OpenOrCreate))
        //    {
        //        byte[] iv = new byte[16];
        //        byte[] buffer = new byte[1024]; // Change to suitable size after testing performance

        //        using (Aes aes = Aes.Create())
        //        {
        //            aes.Key = Encoding.UTF8.GetBytes("AAECAwQFBgcICQoLDA0ODw==");
        //            aes.IV = iv;
        //            aes.Padding = PaddingMode.Zeros;
        //            CryptoStream cs;
        //            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        //            using (FileStream destStream = new FileStream(dest, FileMode.OpenOrCreate))
        //            {
        //                cs = new CryptoStream(destStream,
        //                          encryptor,
        //                         CryptoStreamMode.Write);
        //                int i;
        //                while ((i = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
        //                {
        //                    cs.Write(buffer, 0, i);
        //                }
        //                cs.FlushFinalBlock();

        //            }


        //        }
        //    }

        //    Console.ReadKey();

        //    source = @"C:\DBT-DATA\File types\TESTPDfencrypted.pdf";

        //    dest = @"C:\DBT-DATA\File types\TESTPDfdecrypted.pdf";

        //    using (FileStream sourceStream = new FileStream(source, FileMode.OpenOrCreate))
        //    {
        //        byte[] iv = new byte[16];
        //        byte[] buffer = new byte[1024]; // Change to suitable size after testing performance

        //        using (Aes aes = Aes.Create())
        //        {
        //            aes.Key = Encoding.UTF8.GetBytes("AAECAwQFBgcICQoLDA0ODw==");
        //            aes.IV = iv;
        //            aes.Padding = PaddingMode.None;
        //            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        //            using (FileStream destStream = new FileStream(dest, FileMode.OpenOrCreate))
        //            {
        //                CryptoStream cs = new CryptoStream(destStream,
        //                     decryptor,
        //                    CryptoStreamMode.Write);
        //                int i;
        //                while ((i = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
        //                {
        //                    cs.Write(buffer, 0, i);
        //                }

        //                cs.FlushFinalBlock();
        //            }
        //        }

        //    }
        //}
    }
}