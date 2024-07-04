using GroupDocs.Conversion.Config;
using GroupDocs.Conversion.Handler;
using GroupDocs.Conversion.Options.Save;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using VNPTPM.Model.Core;
using VNPTPM.Model.VM;
using static GroupDocs.Conversion.Options.Save.ImageSaveOptions;


namespace VNPTPM.Model.Commons
{
    public class VNPTHelper
    {
        public static string GetServiceName()
        {
            var request = HttpContext.Current.Request;
            var filePath = request.FilePath;

            return string.IsNullOrEmpty(filePath) ? string.Empty : filePath;
        }
        public static string GetUploadPath()
        {
            var url = "";
            var request = HttpContext.Current.Request;
            if (request.IsSecureConnection)
                url = "https://";
            else
                url = "http://";

            url += $"{request["HTTP_HOST"]}/";

            return url;
        }

        public static string GetUnsigned(string str)
        {

            string[] vietNamChar = new string[]
               {
                    "aAeEoOuUiIdDyY",
                    "áàạảãâấầậẩẫăắằặẳẵ",
                    "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                    "éèẹẻẽêếềệểễ",
                    "ÉÈẸẺẼÊẾỀỆỂỄ",
                    "óòọỏõôốồộổỗơớờợởỡ",
                    "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                    "úùụủũưứừựửữ",
                    "ÚÙỤỦŨƯỨỪỰỬỮ",
                    "íìịỉĩ",
                    "ÍÌỊỈĨ",
                    "đ",
                    "Đ",
                    "ýỳỵỷỹ",
                    "ÝỲỴỶỸ"
               };

            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < vietNamChar.Length; i++)
            {
                for (int j = 0; j < vietNamChar[i].Length; j++)
                    str = str.Replace(vietNamChar[i][j], vietNamChar[0][i - 1]);
            }
            return str;
        }

        public static string GetPrefix(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            name = GetUnsigned(name);

            var result = "";
            var array = name.Split(new string[] { " " }, StringSplitOptions.None).ToList();
            if (array.Count > 0)
            {
                foreach (var item in array)
                {
                    if (item.Length > 0)
                        result += item.Substring(0, 1);
                }
            }

            return result.ToLower();
        }

        public static long GetMaxAutoNumber(IRepository repository, string funcName)
        {
            AD_AUTONUMBER ob = repository.GetByKey<AD_AUTONUMBER>(funcName);
            if (ob == null)
            {
                return 0;
            }

            return ob.Current.GetValueOrDefault();
        }

        public static string getRootDir()
        {
            string root = "";
            string serverNm = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
            string port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            string protocol = ((HttpContext.Current.Request.ServerVariables["HTTPS"]) == "ON") ? "https" : "http";
            root = string.Concat(protocol, "://", serverNm, ":", port, "/");

            return root;
        }

        public static string SubStringCutRight(string str, int start, int lengthRinght)
        {
            string text = "";
            if (!string.IsNullOrEmpty(str))
            {
                text = str.Substring(start, str.Length - lengthRinght);
            }
            return text;
        }

        public static string GetAutoNumber(IRepository repository, string funcName, long? maxNumber)
        {
            string kq = "";
            AD_AUTONUMBER ob = repository.GetByKey<AD_AUTONUMBER>(funcName);

            if (ob != null)
            {
                ob = (AD_AUTONUMBER)ob.Clone();
                //if (ob.Reset == true)
                //{
                //    AD_CONFIG tsNgayDangNhap = repository.GetQuery<AD_CONFIG>().FirstOrDefault(o => funcName.Equals(o.ID));
                //    if (tsNgayDangNhap == null)
                //    {
                //        tsNgayDangNhap = new AD_CONFIG();
                //        tsNgayDangNhap.ID = funcName;
                //        tsNgayDangNhap.Name = DateTime.Today.ToString("yyyyMMdd");

                //        repository.Add(tsNgayDangNhap);

                //        ob.Current = 0;
                //    }
                //    else
                //    {
                //        if (!DateTime.Today.ToString("yyyyMMdd").Equals(tsNgayDangNhap.Name))
                //        {
                //            tsNgayDangNhap.Name = DateTime.Today.ToString("yyyyMMdd");

                //            repository.Update(tsNgayDangNhap);

                //            ob.Current = 0;
                //        }
                //    }
                //}

                if (ob.Step == 0)
                {
                    ob.Step = 1;
                }

                ob.Current = (maxNumber != null ? maxNumber : ob.Current) + ob.Step;
                if (string.IsNullOrEmpty(ob.Format))
                {
                    ob.Format = string.Empty;
                    kq = (ob.Current).ToString();
                }
                else
                {
                    //[BN-][$yyyy][-][00000]
                    List<string> lst = new List<string>();
                    string str = "";
                    for (int i = 0; i < ob.Format.Length; i++)
                    {
                        if (ob.Format[i] == '[') str = "";
                        else if (ob.Format[i] == ']') lst.Add(str);
                        else str += ob.Format[i];
                    }
                    if (lst.Count == 0) lst.Add(str);
                    foreach (string s in lst)
                    {
                        if (s.Contains("$"))
                        {
                            string f = s.Replace("$", "");
                            kq += DateTime.Now.ToString(f);
                        }
                        else if (s.Any(o => o != '0') == false)
                        {
                            kq += ((double)ob.Current).ToString(s);
                        }
                        else kq += s;
                    }
                }

                repository.Update(ob);
            }
            else
            {
                ob = new AD_AUTONUMBER()
                {
                    ID = funcName,
                    Format = "",
                    Current = 1,
                    Step = 1
                };

                repository.Add(ob);

                kq += ((double)ob.Current).ToString(ob.Format);
            }
            return kq;
        }

        public static string GetNumberPixel(string pValue)
        {
            var result = "";
            foreach (Char c in pValue.Trim())
            {
                if (';'.Equals(c))
                {
                    continue;
                }

                if (Char.IsDigit(c))
                {
                    result += c;
                }
            }
            return result.Trim();
        }

        public static int ParseInt(string value)
        {
            int result = 0;
            int.TryParse(value, out result);

            return result;
        }

        public static DateTime setCorrectDate(DateTime date1, DateTime date2)
        {
            var finalDate = new DateTime(date1.Year, date1.Month, date1.Day, date2.Hour, date2.Minute, date2.Second);

            return finalDate;
        }

        public static double ParseDouble(string value)
        {
            //replace "." => "," to convert correctly
            value = value.Replace(".", ",");
            double result = 0;
            double.TryParse(value, out result);

            return result;
        }

        public static string DoubleToString(double value)
        {
            //replace "," => "." to convert correctly
            string result = "";
            result = value.ToString("R").Replace(",", ".");

            return result;
        }

        public static decimal ParseDecimal(string value)
        {
            decimal.TryParse(value, out decimal result);

            return result;
        }

        public static string GetUserName()
        {
            var request = HttpContext.Current.Request;
            var userName = request.Headers["UserName"];

            return string.IsNullOrEmpty(userName) ? string.Empty : userName;
        }

        public static Image Base64ToImage(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                Image image = Image.FromStream(ms, true);
                return image;
            }
            catch { }
            return null;

        }
        public static string ImageToBases64String(string pathFull)
        {
            string base64String = "";
            if (File.Exists(pathFull))
            {
                using (Image image = Image.FromFile(pathFull))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        var ext = Path.GetExtension(pathFull).Replace(".", "");
                        base64String = Convert.ToBase64String(imageBytes);
                        base64String = string.Concat("data:image/", ext, ";base64,", base64String);
                    }
                }
            }

            return base64String;
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public static string ByteArrayImageToBase64(byte[] byteArrayIn)
        {
            try
            {

                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                using (MemoryStream m = new MemoryStream())
                {
                    returnImage.Save(m, returnImage.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }

            }
            catch
            {
                return "";
            }

        }

        //public static void Compress(DirectoryInfo directorySelected, string directoryPath, string fileName)
        //{
        //    var zipName = $"{directoryPath}/{fileName}.zip";
        //    if (File.Exists(zipName))
        //    {
        //        File.Delete(zipName);
        //    }

        //    using (ZipFile zip = new ZipFile())
        //    {
        //        zip.AddDirectory(@"" + directoryPath, @"" + fileName);
        //        zip.Save(@"" + $"{directoryPath}/{fileName}.zip");
        //    }
        //}

        public static List<int> CommaJsonStringToIntList(string _s)
        {
            List<int> list = (List<int>)JsonConvert.DeserializeObject(_s, typeof(List<int>));

            return (list);
        }

        public static string SaveBase64ToImage(string base64String, string pathSaveImage)
        {
            var server = HttpContext.Current.Server;
            var folderBanner = $"{VNPTConfigs.DirDocumentUpload}/{pathSaveImage}";
            //server.MapPath(String.Concat("~", "/", pathSaveImage));
            string imageNm = String.Concat("IMG", DateTime.Now.ToString(Constants.LBM_FORMAT_SAVE_NAME_FILE), ".jpeg");

            if (!string.IsNullOrEmpty(base64String))
            {
                Image image = VNPTHelper.Base64ToImage(base64String);
                if (image != null)
                {
                    bool exists = System.IO.Directory.Exists(folderBanner);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(folderBanner);
                    string fullPath = String.Concat(folderBanner, imageNm);
                    image.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    FileInfo file = new FileInfo(fullPath);
                    if (file.Exists == true)
                    {
                        if (file.Length > Constants.LBM_MAX_SIZE_IMAGE)
                        {
                            return EStatusFile.OverWeight.ToString();
                        }
                    }

                    return imageNm;
                }
                else
                {
                    return EStatusFile.Base64Incorrect.ToString();
                }
            }
            else
            {
                return null;
            }
        }

        public static void DeleteFile(string pathSaveFile, string fileNm)
        {
            var server = HttpContext.Current.Server;
            var folderFile = $"{VNPTConfigs.DirDocumentUpload}/{pathSaveFile}";
            //server.MapPath(String.Concat("~", "/", pathSaveFile));
            string fullPath = String.Concat(folderFile, fileNm);
            FileInfo file = new FileInfo(fullPath);
            if (file.Exists == true)
            {
                File.Delete(fullPath);
            }
        }

        public static void DeleteListFileRemove(string pathSaveFile, string listStringOld, string listStringNew)
        {
            var server = HttpContext.Current.Server;
            var folderFile = $"{VNPTConfigs.DirDocumentUpload}/{pathSaveFile}";
            //server.MapPath(String.Concat("~", "/", pathSaveFile));

            List<string> listOld = new List<string>();
            if (!string.IsNullOrEmpty(listStringOld))
            {
                listOld = (List<string>)JsonConvert.DeserializeObject(listStringOld, typeof(List<string>));
            }
            List<string> listNew = new List<string>();
            if (!string.IsNullOrEmpty(listStringNew) && listStringNew != Constants.LBM_VALUE_DETECT_INPUT_BLANK_STRING)
            {
                listNew = (List<string>)JsonConvert.DeserializeObject(listStringNew, typeof(List<string>));
            }

            foreach (string oldFile in listOld)
            {
                if (!listNew.Contains(oldFile))
                {
                    if (isExistFile(pathSaveFile, oldFile))
                    {
                        DeleteFile(pathSaveFile, oldFile);
                    }
                }
            }
        }

        public static bool isExistFile(string pathSaveFile, string fileNm)
        {
            if (string.IsNullOrEmpty(System.IO.Path.Combine(pathSaveFile, fileNm)))
            {
                var server = HttpContext.Current.Server;
                var folderFile = $"{VNPTConfigs.DirDocumentUpload}/{pathSaveFile}";
                //server.MapPath(String.Concat("~", "/", pathSaveFile));
                FileInfo file = new FileInfo(System.IO.Path.Combine(folderFile, fileNm));
                if (file.Exists == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        // Kiểm tra file có đc phép sửa ko?
        // Truyền mảng tên file để so sánh
        public static bool IsExtensionFileAccepted(List<string> compareFiles)
        {
            var compareResult = Constants.ACCEPT_EXTENSION_FILES.Where(
                        extensionFile => compareFiles.Any(compareFile => compareFile.Contains(extensionFile)));
            if (compareResult.Count() != 0)
            {
                return true;
            }
            return false;
        }
        public static string CoppyFile(string sourcePath, string targetPath, string fileName, string newFileName = null)
        {
            bool isExtensionFileAccepted = IsExtensionFileAccepted(new List<string> { sourcePath, targetPath, fileName });
            if (!isExtensionFileAccepted)
            {
                throw new Exception(VNPTResources.Instance.Get(VNPTResources.ID.MsgFileUploadNotAccept));
            }
            var server = HttpContext.Current.Server;
            sourcePath = $"{VNPTConfigs.DirDocumentUpload}/{sourcePath}";
            //server.MapPath(String.Concat("~", "/", sourcePath));
            targetPath = $"{VNPTConfigs.DirDocumentUpload}/{targetPath}";
            //server.MapPath(String.Concat("~", "/", targetPath));

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            if (!string.IsNullOrEmpty(newFileName))
            {
                fileName = newFileName;
            }
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location:
            // Create a new target folder. 
            // If the directory already exists, this method does not create a new directory.
            System.IO.Directory.CreateDirectory(targetPath);

            // To copy all the files in one directory to another directory.
            // Get the files in the source folder. (To recursively iterate through
            // all subfolders under the current directory, see
            // "How to: Iterate Through a Directory Tree.")
            // Note: Check for target path was performed previously
            //       in this code example.
            if (System.IO.Directory.Exists(sourcePath))
            {
                // To copy a file to another location and 
                // overwrite the destination file if it already exists.
                System.IO.File.Copy(sourceFile, destFile, true);
                //System.IO.File.Delete(sourceFile);
            }
            else
            {
                return EStatusFile.PathNotExist.ToString();
            }

            return fileName;
        }

        public static bool IsNullGuid(Guid? value)
        {
            if (value == null || value == Guid.Empty)
            {
                return true;
            }

            return false;
        }

        public static string GeneratePassword(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }

        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        public static string CreateFileFromBytes(byte[] fileBytes, string typeData, string AccountID, string fileNameOld, ref string errorMsg)
        {
            if (fileBytes != null && AccountID != null)
            {
                //var server = HttpContext.Current.Server;
                string pathSaveMedia = string.Format(Constants.WO_PATH_SAVE_FILE_TMP, AccountID);
                var folderMedia = $"{VNPTConfigs.DirDocumentUpload}/{pathSaveMedia}";
                string fileName = String.Concat(DateTime.Now.ToString(Constants.WO_FORMAT_SAVE_NAME_FILE), "_", fileNameOld);
                string fullPath = String.Concat(folderMedia, fileName);
                bool exists = System.IO.Directory.Exists(folderMedia);
                if (!exists)
                    System.IO.Directory.CreateDirectory(folderMedia);

                //FileInfo file = new FileInfo(fullPath);
                if (typeData.Equals(Constants.WO_TYPE_DETECT_IMAGE))
                {
                    if (fileBytes.Length > Constants.WO_MAX_SIZE_IMAGE)
                    {
                        errorMsg = string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorImageUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                            Constants.LBM_MAX_SIZE_IMAGE));

                        return null;
                    }
                }
                else if (typeData.Equals(Constants.WO_TYPE_DETECT_VIDEO))
                {
                    if (fileBytes.Length > Constants.WO_MAX_SIZE_VIDEO)
                    {
                        errorMsg = string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorVideoUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                            Constants.LBM_MAX_SIZE_VIDEO));

                        return null;
                    }
                }
                else if (typeData.Equals(Constants.WO_TYPE_DETECT_FILES))
                {
                    if (fileBytes.Length > Constants.WO_MAX_SIZE_FILES)
                    {
                        errorMsg = string.Format(VNPTResources.Instance.Get(
                            VNPTResources.ID.MsgErrorFileUrlMaxSize), VNPTHelper.ConvertBytesToMegabytes(
                            Constants.LBM_MAX_SIZE_FILES));

                        return null;
                    }
                }

                using (Stream sw = File.OpenWrite(fullPath))
                {
                    sw.Write(fileBytes, 0, fileBytes.Length);
                    sw.Close();
                }

                return fileName;
            }
            else
            {
                return null;
            }
        }

        public static bool isExistMerchandise(string MerchandiseListInput, string MerchandiseListDB)
        {
            List<int> MerListInput = new List<int>();
            if (!string.IsNullOrEmpty(MerchandiseListInput))
            {
                MerListInput = (List<int>)JsonConvert.DeserializeObject(MerchandiseListInput, typeof(List<int>));
            }
            else
            {
                return true;
            }
            List<int> MerListDB = new List<int>();
            if (!string.IsNullOrEmpty(MerchandiseListDB))
            {
                MerListDB = (List<int>)JsonConvert.DeserializeObject(MerchandiseListDB, typeof(List<int>));
            }
            else
            {
                return true;
            }

            foreach (int mer in MerListInput)
            {
                if (MerListDB.Contains(mer) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public static DateTime AddWorkDays(DateTime? date, int workingDays)
        {
            int direction = workingDays < 0 ? -1 : 1;
            DateTime newDate = (date != null) ? (DateTime)date : new DateTime();
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                    newDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }



        public static void SendSMS(string phone, string content)
        {
            WS_800126.AuthHeader authHeader = new WS_800126.AuthHeader()
            {
                Username = VNPTConfigs.AccoutSMS_UserName,
                Password = VNPTConfigs.AccoutSMS_Password
            };

            WS_800126.Service1 service = new WS_800126.Service1()
            {
                AuthHeaderValue = authHeader
            };

            service.sendsms(phone, content);
        }

        public static void SendSMSCustom(string phone, string content)
        {
            if (!string.IsNullOrEmpty(VNPTConfigs.SMS_ForceSendFlg) && VNPTConfigs.SMS_ForceSendFlg.ToString() == "true")
            {
                VNPTHelper.SendSMS(phone, content);
            }
        }


        public static string RD6()
        {
            Random rd = new Random();

            string n1 = rd.Next(0, 9).ToString();
            string n2 = rd.Next(1, 7).ToString();
            string n3 = rd.Next(3, 9).ToString();
            string n4 = rd.Next(0, 7).ToString();
            string n5 = rd.Next(4, 8).ToString();
            string n6 = rd.Next(2, 6).ToString();

            return n1 + n2 + n3 + n4 + n5 + n6;
        }

        public static string createQRCode(Guid PersonalID, Guid MeetingID)
        {
            string qaCode = "";
            if (PersonalID != null && PersonalID != Guid.Empty &&
                MeetingID != null && MeetingID != Guid.Empty)
            {
                qaCode = string.Concat(PersonalID.ToString(), ";", MeetingID.ToString());
            }

            return qaCode;
        }

        public static string getTimeStr(DateTime? day)
        {
            string dateSt = day.GetValueOrDefault().ToString("HH:mm");
            if ("00:00".Equals(dateSt))
            {
                return "";
            }
            //if (day != null)
            //{
            //    dateSt = day.GetValueOrDefault().ToString("HH") + " giờ";
            //    if(day.GetValueOrDefault().ToString("mm") != "00")
            //    {
            //        dateSt = dateSt + " " + day.GetValueOrDefault().ToString("mm") + " phút";
            //    }
            //}

            return dateSt;
        }

        public static List<M_UNIT> TakeListUnitRecursive(List<M_UNIT> listUnitAll, List<M_UNIT> listInput, List<M_UNIT> unitDB, int LevelLoop = 0)
        {
            List<M_UNIT> listUnit = new List<M_UNIT>();

            foreach (M_UNIT record in listInput)
            {
                if (listUnit.Count() > 0)
                {
                    List<M_UNIT> templistUnit = unitDB
                   .Where(r => (record.ID == r.ParentID && r.DelFlg != true))
                   .Select(m => new M_UNIT
                   {
                       ID = m.ID,
                       Name = m.Name,
                       Loop = LevelLoop + 1,
                       ParentID = m.ParentID,
                       DelFlg = m.DelFlg,
                       CreateAt = m.CreateAt,
                       UpdateAt = m.UpdateAt
                   })
                   .OrderBy(r => r.Name)
                   .ToList();
                    listUnit = listUnit.Concat(templistUnit).ToList();
                }
                else
                {
                    listUnit = unitDB
                        .Where(r => (record.ID == r.ParentID && r.DelFlg != true))
                        .Select(m => new M_UNIT
                        {
                            ID = m.ID,
                            Name = m.Name,
                            Loop = LevelLoop + 1,
                            ParentID = m.ParentID,
                            DelFlg = m.DelFlg,
                            CreateAt = m.CreateAt,
                            UpdateAt = m.UpdateAt
                        })
                        .OrderBy(r => r.Name)
                        .ToList();
                }
            }

            listUnitAll = listUnitAll.Concat(listUnit).ToList();
            if (listUnit.Count() > 0)
            {
                listUnitAll = TakeListUnitRecursive(listUnitAll, listUnit, unitDB, LevelLoop + 1);
            }


            return listUnitAll;
        }

        public static List<CommonObjVM> ConvertToListLevel(string configLevel)
        {
            List<CommonObjVM> list = new List<CommonObjVM>();
            if (!string.IsNullOrEmpty(configLevel))
            {
                List<string> results = configLevel.Split(',').ToList();
                foreach (string item in results)
                {
                    string[] ite = item.Split('_');
                    CommonObjVM x = new CommonObjVM();
                    x.key = VNPTHelper.ParseInt(ite[0]);
                    x.value = VNPTHelper.ParseInt(ite[1]);

                    list.Add(x);
                }
            }

            return list;
        }


        public static string Sha256(string data)
        {
            using (var sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                // Convert byte array to a string   
                var builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private static bool CheckRoleAdmin(IRepository repository)
        {
            var userName = VNPTHelper.GetUserName();
            var roleID = repository.GetQuery<AD_USER>()
                .FirstOrDefault(r => r.UserName.Equals(userName))
                ?.RoleID;

            if (string.IsNullOrEmpty(roleID))
            {
                return false;
            }

            var roleIDs = (List<string>)JsonConvert.DeserializeObject(roleID, typeof(List<string>));

            var roles = repository.GetQuery<AD_ROLE>()
                .Where(r => r.DelFlg != true)
                .ToList();
            var roleTemp = roles
                .Where(r => r.DelFlg != true && roleIDs.Any(a => a == r.ID))
                .ToList();

            if (roleTemp == null || roleTemp.Count == 0)
            {
                return false;
            }

            if (roleTemp.Any(r => r.DefaultFlg == true))
            {
                return true;
            }

            return false;
        }
        private static List<AD_ROLE> TakeListRoleRecursive(List<AD_ROLE> listRoleAll, List<AD_ROLE> listInput, List<AD_ROLE> roleDB)
        {
            List<AD_ROLE> listRole = new List<AD_ROLE>();

            foreach (AD_ROLE record in listInput)
            {
                listRole.AddRange(roleDB
                    .Where(r => (record.ID == r.ParentID && r.DelFlg != true))
                    .OrderBy(r => r.Name)
                    .ToList());
            }

            if (listRole.Count() > 0)
            {
                listRoleAll = listRoleAll.Concat(listRole).ToList();
                listRoleAll = TakeListRoleRecursive(listRoleAll, listRole, roleDB);
            }
            return listRoleAll;
        }

        public static List<AD_ROLE> GetRoleByChilds(IRepository repository)
        {
            var roles = repository.GetQuery<AD_ROLE>()
                .Where(r => r.DelFlg != true)
                .ToList();

            var userName = VNPTHelper.GetUserName();
            var roleID = repository.GetQuery<AD_USER>()
                .FirstOrDefault(r => r.UserName.Equals(userName))
                ?.RoleID;

            if (string.IsNullOrEmpty(roleID))
            {
                return null;
            }

            var roleIDs = (List<string>)JsonConvert.DeserializeObject(roleID, typeof(List<string>));

            var roleTemp = roles
                .Where(r => r.DelFlg != true && roleIDs.Any(a => a == r.ID))
                .ToList();

            if (roleTemp == null || roleTemp.Count == 0)
            {
                return null;
            }

            if (roleTemp.Any(r => r.DefaultFlg == true))
            {
                return roles;
            }

            var listRoleChildTemp = VNPTHelper.TakeListRoleRecursive(roleTemp, roleTemp, roles.ToList());

            return listRoleChildTemp.Distinct()
                .ToList();
        }

        private static List<M_UNIT> TakeListUnitRecursive2(List<M_UNIT> listUnitAll, List<M_UNIT> listInput, List<M_UNIT> unitDB)
        {
            List<M_UNIT> listUnit = new List<M_UNIT>();

            foreach (M_UNIT record in listInput)
            {
                listUnit.AddRange(unitDB
                    .Where(r => (record.ID == r.ParentID && r.DelFlg != true))
                    .OrderBy(r => r.Name)
                    .ToList());
            }

            if (listUnit.Count() > 0)
            {
                listUnitAll = listUnitAll.Concat(listUnit).ToList();
                listUnitAll = TakeListUnitRecursive(listUnitAll, listUnit, unitDB);
            }
            return listUnitAll;
        }


        public static List<M_UNIT> GetUnitByChilds(IRepository repository, ref string unitIDLogin)
        {
            var units = repository.GetQuery<M_UNIT>()
                .Where(r => r.DelFlg != true)
                .ToList();

            var userName = VNPTHelper.GetUserName();
            var accountID = repository.GetQuery<AD_USER>()
                .FirstOrDefault(r => r.UserName.Equals(userName))
                ?.AccountID;

            if (VNPTHelper.IsNullGuid(accountID))
            {
                return null;
            }

            var isAdmin = VNPTHelper.CheckRoleAdmin(repository);
            if (isAdmin)
            {
                return units;
            }

            var unitID = repository.GetQuery<DATA_ACCOUNT>()
                .FirstOrDefault(r => r.ID == accountID)
                ?.UnitID;

            var unitTemp = units
                .Where(r => r.DelFlg != true && r.ID == unitID)
                .ToList();

            if (unitTemp == null || unitTemp.Count == 0)
            {
                return null;
            }

            unitIDLogin = unitID;

            var listUnitChildTemp = VNPTHelper.TakeListUnitRecursive2(unitTemp, unitTemp, units.ToList());

            var result = new List<M_UNIT>();
            foreach (var item in listUnitChildTemp)
            {
                if (result.Any(r => r.ID == item.ID)) continue;

                result.Add(item);
            }

            return result;
        }

        public static Boolean CheckUserPermission(IRepository repository, Guid accountID, bool adminRight = true)
        {

            string userNameGetFromOAuth = HttpContext.Current.User.Identity.Name;
            var roleIDJSON = System.Security.Claims.ClaimsPrincipal.Current.Claims.ToList()[2].Value;
            List<string> roleIDs = JsonConvert.DeserializeObject<List<string>>(roleIDJSON);
            foreach (string roleID in roleIDs)
            {
                var DefaultFlg = repository.GetQuery<AD_ROLE>().Where(r => r.DelFlg != true && r.ID == roleID)
                   .Select(r => new
                   {
                       DefaultFlg = r.DefaultFlg
                   }).FirstOrDefault().DefaultFlg
                   ;
                if (DefaultFlg == adminRight) return true;
            }
            var userNameEditFromBody = repository.GetQuery<AD_USER>().Where(r => r.DelFlg != true && r.AccountID == accountID)
                .Select(r => new
                {
                    UserName = r.UserName
                }).FirstOrDefault();

            if (userNameEditFromBody?.UserName == userNameGetFromOAuth)
            {
                return true;
            }
            return false;
        }
        public static bool CheckUserPermissionByRoleIDs(IRepository repository, string usernameFromHeader, bool adminRight = true)
        {
            //var roleIDJSON = System.Security.Claims.ClaimsPrincipal.Current.Claims.ToList()[2].Value;
            //List<string> roleIDs = JsonConvert.DeserializeObject<List<string>>(roleIDJSON);

            string absoluteUrl = HttpContext.Current.Request.Url.AbsolutePath;
            string requestMethod = HttpContext.Current.Request.HttpMethod;

            string userNameGetFromOAuth = HttpContext.Current.User.Identity.Name.ToString();
            bool isSameUserName = usernameFromHeader == userNameGetFromOAuth;
            if (!isSameUserName)
            {
                return false;
            }

            //get Role of user login
            List<string> roleIDs = new List<string>();
            var roleOfUser = repository.GetQuery<AD_USER>().Where(r => r.UserName == usernameFromHeader).FirstOrDefault(r => r.DelFlg != true);
            if (roleOfUser == null)
            {
                return false;
            }
            else
            {
                if (roleOfUser.RoleID != null)
                {
                    roleIDs = JsonConvert.DeserializeObject<List<string>>(roleOfUser.RoleID);
                }

            }

            foreach (string roleID in roleIDs)
            {
                var DefaultFlg = repository.GetQuery<AD_ROLE>().Where(r => r.DelFlg != true && r.ID == roleID)
                   .Select(r => new
                   {
                       DefaultFlg = r.DefaultFlg
                   }).FirstOrDefault().DefaultFlg
                   ;
                if (DefaultFlg == adminRight) return true;
            }

            List<string> rolePageList = new List<string>();
            List<string> rolePageListPerValues = new List<string>();
            List<roleControl> listRoleControl = new List<roleControl>();
            foreach (string roleID in roleIDs)
            {
                var rolePages = repository.GetQuery<AD_ROLE_PAGE>().Where(r => r.RoleID.Equals(roleID))
                    .Join(repository.GetQuery<AD_CONTROL>(), a => a.PageID, b => b.PageID, (a, b) => new
                    {
                        RoleID = a.RoleID,
                        PageID = a.PageID,
                        Value = a.Value,
                        ID = b.ID,
                        ControlID = b.ControlID,
                        AbsoluteUrl = b.AbsoluteUrl,
                        MethodRequest = b.MethodRequest
                    }).ToList();

                List<int> listVal = null;
                foreach (var rolePage in rolePages)
                {
                    var strVal = rolePage.Value;
                    if (!string.IsNullOrEmpty(strVal))
                    {
                        listVal = (List<int>)JsonConvert.DeserializeObject(strVal, typeof(List<int>));

                        foreach (var value in listVal)
                        {
                            if (value == rolePage.ID)
                            {
                                roleControl rcItem = new roleControl();
                                rcItem.RoleID = rolePage.RoleID;
                                rcItem.AbsoluteUrl = rolePage.AbsoluteUrl;
                                rcItem.MethodRequest = rolePage.MethodRequest;
                                rcItem.PageID = rolePage.PageID;
                                rcItem.ControlID = rolePage.ControlID;
                                listRoleControl.Add(rcItem);
                            };
                        }
                    }
                }
            }

            foreach (var roleControlItem in listRoleControl)
            {
                if ((roleControlItem.AbsoluteUrl == absoluteUrl) && (roleControlItem.MethodRequest == requestMethod))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool checkToken(IRepository repository, string bearerToken, string userNameFromHeader)
        {
            var token = bearerToken.Replace("Bearer ", "");
            var dataItem = repository.GetQuery<USER_TOKEN>()
                    .FirstOrDefault(r => r.Token == token);
            if (userNameFromHeader != "")
            {
                var userNameFromToken = repository.GetQuery<USER_TOKEN>()
                    .Where(r => r.Token == token)
                    .Select(r => r.Username)
                    .FirstOrDefault();
                if (userNameFromToken != userNameFromHeader)
                {
                    return false;
                }
            }
            if (dataItem != null)
            {
                return true;
            }
            return false;
        }
        public static bool checkIsAuthorizedUser(IRepository repository, string accountIDFromBody)
        {
            var httpCurrent = HttpContext.Current;
            if (httpCurrent != null)
            {
                var request = httpCurrent.Request;
                var token = "";
                if (request.Headers != null)
                {
                    var bearerToken = request.Headers.GetValues("Authorization").FirstOrDefault();
                    token = bearerToken.Replace("Bearer ", "");
                    var accountIDFromToken = repository.GetQuery<USER_TOKEN>()
                        .Where(r => r.Token == token)
                        .Select(r => r.AccountID)
                        .FirstOrDefault().ToString().ToLower();
                    if (accountIDFromToken == accountIDFromBody)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public static string GetUnitIDByLogin(IRepository repository)
        {
            var unitID = "";
            var userName = VNPTHelper.GetUserName();
            if (!string.IsNullOrEmpty(userName))
            {
                var accountID = repository.GetQuery<AD_USER>()
                    .FirstOrDefault(r => r.UserName.Equals(userName))
                    ?.AccountID;
                if (!VNPTHelper.IsNullGuid(accountID))
                {
                    unitID = repository.GetQuery<DATA_ACCOUNT>()
                        .FirstOrDefault(r => r.ID == accountID)
                        ?.UnitID;
                }
            }

            return unitID;
        }

        public static string convertPhone(string phone)
        {
            string phoneConvert = "";
            if (!string.IsNullOrEmpty(phone))
            {
                string first = phone.Substring(0, 1);
                if ("0".Equals(first))
                {
                    phoneConvert = "84" + phone.Substring(1);
                }
                else
                {
                    string tmp = phone.Substring(0, 3);
                    if ("+84".Equals(tmp))
                    {
                        phoneConvert = "84" + phone.Substring(3);
                    }
                    else
                    {
                        phoneConvert = "84" + phone;
                    }
                }
            }

            return phoneConvert;
        }

        public static USER_TOKEN getInfoAuthorizedUser(IRepository repository)
        {
            var httpCurrent = HttpContext.Current;
            USER_TOKEN item = new USER_TOKEN();
            if (httpCurrent != null)
            {
                var request = httpCurrent.Request;
                var token = "";
                if (request.Headers != null)
                {
                    var bearerToken = request.Headers.GetValues("Authorization").FirstOrDefault();
                    token = bearerToken.Replace("Bearer ", "");
                    var data = repository.GetQuery<USER_TOKEN>()
                        .First(r => r.Token == token);
                    if (data != null)
                    {
                        item = data;
                    }
                }
            }

            return item;
        }


        public class VNPTClone<T> where T : class, new()
        {
            public static T Clone(T param)
            {
                var result = new T();
                var properties = param.GetType().GetProperties();
                if (properties != null)
                {
                    foreach (var p in properties)
                    {
                        var value = p.GetValue(param);
                        if (p.PropertyType.Name == "String")
                        {
                            if (!"none".Equals(value))
                            {
                                result.GetType().GetProperty(p.Name).SetValue(result, value);
                            }
                        }
                    }
                }

                return result;
            }
        }

        public class roleControl
        {
            public string RoleID { get; set; }
            public string AbsoluteUrl { get; set; }
            public string MethodRequest { get; set; }
            public string PageID { get; set; }
            public string ControlID { get; set; }

        }
    }
}



