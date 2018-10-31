using NSCSC.Shared;
using NSCSC.Shared.Models;
using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace NuWebNCloud.Shared.Utilities
{
    public static class FTP
    {
        //private static string _ftpHost = ConfigurationManager.AppSettings["FTPHost"] + "/";
        //private static string _userName = ConfigurationManager.AppSettings["FTPUser"];
        //private static string _password = ConfigurationManager.AppSettings["FTPPassword"];

        //private static string _ftpHost = Commons.FTPHost + "/";
        //private static string _userName = Commons.FTPUser;
        //private static string _password = Commons.FTPPassword;

        public static bool CopyFile(string fileName, string fileToCopy)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Commons.FTPHost + fileName);
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                request.Credentials = new NetworkCredential(Commons.FTPUser, Commons.FTPPassword);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                Upload(fileToCopy, ToByteArray(responseStream));
                if (responseStream != null)
                    responseStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return false;
            }
        }

        public static Byte[] ToByteArray(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] chunk = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(chunk, 0, chunk.Length)) > 0)
            {
                ms.Write(chunk, 0, bytesRead);
            }
            return ms.ToArray();
        }

        public static bool Upload(string fileName, byte[] image)
        {
            try
            {
                FtpWebRequest clsRequest = (FtpWebRequest)WebRequest.Create(Commons.FTPHost + fileName);
                clsRequest.Credentials = new NetworkCredential(Commons.FTPUser, Commons.FTPPassword);
                clsRequest.Method = WebRequestMethods.Ftp.UploadFile;
                Stream clsStream = clsRequest.GetRequestStream();
                clsStream.Write(image, 0, image.Length);
                clsStream.Close();
                clsStream.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return false;
            }
        }

        public static bool UploadClient(string fileName, byte[] image)
        {
            try
            {
                FtpWebRequest clsRequest = (FtpWebRequest)WebRequest.Create(Commons.FTPHostClient + "/" + fileName);
                clsRequest.Credentials = new NetworkCredential(Commons.FTPUserClient, Commons.FTPPasswordClient);
                clsRequest.Method = WebRequestMethods.Ftp.UploadFile;
                Stream clsStream = clsRequest.GetRequestStream();
                clsStream.Write(image, 0, image.Length);
                clsStream.Close();
                clsStream.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.ToString();
                return false;
            }
        }
    }
}
