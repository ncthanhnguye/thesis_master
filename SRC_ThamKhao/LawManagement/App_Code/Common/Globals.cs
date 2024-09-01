using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Xml;
using System.Data.SqlClient;
using System.Web.UI;
using System.Collections;
using System.Text;
using System.Data;


using MOABSearch.BLL;
using System.Drawing;

using System.Web.Script.Serialization;


namespace MOABSearch.Common
{
    public class Globals
    {
        public static List<Geomatries.Polygon> zonePD;

        public static List<Geomatries.Polygon> zoneLC;
        public static string UserMC;
        public static string UserLC;
        public static string UserNVLS;

        public static double discluster2 = 50;
        public static int g_CommandTimeOut = GetTimeoutValue();

        private static int GetTimeoutValue()
        {
            int v = 300;
            if (System.Configuration.ConfigurationManager.AppSettings["SQLTimeout"] != null)
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["SQLTimeout"], out v);
            return v;
        }

        public static string WEB_VERSION_CD = "5.0";
        public static String gFormatDate = "MM-dd-yy";
        public static String gFormatTime = "HH:mm:ss";
        public static String gFormatDateTime = "MM-dd-yy HH:mm:ss.fff";
        public static Boolean gCardataInvalid = false;
        public static String g_UserLogin = "UserLogin";
        public static String g_EventsLocal = "Events_Local";
        public static string g_SqlFalse = "1=0";
        public static String g_JS_CSS_Ver = "JS_CSS_VersionFile";
        public static string g_GMTDatetimeID = "gGMTTimeID";
        public static String GetServerPort()
        {
            string Port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            if (Port == "80" || Port == "443")//i.e. HTTP Port = 80 and HTTPS Port = 443
                Port = "";
            else
                Port = ":" + Port;
            return Port;

        }

        public static String GetUrlSchema()
        {
            try
            {
                return String.Format("{0}://", HttpContext.Current.Request.Url.Scheme);
            }
            catch { }

            return "http://";
        }

        public static Int64 GetCellID2(double X, double Y, double fDist)
        {
            return (Int64)((int)((X + 90) / fDist)) * (int)(360 / fDist) + (int)((Y + 180) / fDist);
        }

        public static void WriteLog(string fileName, string strContent)
        {
            try
            {
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
                writer.WriteLine(String.Format("- {0}: {1}", DateTime.Now.ToString(gFormatDateTime), strContent));
                writer.Close();
            }
            catch { }

        }
        public static void WriteLog_New(string fileName, string strContent)
        {
            try
            {
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
                writer.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff") + "_: " + strContent);
                writer.Close();
            }
            catch { }





        }
        public static void WriteStopWatch(string strContent)
        {
            try
            {
                string path = System.Web.HttpContext.Current.Server.MapPath("~\\LogFiles");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fileName = "StopWatch.log";
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
                writer.WriteLine(String.Format("- {0}: {1}", DateTime.Now.ToString(gFormatDateTime), strContent));
                writer.Close();
            }
            catch { }

        }

      
    

      

        public static double ToRadians(double degrees)
        {
            double LOCAL_PI = 3.1415926535897932385;
            double radians = degrees * LOCAL_PI / 180;
            return radians;
        }

        public static double DirectDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double earthRadius = 6371.009; //3958.75;
            double dLat = ToRadians(lat2 - lat1);
            double dLng = ToRadians(lng2 - lng1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                        Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double dist = earthRadius * c;
            double meterConversion = 1000;//1609.00;
            return dist * meterConversion;
        }

        public static bool DBConnectionStatus()
        {
            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CSConnString"].ConnectionString;
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();
                    return (sqlConn.State == System.Data.ConnectionState.Open);
                }
            }
            catch (SqlException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static String GetConfig(String strConfig, String strDefault)
        {

            string strSQL;
            strSQL = "Select top 1 Value from [setting] WITH (NOLOCK) where Name='" + strConfig + "'";

            string strValue = "";
            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CSConnString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = strSQL;
                    object o = cmd.ExecuteScalar();
                    if (o != null)
                        strValue = o.ToString();
                }
            }
            catch { }

            if (String.IsNullOrEmpty(strValue))
                strValue = strDefault;

            return strValue;

        }
        public static string GetConfig(string strConfig)
        {

            string strSQL;
            strSQL = "Select top 1 Value from [setting] WITH (NOLOCK) where Name='" + strConfig + "'";

            string strValue = "";
            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CSConnString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = strSQL;
                    object o = cmd.ExecuteScalar();
                    if (o != null)
                        strValue = o.ToString();
                }
            }
            catch { }

            return strValue;

        }
        public static string GetMOABConfig(string strConfig)
        {

            string strSQL;
            strSQL = "Select top 1 Value from [setting] WITH (NOLOCK) where Name='" + strConfig + "'";

            string strValue = "";
            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MOABConnString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch
                    {
                        return "";
                    }
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = strSQL;
                    object o = cmd.ExecuteScalar();
                    if (o != null)
                        strValue = o.ToString();
                }
            }
            catch { }

            return strValue;

        }
        public static String ConvertString(object obj)
        {
            try
            {
                String str = "";
                if (obj != null)
                    str = obj.ToString().Trim();
                str = str.Replace("&nbsp;", "");
                return str;
            }
            catch
            {
                return String.Empty;
            }

        }
        public static String ConvertString_NA(object obj)
        {
            try
            {
                string str = "N/A";
                if (obj != null && obj.ToString() != "")
                    str = obj.ToString().Trim();
                return str;
            }
            catch
            {
                return "N/A";
            }

        }
        public static Boolean IsNVLS_Connect()
        {
            try
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings["CSConnString_NVLS"] != null
                    && !String.IsNullOrEmpty(System.Configuration.ConfigurationManager.ConnectionStrings["CSConnString_NVLS"].ToString()))
                    return true;
            }
            catch
            {

            }
            return false;
        }
        public static void CheckLogoutPage()
        {
            if (HttpContext.Current.Session[g_UserLogin] == null)
            {
                String url = HttpContext.Current.Request.ServerVariables["URL"];

                if (url.ToLower().IndexOf("gui") > 0)
                    url = url.Substring(0, url.LastIndexOf("/") - 3);
                else
                    url = url.Substring(0, url.LastIndexOf("/") + 1);

                url = GetUrlSchema() + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + GetServerPort() + url + "home.aspx";

                String query = HttpContext.Current.Request.Url.Query;

                if (!String.IsNullOrEmpty(query) && query.ToLower().IndexOf("?addr=") >= 0)
                    url += query;

                HttpContext.Current.Response.Redirect(url);
                return;
            }
        }

        public static int GetGMTDateTimeID(DateTime dtCurTime)
        {
            int gmtDateTimeID = 0;

            try { 
                 DateTime dtStartTime = new DateTime(2000, 1, 1, 0, 0, 0);            
                TimeSpan span = new TimeSpan();
                span = dtCurTime.Subtract(dtStartTime);
                if (span.TotalSeconds > 0 && span.TotalSeconds < 2000000000)
                    gmtDateTimeID = (int)span.TotalSeconds;
                else
                    gmtDateTimeID = 0;
            }catch{}

            return gmtDateTimeID;
        }

   
        public static string ParseWhereID(string sID)
        {
            try
            {
                if (!string.IsNullOrEmpty(sID))
                {
                    if (sID.IndexOf(",") > 0)
                    {

                        if (sID.IndexOf(",'") > 0)
                        {
                            sID = sID.Replace("','", ",");
                            sID = sID.Substring(1, sID.Length - 2);
                            sID = sID.Replace("'", "''");
                            sID = " in ('" + sID.Replace(",", "','") + "')";
                        }
                        else
                            sID = " in ('" + sID.Replace(",", "','") + "')";
                    }
                    else if (sID.IndexOf("'") >= 0)
                    {
                        sID = " = " + sID;
                    }
                    else
                        sID = "  = '" + sID + "'";
                }
                sID += " ";
            }
            catch
            {
            }
            return sID;
        }        

        public static string ParseWhereBigIntID(string sID)
        {
            try
            {
                sID = sID.Replace("'", "");
                if (sID.IndexOf(",") > 0)
                    sID = " in (" + sID + ")";
                else
                    sID = "  = " + sID;
            }
            catch
            {
            }
            return sID;
        }

        public static Boolean IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }

     

        public static String ZoneParser(Byte[] bytearray)
        {
            String strZone = String.Empty;
            try
            {
                strZone = System.Text.Encoding.ASCII.GetString(bytearray);
            }
            catch
            {
            }
            return strZone;
        }

        public static Byte NumFractionDigits()
        {           
            try
            {                
                return Byte.Parse(System.Configuration.ConfigurationManager.AppSettings["NumFractionDigitsZone"].ToString());
            }
            catch { }

            return 8;
        }


        public static void RegisterScriptFile(Page pge, string scriptFile)
        {
            try
            {
                HtmlGenericControl scriptInclude = new HtmlGenericControl("script");
                scriptInclude.Attributes.Add("type", "text/javascript");
                scriptInclude.Attributes.Add("src", System.Web.HttpUtility.HtmlDecode(scriptFile));
                if (!pge.Header.Controls.Contains(scriptInclude))
                    pge.Header.Controls.Add(scriptInclude);
            }
            catch { }
        }

        public static void RegisterScriptFile(Page pge, String[] scriptFile)
        {
            try
            {
                HtmlGenericControl scriptInclude;
                foreach (String item in scriptFile)
                {
                    scriptInclude = new HtmlGenericControl("script");
                    scriptInclude.Attributes.Add("type", "text/javascript");
                    scriptInclude.Attributes.Add("src", String.Format("Jscripts/{0}.js", item));
                    if (!pge.Header.Controls.Contains(scriptInclude))
                        pge.Header.Controls.Add(scriptInclude);
                }
            }
            catch (Exception e) { String ss = e.Message; }
        }

        public static void IncludeScriptFile(Page pge, String[] scriptFile)
        {
            try
            {
                foreach (String item in scriptFile)
                {
                    if (!pge.ClientScript.IsClientScriptIncludeRegistered(item))
                        pge.ClientScript.RegisterClientScriptInclude(item, String.Format("Jscriptss/{0}.js", item));

                }
            }
            catch (Exception e) { String ss = e.Message; }
        }

        public static void RegisterCssFile(Page pge, String CssFile)
        {
            try
            {
                HtmlLink css = new HtmlLink();
                css.Href = CssFile;
                css.Attributes["rel"] = "stylesheet";
                css.Attributes["type"] = "text/css";
                css.Attributes["media"] = "all";
                if (!pge.Header.Controls.Contains(css))
                    pge.Header.Controls.Add(css);
            }
            catch { }
        }

        public static void RegisterCssFile(Page pge, String[] CssFiles)
        {
            try
            {
                HtmlLink css;
                foreach (String item in CssFiles)
                {
                    css = new HtmlLink();
                    css.Href = String.Format("Style/{0}.css", item);
                    css.Attributes["rel"] = "stylesheet";
                    css.Attributes["type"] = "text/css";
                    css.Attributes["media"] = "all";
                    if (!pge.Header.Controls.Contains(css))
                        pge.Header.Controls.Add(css);
                }
            }
            catch { }
        }

        public static void RegisterCssFile_2(Page pge, String[] CssFiles)
        {
            try
            {
                HtmlLink css;
                foreach (String item in CssFiles)
                {
                    css = new HtmlLink();
                    css.Href = String.Format("Style/{0}.css", item);
                    css.Attributes["rel"] = "stylesheet";
                    css.Attributes["type"] = "text/css";
                    css.Attributes["media"] = "all";
                    if (!pge.Header.Controls.Contains(css))
                        pge.Header.Controls.Add(css);
                }
            }
            catch { }
        }

        public static string CreateDirectory(string d)
        {
            try
            {
                if (!Directory.Exists(d))
                    Directory.CreateDirectory(d);
                return d;
            }
            catch { return ""; }
        }



        public static void RegisterFilesOnHeader(Page pge, String[] files, Boolean jsFile, string path)
        {
            try
            {
                // dang ky JS file
                if (jsFile)
                {
                    HtmlGenericControl scriptInclude;
                    foreach (String item in files)
                    {
                        scriptInclude = new HtmlGenericControl("script");
                        scriptInclude.Attributes.Add("type", "text/javascript");
                        scriptInclude.Attributes.Add("src", String.Format(path + "Jscripts/{0}.js{1}", item, Globals.GetVersion(item)));
                        if (!pge.Header.Controls.Contains(scriptInclude))
                            pge.Header.Controls.Add(scriptInclude);
                    }
                }
                else // dang ky CSS file
                {
                    HtmlLink css;
                    foreach (String item in files)
                    {
                        css = new HtmlLink();
                        css.Href = String.Format(path + "Styles/{0}.css{1}", item, Globals.GetVersion(item));
                        css.Attributes["rel"] = "stylesheet";
                        css.Attributes["type"] = "text/css";
                        css.Attributes["media"] = "all";
                        if (!pge.Header.Controls.Contains(css))
                            pge.Header.Controls.Add(css);
                    }
                }
            }
            catch { }
        }

        public static string GetVersion(string name)
        {
            String vsersion = String.Empty; name = name.ToUpper();
            try
            {
                CurrentAppConfig ap = GetCurrentAppConfig();
                if (ap != null && ap.dicVersion.ContainsKey(name))
                    vsersion = (String)ap.dicVersion[name];
            }
            catch { }
            if (String.IsNullOrEmpty(vsersion))
                vsersion = DateTime.Now.ToString("5.1.MMddyy.HHmm");

            return String.Format("?v=" + vsersion);
        }
        public static string[] GetToolTips(string strKey)
        {
            try
            {
                string[] strValue = new string[2];
                DataSet dst = new DataSet();
                string fName = HttpContext.Current.Server.MapPath("~/XML/ToolTips.xml");

                using (FileStream fs = new FileStream(fName, FileMode.Open, FileAccess.Read))
                {
                    dst.ReadXml(fs);
                    fs.Close();
                }
                if (dst != null && dst.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dst.Tables[0].Rows)
                    {
                        strValue[0] = row["TextKey"].ToString();
                        if (strValue[0].ToLower() == strKey.ToLower())
                        {
                            strValue[0] = row["TextContent"].ToString();
                            strValue[1] = row["TextHeader"].ToString();
                            return strValue;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                string ss = ex.Message;
                return null;
            }
            return null;
        }
        public static void SetToolTipsInnerHtml(HtmlGenericControl Content, HtmlGenericControl Header, string KeyToolTip)
        {
            try
            {
                string[] strTooltip = null;
                strTooltip = Globals.GetToolTips(KeyToolTip);
                if (strTooltip != null)
                {
                    Content.InnerHtml = strTooltip[0];
                    Header.InnerHtml = strTooltip[1];
                }
            }
            catch { }
        }
       public static double GetDistanceFromLatLonInMiles(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371 * 1000 * 0.000621371192; // Radius of the earth in Miles
            double dLat = deg2rad(lat2 - lat1);  // deg2rad below
            double dLon = deg2rad(lon2 - lon1);
            double a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c; // Distance in Miles
            return d;
        }

        static double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public static String GetResponseContentType(String Extension)
        {
            String type = String.Empty;
            switch (Extension.ToLower())
            {
                case ".pdf": type = "application/pdf"; break;
                case ".zip": type = "application/x-zip-compressed"; break;
                case ".xls": type = "application/vnd.ms-excel"; break;
                case ".xlsx": type = "application/vnd.ms-excel.12"; break;
                case ".csv": type = "text/csv"; break;
                case ".doc": type = "application/msword"; break;
                case ".docx": type = "application/vnd.ms-word.document.12"; break;
                case ".txt": type = "text/plain"; break;

                case ".htm":
                case ".html":
                    type = "text/html"; break;

                default:
                    type = "application/octet-stream";
                    break;
            }

            return type;
        }
       
        public static class MOABSerializer<T> where T : class,System.Runtime.Serialization.ISerializable
        {
            public static Byte[] Serialize(T objectToSerialize)
            {
                try
                {
                    MemoryStream ms = new MemoryStream();
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bFormatter.Serialize(ms, objectToSerialize);
                    Byte[] retVal = ms.ToArray();
                    ms.Close();
                    return retVal;
                }
                catch (System.Exception ex)
                {
                    String ss = ex.Message;
                }
                return null;
            }
            public static T DeSerialize(byte[] data)
            {
                T objectToSerialize = null;

                try
                {

                    MemoryStream ms = new MemoryStream();
                    ms.Write(data, 0, data.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    objectToSerialize = (T)bFormatter.Deserialize(ms);
                    ms.Close();
                }
                catch (System.Exception ex)
                {
                    String ss = ex.Message;
                }
                return objectToSerialize;
            }
        }

        public static List<string> WrapText(int myLimit, string sentence)
        {  
            string[] words = sentence.Split(new char[] { ' ' });
            IList<string> sentenceParts = new List<string>();
            sentenceParts.Add(string.Empty);
            int partCounter = 0;
            foreach (string word in words)
            {
                if (word.Length <= myLimit)
                {
                    if ((sentenceParts[partCounter] + word).Length > myLimit)
                    {
                        partCounter++;
                        sentenceParts.Add(string.Empty);
                    }

                    sentenceParts[partCounter] += word + " ";
                }
                else {
                     partCounter++;
                     sentenceParts.Add(string.Empty);
                    int s = myLimit;
                    for (int i = 0; i < word.Length; i++) {
                        if (i == s - 1)
                        {
                            s += myLimit;
                            partCounter++;
                            sentenceParts.Add(string.Empty);
                        }
                        sentenceParts[partCounter] += word[i];                       
                    }
                }
            }
            return sentenceParts.ToList();
        }

    
        public static string getUrlLogo()
        {// Get Url Logo
            try
            {
                string strUrl = "";
                strUrl = System.Configuration.ConfigurationManager.AppSettings["UrlLogo"].ToString();
                if (strUrl != "")
                    return strUrl;
                return "";
            }
            catch
            {
                return "";
            }
        }
        internal static string GetConfig_Default(string strConfig, string sDefault)
        {
            string strSQL;
            strSQL = "Select top 1 Value from [setting] WITH (NOLOCK) where Name='" + strConfig + "'";

            string strValue = "";
            try
            {
                String ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CSConnString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = conn.CreateCommand();
                        cmd.CommandText = strSQL;
                        object o = cmd.ExecuteScalar();
                        if (o != null)
                            strValue = o.ToString();
                    }
                    catch { }
                }
            }
            catch { }

            if (String.IsNullOrEmpty(strValue))
                strValue = sDefault;

            return strValue;
        }


        public static string SessionConfigName = "CurrentSessionConfig";
        public static string sAppConfig = "CurrentAppConfig";
        public static string ContactPage = "";
              public static CurrentAppConfig GetCurrentAppConfig()
        {
            if (HttpContext.Current.Application[Globals.sAppConfig] != null)
            {
                CurrentAppConfig a = HttpContext.Current.Application[Globals.sAppConfig] as CurrentAppConfig;
                if (a.isLoaded)
                    return a;
            }
            CurrentAppConfig o = new CurrentAppConfig();
            HttpContext.Current.Application[Globals.sAppConfig] = o;
            return o;
        }
        public static void LogoutToLEARN(bool isLogout)
        {
            try
            {
                CurrentAppConfig app = GetCurrentAppConfig();
              
                string url = app.LinkLearnNewLanding;
                if (string.IsNullOrEmpty(url)) url = Globals.GetConfig("LinkLearnNewLanding_DSL");
                if (isLogout)
                    url = url + "/home.aspx?loghome=1";// foraudit
                else
                    url = url + "/gui/index.aspx?ProviderType=NormalProvider";
            
                
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
                Globals.WriteLog("MOABTrace.log", "Logout: " + url);
                HttpContext.Current.Response.Redirect(url, false);
            }
            catch (Exception ex)
            {
                Globals.WriteLog(LogFileName.MOABError, "LogoutToLEARN: " + ex.Message);
            }
        }

        public static void ValidateSession()
        {

            if (HttpContext.Current.Session["UserLogin"] == null)
                HttpContext.Current.Response.Redirect("Default.aspx");
        }

        public static void ClearConnections()
        {
            string sql = "Delete from keygenerate where isloggedin = " + (int)LogInType.MOABSearch;
            UserProvider up = new UserProvider();
            up.ExecuteNonQuery(sql);
        }

        public static string GetStateID(double x, double y)
        {
            try
            {

                List<StatePolygon> lstPolygon = GetListStatePolygon();
                for (int i = 0; i < lstPolygon.Count; i++)
                {
                    if ((x >= lstPolygon[i].MinX) && (x <= lstPolygon[i].MaxX) && (y >= lstPolygon[i].MinY) && (y <= lstPolygon[i].MaxY))
                    {
                        if (IsInsidePolyGon(lstPolygon[i].pts, (float)x, (float)y))
                            return lstPolygon[i].Name.Trim().ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                Globals.WriteLog("StakeoutError.log", "GetStateValueFromLatLng: " + ex.Message);
            }
            return "";
        }
        public static List<StatePolygon> g_lstStatePolygon { get; set; }
        internal static bool IsInsidePolyGon(List<PointF> lst, float X, float Y)
        {

            PointF[] poly = new PointF[lst.Count];

            for (int z = 0; z < lst.Count; z++)
            {
                poly[z].X = lst[z].X;
                poly[z].Y = lst[z].Y;
            }
            PointF pt = new PointF(0, 0);
            try
            {
                pt.X = X;//Utilities.ParseFloat(X);
                pt.Y = Y;// Utilities.ParseFloat(Y);
            }
            catch //(System.Exception e)
            {
            }
            int npoints = poly.Length - 1; // number of points in polygon
            // this assumes that last point is same as first
            float xnew, ynew, xold, yold, x1, y1, x2, y2;
            int i;
            bool inside = false;

            if (npoints < 3)
            { // points don't describe a polygon
                return false;
            }
            xold = poly[npoints - 1].X; yold = poly[npoints - 1].Y;

            for (i = 0; i < npoints; i++)
            {
                xnew = poly[i].X; ynew = poly[i].Y;
                if (xnew > xold)
                {
                    x1 = xold; x2 = xnew; y1 = yold; y2 = ynew;
                }
                else
                {
                    x1 = xnew; x2 = xold; y1 = ynew; y2 = yold;
                }
                if ((xnew < pt.X) == (pt.X <= xold) && ((pt.Y - y1) * (x2 - x1) < (y2 - y1) * (pt.X - x1)))
                {
                    inside = !inside;
                }
                //polyX[i]+(y-polyY[i])/(polyY[j]-polyY[i])*(polyX[j]-polyX[i])<x
                xold = xnew; yold = ynew;
            }; // for
            return inside;
        }

        internal static List<StatePolygon> GetListStatePolygon()
        {
            if (g_lstStatePolygon == null || g_lstStatePolygon.Count == 0)
            {
                g_lstStatePolygon = new List<StatePolygon>();
                StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath("~/USState_Border.csv"));
                float lat, lng;
                string[] arr = null;
                string[] separator = new string[] { "," };

                string s;
                int stateIndex = 0;
                int latIndex = 1;
                int lngIndex = 2;

                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();
                    arr = s.Split(separator, StringSplitOptions.None);
                    if (arr == null || arr.Length < 3)
                        continue;
                    break;
                }
                if (arr == null)
                    return null;
                for (int i = 0; i < arr.Length; i++)    //Get header index
                {
                    switch (arr[i].ToLower())
                    {
                        case "lat":
                        case "latitude":
                            latIndex = i;
                            break;
                        case "lon":
                        case "long":
                        case "longitude":
                            lngIndex = i;
                            break;
                        case "state":
                            stateIndex = i;
                            break;
                    }
                }
                StatePolygon sp = new StatePolygon();
                string strState;
                int iPoints = 0;
                //string strPoints = "";
                List<PointF> ptLst = new List<PointF>();
                float MinX = 0, MinY = 0, MaxX = 0, MaxY = 0;
                bool bRefresh = true;
                while (!sr.EndOfStream)
                {
                    try
                    {
                        s = sr.ReadLine();
                        arr = s.Split(separator, StringSplitOptions.None);
                        if (arr == null || arr.Length < 3)
                            continue;

                        strState = arr[stateIndex];

                        if (sp.Name == null)   //State dau tien
                            sp.Name = arr[stateIndex];
                        if ((strState != sp.Name) && (strState != "") && (sp.Name != null))  //Neu het point cua 1 state
                        {
                            sp.MinX = MinX;
                            sp.MaxX = MaxX;
                            sp.MinY = MinY;
                            sp.MaxY = MaxY;

                            sp.pts = ptLst;
                            g_lstStatePolygon.Add(sp);

                            sp = new StatePolygon();
                            iPoints = 0;
                            //strPoints = "";
                            ptLst = new List<PointF>();
                            sp.Name = arr[stateIndex]; //Tiep tuc cho State moi
                            bRefresh = true;
                        }

                        if (!float.TryParse(arr[latIndex], out lat))
                            continue;
                        if (!float.TryParse(arr[lngIndex], out lng))
                            continue;
                        if (lat == 0 || lng == 0)
                            continue;

                        if (bRefresh)
                        {
                            MinX = MaxX = lat;
                            MinY = MaxY = lng;

                            bRefresh = false;
                        }
                        else
                        {
                            if (lat < MinX)
                                MinX = lat;
                            else if (lat > MaxX)
                                MaxX = lat;
                            if (lng < MinY)
                                MinY = lng;
                            else if (lng > MaxY)
                                MaxY = lng;
                        }

                        iPoints++;

                        // strPoints += lat.ToString() + ";" + lng.ToString() + ";";
                        ptLst.Add(new PointF(lat, lng));
                    }
                    catch (Exception ex)
                    { }
                }
                try
                {
                    if (!string.IsNullOrEmpty(sp.Name) && iPoints > 0)
                    {
                        sp.pts = ptLst;
                        g_lstStatePolygon.Add(sp);
                    }
                }
                catch
                {
                }

            }

            return g_lstStatePolygon;
        }
        public static string[] GetListStateCode(bool isGetStateCode = true, string selectedStates = "")
        {
            try
            {
                List<string> lst = new List<string>();
                UserProvider up = new UserProvider();
                string str = "";
                if (selectedStates != "")
                {
                    string[] arr = selectedStates.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x.Trim()) && !x.Contains("'")).Select(s=>s.Trim()).ToArray();
                    str = String.Format(" and st.stateid in('{0}')", string.Join("','", arr));
                }
                string sql = isGetStateCode ? "select StateID from State st with(nolock) where stateid is not null and stateid <> '' " + str + " order by stateid" :
                    "select m.StateID from State st with(nolock) left join US_StateID_Mapping m  with(nolock) on st.StateName = m.StateName where st.stateid is not null and st.StateID <> ''  " + str + " order by m.stateid";
                DataSet ds = up.ExecuteDataset(sql);
                if (ds != null && ds.Tables.Count > 0)
                    foreach (DataRow r in ds.Tables[0].Rows)
                        lst.Add(r[0].ToString());
                return lst.ToArray();
            }
            catch (Exception ex)
            {
                Globals.WriteLog_New(LogFileName.MOABError, "GetListStateCode: " + ex.Message);
            }

            return new string[0];
        }
        static PSSetting ps;
        public static string g_GetImageURL = "";
        public static string g_GetCarImageURL = "";
        public static void SetImageConnection()
        {
            try
            {
                if (string.IsNullOrEmpty(g_GetImageURL))
                    Globals.g_GetImageURL = GetConfig("DSL_PlateImageURL", "https://images.drndata.com/webrepo/gui/GetImage.aspx");
                if (string.IsNullOrEmpty(g_GetCarImageURL))
                    Globals.g_GetCarImageURL = GetConfig("DSL_CarImageURL", "https://images.drndata.com/webrepo/gui/GetCarImage.aspx");
            }
            catch (Exception ex)
            {
                WriteLog("PSError.log", "SetImageConnection: " + ex.Message);
            }
        }
        public static PSSetting VINSetting { get { if (ps == null) ps = new PSSetting(); return ps; } set { ps = value; } }

        public static Byte[] GetNoImageAvailabe()
        {
            String strImage = "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";
            try
            {
                return Convert.FromBase64String(strImage);
            }
            catch { }

            return null;
        }
    }
}