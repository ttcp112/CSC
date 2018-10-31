using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace NSCSC.Shared.Utilities
{
    public class CommonHelper
    {
        public static string GetSHA512(string text)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = UE.GetBytes(text);

            SHA512Managed hashString = new SHA512Managed();
            string hex = "";

            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        public static string PublicImages = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PublicImages"]) ? "" : ConfigurationManager.AppSettings["PublicImages"];

        //public static string _UploadPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads");
        //public static string _TemplateExcelPath = System.Web.HttpContext.Current.Server.MapPath("~/ImportExportTemplate");
        //public static string _ExtractFolderPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads") + "/ExtractFolder";


        public static string _InnerException { get; set; }
        public static string _SheetName = "Sheet1";
        public static string _NeverDate = "30/12/9999";

        public static string GetUploadPath()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/Uploads");
        }
        public static string GetTemplateExcelPath()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/ImportExportTemplate");
        }
        public static string GetExtractFolderPath()
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/Uploads") + "/ExtractFolder";
        }

        /* Generate the name of export file */
        public static string GetExportFileName(string name)
        {
            return string.Format("Export_{0}_{1}", name, DateTime.Now.ToString("ddMMyy"));
        }

        /*Resize image */
        public static Bitmap ResizeImage(Image image, int width = 150, int height = 150)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        /* Convert Image -> byte[] -> base64String */
        public static string ImageToBase64(Image image)
        {
            //ImageFormat format = new ImageFormat(Guid.NewGuid());
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    // Convert Image to byte[]
                    image.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    ms.Close();
                    return base64String;

                }
                catch (Exception)
                {
                    ms.Close();
                    return "";
                }

            }
        }

        /* Extract ZipFile */
        public static void ExtractZipFile(string filePath, string serverZipExtractPath)
        {
            string zipToUnpack = filePath;
            string unpackDirectory = serverZipExtractPath;
            using (Ionic.Zip.ZipFile zip1 = Ionic.Zip.ZipFile.Read(zipToUnpack))
            {
                foreach (Ionic.Zip.ZipEntry e in zip1)
                {
                    e.Extract(unpackDirectory, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                }
            }

        }

        public static bool IsDirectoryEmpty(string path)
        {
            IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
            using (IEnumerator<string> en = items.GetEnumerator())
            {
                return !en.MoveNext();
            }
        }

        public static FileInfo[] GetListFileFromZip(HttpPostedFileBase zipFile, out string imageZipPath)
        {
            FileInfo[] listFiles = new FileInfo[] { }; //list image file in folder after extract
            imageZipPath = GetFilePath(zipFile);
            try
            {

                //upload file to server
                if (System.IO.File.Exists(imageZipPath))
                    System.IO.File.Delete(imageZipPath);
                zipFile.SaveAs(imageZipPath);

                //extract file
                //ExtractZipFile(filePath, _serverZipExtractPath);
                ExtractZipFile(imageZipPath, /*_ExtractFolderPath*/GetExtractFolderPath());
                //delete zip file after extract
                if (System.IO.File.Exists(imageZipPath))
                    System.IO.File.Delete(imageZipPath);

                if (!IsDirectoryEmpty(/*_ExtractFolderPath*/GetExtractFolderPath()))
                {

                    string[] extensions = new[] { ".jpg", ".jpeg", ".png" };
                    DirectoryInfo dInfo = new DirectoryInfo(/*_ExtractFolderPath*/GetExtractFolderPath()); //Assuming Test is your Folder
                    //Getting Text files
                    listFiles =
                        dInfo.EnumerateFiles()
                             .Where(f => extensions.Contains(f.Extension.ToLower()))
                             .ToArray();
                }
                return listFiles; ;
            }
            catch (Exception e)
            {
                _InnerException = "Error occur when unzip folder image. " + e.Message;
                return null;
            }
        }

        public static string GetFilePath(HttpPostedFileBase file)
        {
            return string.Format("{0}/{1}", /*_UploadPath*/GetUploadPath(), Path.GetFileName(file.FileName));
        }

        public static bool SaveFileExcelToServer(HttpPostedFileBase excelFile, out string filePath)
        {
            try
            {
                filePath = GetFilePath(excelFile);

                //upload file to server
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                excelFile.SaveAs(filePath);

                return true;
            }
            catch (Exception e)
            {
                _InnerException = "Error occur when save file excel to server. " + e.Message;
                filePath = "";
                return false;
            }

        }

        public static bool DeleteFileFromServer(string path, bool isImageZipFile = false)
        {
            try
            {
                //delete file excel after insert to database
                if (!isImageZipFile)
                {
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
                else
                {
                    //delete folder extract after get file.
                    if (string.IsNullOrEmpty(path))
                        path = /*_ExtractFolderPath*/GetExtractFolderPath();
                    if (System.IO.Directory.Exists(path))
                        System.IO.Directory.Delete(path, true);
                }

                return true;
            }
            catch (Exception e)
            {
                _InnerException = e.Message;
                return false;
            }
        }

        public static DataTable GetDataTableFromExcelFile(string excelPath, string templateExcelName)
        {
            OleDbConnection conn = null;
            OleDbDataAdapter da = null;
            OleDbDataAdapter tplDa = null;
            DataTable dt = null;
            DataTable tplDt = null;
            try
            {
                string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=YES; IMEX=1;'; Persist Security Info=False", excelPath);
                conn = new OleDbConnection(connectionString);
                conn.Open();

                string query = "SELECT * FROM [" + _SheetName + "$]";

                da = new OleDbDataAdapter(query, conn);
                dt = new DataTable();
                da.Fill(dt);
                conn.Close();
                // Template
                string templateExcelPath = string.Format("{0}/{1}", /*_TemplateExcelPath*/GetTemplateExcelPath(), templateExcelName);

                if (!System.IO.File.Exists(templateExcelPath))
                {
                    _InnerException = "Not have template excel file. Please add template excel file to procject first";
                    return null;
                }

                string tplConnectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=YES; IMEX=1;'; Persist Security Info=False", templateExcelPath);
                conn = new OleDbConnection(tplConnectionString);
                conn.Open();

                tplDa = new OleDbDataAdapter(query, conn);
                tplDt = new DataTable();
                tplDa.Fill(tplDt);

                if (dt.Columns.Count != tplDt.Columns.Count)
                {
                    _InnerException = Commons._MsgDoesNotMatchFileExcel;// "Your excel file was not match template excel file.";
                    return null;
                }

                List<string> listNames = new List<string>() { "store", "group/area/store", "store/area/group", "store name", "storename", "store/area/group name" };

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string tempColumnName = tplDt.Columns[i].ColumnName.Trim().ToLower();
                    if (listNames.Contains(tempColumnName))
                        continue;
                    if (dt.Columns[i].ColumnName.Trim().ToLower() != tplDt.Columns[i].ColumnName.Trim().ToLower())
                    {
                        _InnerException = Commons._MsgDoesNotMatchFileExcel;//"Your excel file was not match template excel file.";
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                _InnerException = e.Message;
            }
            finally
            {
                if (tplDt != null)
                    tplDt.Dispose();
                if (dt != null)
                    dt.Dispose();
                if (tplDa != null)
                    tplDa.Dispose();
                if (da != null)
                    da.Dispose();
                if (conn != null)
                    conn.Close();
            }

            return dt;
        }

        //public static bool CompareTemplateWithFileExcel(string templatePath, string excelPath)
        //{
        //    OleDbConnection conn = null;
        //    DataTable dt = null;
        //    DataSet ds = null;
        //    string connectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=YES; IMEX=1;'; Persist Security Info=False", excelPath);
        //    try
        //    {

        //        return true;
        //    }
        //    catch (Exception e)
        //    {

        //        return false;
        //    }
        //}

        /* Convert FileInfo Object To Image was resized, then to base64String for saving to Database  */
        public static string ConvertFileInfoToBase64String(FileInfo fileInfo)
        {
            try
            {
                //convert to image
                byte[] bytes = new byte[fileInfo.Length];
                Stream stream = fileInfo.OpenRead();
                Image imgage = Image.FromStream(stream, true, true);
                Bitmap bitmap = ResizeImage(imgage);
                stream.Close();
                return ImageToBase64(bitmap);
            }
            catch (Exception e)
            {
                string error = e.ToString();
                //logger.Error("Convert FileInfo To Byte Array Fail. Please check in Data Processing Function!" + e.Message);
                return "";
            }
        }

        /* Trim spaces */
        public static string Trim(string input)
        {
            return Regex.Replace(input, @"\s+", " ");
        }

        public static bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (regex.IsMatch(email))
            {
                return true;
            }
            return false;
        }
        public static bool IsNumber(string snumber)
        {
            Regex regex = new Regex("^[0-9]+$");
            if (regex.IsMatch(snumber))
            {
                return true;
            }
            return false;
        }

        public static DateTime ConvertToLocalTime(DateTime dateInput)
        {
            try
            {
                var date = DateTime.SpecifyKind(dateInput, DateTimeKind.Utc);
                date = date.ToLocalTime();
                return date;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ConvertToLocalTime Error", ex);
            }
            return DateTime.Now;
        }

        public static DateTime ConvertToUTCTime(DateTime dateInput)
        {
            try
            {
                var date = DateTime.SpecifyKind(dateInput, DateTimeKind.Utc);
                return date;
            }
            catch (Exception ex)
            {
                NSLog.Logger.Error("ConvertToUTCTime Error", ex);
            }
            return DateTime.Now;
        }
        public static List<SelectListItem> GetListQRCodeSize = new List<SelectListItem>()
            {
                new SelectListItem(){Value ="1", Text="100 x 100"},
                new SelectListItem(){Value ="2", Text="200 x 200"},
                new SelectListItem(){Value ="3", Text="300 x 300"},
                new SelectListItem(){Value ="4", Text="400 x 400"},
                new SelectListItem(){Value ="5", Text="500 x 500"}

            };

        public static string ReturnQRCodeSize(string value)
        {
            switch (value)
            {
                case "1":
                    return "100x100";
                case "2":
                    return "200x200";
                case "3":
                    return "300x300";
                case "4":
                    return "400x400";
                case "5":
                    return "500x500";

                default:
                    return "200x200";
            }
        }

        /// <summary>
        /// Function Encrypt for Link
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            string key = Commons.KEYENCRYPTANDDECRYPT;
            byte[] EncryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            EncryptKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            byte[] inputByte = Encoding.UTF8.GetBytes(plainText);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
            cStream.Write(inputByte, 0, inputByte.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// Function Decrypt for Link
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText)
        {
            string key = Commons.KEYENCRYPTANDDECRYPT;
            byte[] DecryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            encryptedText = encryptedText.Replace(" ", "+");
            byte[] inputByte = new byte[encryptedText.Length];
            DecryptKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            inputByte = Convert.FromBase64String(encryptedText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByte, 0, inputByte.Length);
            cs.FlushFinalBlock();
            Encoding encoding = Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type">left|right</param>
        /// <param name="lengthLimited"></param>
        /// <returns></returns>
        public static string GetMaskString(string text, string type, int lengthLimited)
        {
            string result = "";
            //int lengthLimited = 4;
            int lengthText = text.Length;
            if (lengthText < lengthLimited)
            {
                return text;
            }
            else
            {
                int lengthTextSub = lengthText - lengthLimited;
                if (type.Equals("left"))
                {
                    string value = text.Substring(lengthText - lengthLimited, lengthLimited);
                    result = value.PadLeft(lengthText, '*');
                }
                else if (type.Equals("right"))
                {
                    string value = text.Substring(0, lengthLimited);
                    result = value.PadRight(lengthText, '*');
                }
                return result;
            }
        }

        public static string PeriodName(double Period, int PeriodType)
        {
            string _PeriodName = (PeriodType == (byte)Commons.EPeriodType.OneTime)
                                                ? Commons.PeriodTypeOneTime
                                                : ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), PeriodType)).ToString()
                                                + (Period > 1 ? "s" : "");
            return _PeriodName;
        }

        public static string PeriodOfTheProduct(double Period, int PeriodType)
        {
            string _PeriodName = (PeriodType == (byte)Commons.EPeriodType.OneTime)
                                                ? Commons.PeriodTypeOneTime
                                                : Period + " " + ((Commons.EPeriodType)Enum.ToObject(typeof(Commons.EPeriodType), PeriodType)).ToString()
                                                + (Period > 1 ? "s" : "");
            return _PeriodName;
        }
    }
}
