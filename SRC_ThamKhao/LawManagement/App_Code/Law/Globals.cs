using MOABSearch.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BDSLaw
{
    public enum LawType
    {
        Law = 0,
        Chapter = 1,
        ChapterItem = 2,
        Artical = 3,
    }
    public class Globals
    {
        public static int Articalnumber = 0;
     
        public static string KeyPhareAPIURL { get { return System.Configuration.ConfigurationManager.AppSettings["KeyPhareAPIURL"] ?? "http://127.0.0.1:5000/keyphrase"; } }
        public static void ReloadLaw(int lawID)
        {
            Dictionary<int, LawDoc> memLaw = HttpContext.Current.Session["AllLaw"] as Dictionary<int, LawDoc>;
            if (memLaw == null) memLaw = new Dictionary<int, LawDoc>();
            if (memLaw.ContainsKey(lawID))
                memLaw.Remove(lawID);
        }

       

        public static LawDoc GetLaw(int lawID, int chapterID = -1, int chapterItemID = -1, int articalID = -1)
        {
            Dictionary<int, LawDoc> memLaw = GetAllLaws();
            if (memLaw.ContainsKey(lawID))
                return memLaw[lawID];
            LawDoc law = new LawDoc();
            law.Load(lawID, chapterID, chapterItemID, articalID);
            memLaw.Add(lawID, law);
            return law;
        }

        public static string GetUerIDLogin()
        {
            BaseUser u = HttpContext.Current. Session["UserLogin"] as BaseUser;
            if (u != null)
                return u.ID;
            return "-1";
        }

        public static string GetSHA512(string password)
        {
            return MySecurity.EncryptSHA512(password);
        }

        public static DataSet GetDataset(string v)
        {
            LuatProvider pd = new LuatProvider();
            return pd.ExecuteDataSet(v);
        }

        internal static string GetConfig(string name, string defaultvalue)
        {
            string sql = string.Format( "select value from settings with(nolock) where name ='{0}'", name);
            LuatProvider pd = new LuatProvider();
            DataSet ds = pd.ExecuteDataSet(sql);
            string value = GetinDS_String(ds, 0, 0);
            if (string.IsNullOrEmpty(value))
                value = defaultvalue;
            return value;
        }

        public static Dictionary<int, LawDoc> GetAllLaws()
        {

            Dictionary<int, LawDoc> memLaw =  HttpContext.Current.Session["AllLaw"] as Dictionary<int, LawDoc>;
            if (memLaw == null)
            {
                memLaw = new Dictionary<int, LawDoc>();
                LawDoc law = new LawDoc();
                LuatProvider pd = new LuatProvider();
                DataSet ds = pd.ExecuteDataSet("select * from law with(nolock) order by Name");
                int lawID = 0;
                for (int i = 0; i < Globals.DSCount(ds); i++)
                {
                    law = new LawDoc();
                    lawID = GetIDinDS(ds, i, "ID");
                    law.Load(lawID);
                    memLaw.Add(lawID, law);
                }
                HttpContext.Current.Session["AllLaw"] = memLaw;
            }

            return memLaw;
        }

        public static void MergeLaw1IntoLaw2(LawDoc law1, LawDoc law2)
        {
            foreach (var newChapter in law1.lstChapters)
            {
                Chapter myChapter = law2.lstChapters.Where(x => newChapter.ID == x.ID).FirstOrDefault();
                if (myChapter == null)
                    law2.lstChapters.Add(newChapter);
                else
                {
                    foreach (var newChapterItem in newChapter.lstChapterItem)
                    {
                        ChapterItem myhChapterItem = myChapter.lstChapterItem.Where(x => newChapterItem.ID == x.ID).FirstOrDefault();

                        if (myhChapterItem == null)
                            myChapter.lstChapterItem.Add(newChapterItem);
                        else
                        {
                            foreach (var artical in newChapterItem.lstArtical)
                            {
                                myhChapterItem.lstArtical.Add(artical);

                            }
                        }
                    }
                }
            }
        }

        internal static string GetTail(string content, string Name)
        {
            return content == null || Name == null ? "" : content.IndexOf(Name + ".", StringComparison.InvariantCultureIgnoreCase) < 0 && content.IndexOf(Name + ":", StringComparison.InvariantCultureIgnoreCase) > 0 ? ":" : ".";

        }

        public static void WriteLog(string strContent, bool isError = false, string fileName = "Law.log")
        {
            try
            {
                if (isError) strContent = "Error: " + strContent;
                string path = System.Web.HttpContext.Current.Server.MapPath("~\\LogFiles");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string filenameLog = path + "\\" + fileName;

                try
                {
                    FileInfo fi = new FileInfo(filenameLog);
                    if (fi.Length > 10485760)    //10MB
                    {
                        File.Move(filenameLog, path + "\\" + fileName.Replace(".log", "_" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + ".log"));
                    }
                }
                catch { }

                StreamWriter writer = new StreamWriter(filenameLog, true);
                writer.WriteLine(String.Format("- {0}: {1}", DateTime.Now.ToString("MM-dd-yy HH:mm:ss.fff"), strContent));
                writer.Close();
            }
            catch { }

        }

        public static int DSCount(DataSet dsKey)
        {
            return dsKey == null || dsKey.Tables.Count == 0 ? 0 : dsKey.Tables[0].Rows.Count;
        }

        public static string GetKeyJoin(string searchInput)
        {

            return string.Join("_", searchInput.Split(' ').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray());
        }


        public static void WriteLog(Exception ex)
        {
            WriteLog(ex.Message + " - " + ex.StackTrace, true);
        }

        public static string GetKeySetting(string v)
        {
            return System.Configuration.ConfigurationManager.AppSettings[v] ?? "";
        }

        public static bool isUseKeyAPI()
        {
            return GetKeySetting("isUseKeyAPI") == "1";
        }

        public static string NextItem(string chuongContent, string v)
        {
            int idx = chuongContent.IndexOf(v, StringComparison.CurrentCultureIgnoreCase);
            return idx >= 0 ? chuongContent.Substring(idx) : "";

        }
        public static string DecodeFromUtf8(string utf8String)
        {
            // copy the string as UTF-8 bytes.
            byte[] utf8Bytes = new byte[utf8String.Length];
            for (int i = 0; i < utf8String.Length; ++i)
            {
                //Debug.Assert( 0 <= utf8String[i] && utf8String[i] <= 255, "the char must be in byte's range");
                utf8Bytes[i] = (byte)utf8String[i];
            }

            return Encoding.UTF8.GetString(utf8Bytes, 0, utf8Bytes.Length);
        }

        public static int CountTerm(string inputtext, string searchTerm)
        {
            string text = NormalizeVietnamese(inputtext);
            searchTerm = NormalizeVietnamese(searchTerm);
            if (!text.Contains(searchTerm)) return 0;
            var arr = text.Split(new string[] { searchTerm }, System.StringSplitOptions.RemoveEmptyEntries);
            int count = arr.ToList().Where(x => x != "").Count();
            return Math.Max(1, count - 1);

        }

        public static string GetAppSetting(string v1, string v2)
        {

            return System.Configuration.ConfigurationManager.AppSettings[v1] ?? v2;
        }

        private static string NormalizeVietnamese(string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }
            sb = sb.Replace('Đ', 'D');
            sb = sb.Replace('đ', 'd');
            return (sb.ToString().Normalize(NormalizationForm.FormD)).ToLower();
        }

        public static string HighlightKeyphrase(string content, List<KeyPhrase> lstKeys)
        {
            if (string.IsNullOrEmpty(content) || lstKeys == null || lstKeys.Count == 0) return "";
            int idx;
            lstKeys = lstKeys.OrderByDescending(x => x.Key.Length).ToList();
            foreach (var key in lstKeys)
            {
                string item = key.Key.Replace("_", " ");
                
                idx = content.IndexOf(item, StringComparison.CurrentCultureIgnoreCase);
                while (idx >= 0)
                {
                    content = ReplaceKeytext_Div(content, item,  idx);
                    idx = content.IndexOf(item, StringComparison.CurrentCultureIgnoreCase);
                }
               
            }
            return content;
        }

        private static string ReplaceKeytext_Div(string title, string item, int idx)
        {
            title = title.Substring(0, idx) + "<a class=\"clsKeyHighlight\">" + item.Replace(" ", "_") + "</a>"
                        + title.Substring(idx + item.Length)
                        ;
            return title;
        }

        public static string GetinDS_String(string v1, int v2)
        {
            LuatProvider pd = new LuatProvider();
            return GetinDS_String(pd.ExecuteDataSet(v1), 0, v2);
        }

        public static string GetContentBetween(string content, string begin, string end, bool isKeepBegin, bool isCleanBreakLine, string secondEnd = "", string thirdEnd = "")
        {
            if (content == "" || content.IndexOf(begin, StringComparison.CurrentCultureIgnoreCase) < 0) return "";
            string data = content.Substring(content.IndexOf(begin, StringComparison.CurrentCultureIgnoreCase));
            if (!isKeepBegin)
                data = data.Substring(begin.Length);
            if (data.IndexOf(end, StringComparison.CurrentCultureIgnoreCase) >= 0)
                data = data.Substring(0, data.IndexOf(end, StringComparison.CurrentCultureIgnoreCase));
            else
            {
                if (secondEnd != "" && data.IndexOf(secondEnd, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    data = data.Substring(0, data.IndexOf(secondEnd, StringComparison.CurrentCultureIgnoreCase));
                else if (thirdEnd != "" && data.IndexOf(thirdEnd, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    data = data.Substring(0, data.IndexOf(thirdEnd, StringComparison.CurrentCultureIgnoreCase));
              
            }
            if (isCleanBreakLine)
                data = data.TrimEnd('\r').TrimEnd('\n').Trim();
            return data;

        }

        public static int GetIDinDS(DataSet ds, int row, string v)
        {
            return ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 || ds.Tables[0].Rows[row][v].ToString() == "" ? -1 : int.Parse(ds.Tables[0].Rows[row][v].ToString());
        }

        public static string GetinDS_String(DataSet ds, int row, string v)
        {
            return ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 ? "" : ds.Tables[0].Rows[row][v].ToString();
        }

        public static int GetIDinDS(DataSet ds, int row, int v)
        {
            return ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 ? -1 : int.Parse(ds.Tables[0].Rows[row][v].ToString());
        }
        public static string GetinDS_String(DataSet ds, int row, int v)
        {
            return ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0 ? "" : ds.Tables[0].Rows[row][v].ToString();
        }
        public static string GetLama(int _Number)
        {
            try
            {
                string strRet = string.Empty;
                Boolean _Flag = true;
                string[] ArrLama = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
                int[] ArrNumber = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
                int i = 0;
                while (_Flag)
                {
                    while (_Number >= ArrNumber[i])
                    {
                        _Number -= ArrNumber[i];
                        strRet += ArrLama[i];
                        if (_Number < 1)
                            _Flag = false;
                    }
                    i++;
                }
                return strRet;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static void SetLastEditLaw(EditLawResult result)
        {
            HttpContext.Current.Session["LastEditlaw"] = null;

            if (result != null && result.mess == "")
                HttpContext.Current.Session["LastEditlaw"] = result;
        }
        public static EditLawResult GetLastEditLaw()
        {
            EditLawResult result = HttpContext.Current.Session["LastEditlaw"] as EditLawResult;
            if (result == null || result.icode < 0)
                return null;
            return result;
        }

      
    }
}
