using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Collections;


using System.Collections.Specialized;
using MOABSearch.BLL;

namespace MOABSearch.Common
{

    public class CSUser
    {
        public CSUser() { }
    }

    [Serializable]
    public abstract class BizObject
    {
        protected const int MAXROWS = int.MaxValue;

        protected static Cache Cache
        {
            get { return HttpContext.Current.Cache; }
        }

        protected static string CurrentUserIP
        {
            get { return HttpContext.Current.Request.UserHostAddress; }
        }

        protected static string ServerName
        {
            get
            {
                string str = "";
                if (HttpContext.Current.Request.IsSecureConnection)
                    str = "https://";
                str += HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
                return str;
            }
        }
        protected string WebSiteName
        {
            get
            {
                int ii = HttpContext.Current.Request.ServerVariables["URL"].IndexOf('/', 1);
                return HttpContext.Current.Request.ServerVariables["URL"].Substring(1, ii - 1);
            }
        }
        protected static int GetPageIndex(int startRowIndex, int maximumRows)
        {
            if (maximumRows <= 0)
                return 0;
            else
                return (int)Math.Floor((double)startRowIndex / (double)maximumRows);
        }
        protected static string ServerPort
        {
            get
            {
                string Port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
                if (Port == "80")
                    Port = "";
                else
                    Port = ":" + Port;
                return Port;
            }
        }
        protected static string EncodeText(string content)
        {
            content = HttpUtility.HtmlEncode(content);
            content = content.Replace("  ", "&nbsp;&nbsp;").Replace("\n", "<br>");
            return content;
        }

        protected static string ConvertNullToEmptyString(string input)
        {
            return (input == null ? "" : input);
        }

        protected static void PurgeCacheItems(string prefix)
        {
            prefix = prefix.ToLower();
            List<string> itemsToRemove = new List<string>();

            IDictionaryEnumerator enumerator = BizObject.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith(prefix))
                    itemsToRemove.Add(enumerator.Key.ToString());
            }

            foreach (string itemToRemove in itemsToRemove)
                BizObject.Cache.Remove(itemToRemove);
        }

        protected string Decrypt(string sInput)
        {
            return MySecurity.Decrypt(sInput, Globals.GetConfig("EncryptKey"));
        }
        protected string Encrypt(string sInput)
        {
            return MySecurity.Encrypt(sInput, Globals.GetConfig("EncryptKey"));
        }
    }

    public class UserDetails
    {
        public UserDetails()
        {
            m_IDUser = Guid.NewGuid().ToString();
            this.m_Zone = this.m_ZoneEstimated = String.Empty;
        }

        private string m_IDUser;
        public string IDUser
        {
            get { return m_IDUser; }
            set { m_IDUser = value; }
        }

        private string m_ID;
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        private string m_UserName;
        public string UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }

        private string m_Name_UserName;
        public string Name_UserName
        {
            get { return m_Name_UserName; }
            set { m_Name_UserName = value; }
        }
        private string m_Server;
        public string Server
        {
            get { return m_Server; }
            set { m_Server = value; }
        }
        private string m_Description;
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        private string m_Password;
        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }
        private string m_PasswordSHA512;
        public string PasswordSHA512
        {
            get { return m_PasswordSHA512; }
            set { m_PasswordSHA512 = value; }
        }
        private int m_Volumn;
        public int Volumn
        {
            get { return m_Volumn; }
            set { m_Volumn = value; }
        }
        private long m_Privilege;
        public long Privilege
        {
            get
            {
                return m_Privilege;
            }
            set { m_Privilege = value; }
        }
        private string m_Region;
        public string Region
        {
            get { return m_Region; }
            set { m_Region = value; }
        }
        private int m_Type;
        public int Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }
        private string m_ExportType;
        public string ExportType
        {
            get { return m_ExportType; }
            set { m_ExportType = value; }
        }
        private string m_Email;
        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }
        private string m_Phone;
        public string Phone
        {
            get { return m_Phone; }
            set { m_Phone = value; }
        }
        private string m_Address;
        public string Address
        {
            get { return m_Address; }
            set { m_Address = value; }
        }
        private bool m_Closed;
        public bool Closed
        {
            get { return m_Closed; }
            set { m_Closed = value; }
        }
        private string m_Provider;
        public string Provider
        {
            get { return m_Provider; }
            set { m_Provider = value; }
        }
        private string m_Code;
        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }
        private int m_UDBSAID;
        public int UDBSAID
        {
            get { return m_UDBSAID; }
            set { m_UDBSAID = value; }
        }
        private int m_Status;
        public int Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }
        private DateTime m_LastLoginTime;
        public DateTime LastLoginTime
        {
            get { return m_LastLoginTime; }
            set { m_LastLoginTime = value; }
        }
        private int m_TimesLogin;//So lan login
        public int TimesLogin
        {
            get { return m_TimesLogin; }
            set { m_TimesLogin = value; }
        }
        private int m_NormalType;
        public int NormalType
        {
            get { return m_NormalType; }
            set { m_NormalType = value; }
        }
        private DateTime? m_Timeout = null;
        public DateTime? TimeOut
        {
            get { return m_Timeout; }
            set { m_Timeout = value; }
        }

        private String m_Zone;
        public String Zone
        {
            get { return m_Zone; }
            set { m_Zone = value; }
        }
        private String m_ZoneEstimated;
        public String ZoneEstimated
        {
            get { return m_ZoneEstimated; }
            set { m_ZoneEstimated = value; }
        }
        private float? m_Accuracy;
        public float? Accuracy
        {
            get { return m_Accuracy; }
            set { m_Accuracy = value; }
        }
        public string strAuditInfo;
        
    }

    [Serializable]
    public class NormalUser : BaseUser
    {
        public NormalUser()
        {
        }
        public NormalUser(UserDetails details)
            : base(details)
        {
            this.NormalType = (NormalUserType)details.NormalType;
            this.Status = details.Status;
            this.AgencyName = details.Provider;
            this.Zone = details.Zone;
            this.ZoneEstimated = details.ZoneEstimated;
            m_Accuracy = details.Accuracy;
        }

        private string m_AgencyName;
        public string AgencyName
        {
            get { return m_AgencyName; }
            set { m_AgencyName = value; }
        }
        private int m_Status;
        public int Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }
        private NormalUserType m_NormalType = NormalUserType.Unknown;
        public NormalUserType NormalType
        {
            get { return m_NormalType; }
            set { m_NormalType = value; }
        }
        /// <summary>
        /// Duong dan toi file timeout + ten file
        /// </summary>
        protected string UpdateTimeOutFile
        {
            get
            {
                string sHotListFolder = Globals.GetConfig("HotListFolder");
                if (sHotListFolder != "" && !sHotListFolder.EndsWith("\\"))
                    sHotListFolder = sHotListFolder + "\\";

                return sHotListFolder + this.UserName + "\\00__UpdateTimeOut.txt";
            }
        }
        public string HotlistFolder
        {
            get
            {
                string sHotListFolder = Globals.GetConfig("HotListFolder");
                if (sHotListFolder != "" && !sHotListFolder.EndsWith("\\"))
                    sHotListFolder = sHotListFolder + "\\";
                return sHotListFolder + this.UserName;
            }
        }
        public string EventFolder
        {
            get
            {
                string sEventFolder = Globals.GetConfig("EventFolder");
                if (sEventFolder != "" && !sEventFolder.EndsWith("\\"))
                    sEventFolder = sEventFolder + "\\";
                return sEventFolder + this.UserName;
            }
        }
        public String Zone { get; set; }

        public String ZoneEstimated { get; set; }

        private float? m_Accuracy = null;
        public float? Accuracy
        {
            get { return m_Accuracy; }
            set { m_Accuracy = value; }
        }

        public override UserType GetUserType()
        {
            return UserType.Normal;
        }

        protected override UserDetails GetUserDetails()
        {
            UserDetails details = base.GetUserDetails();
            details.Status = this.Status;
            details.NormalType = (int)this.NormalType;
            details.Accuracy = m_Accuracy;
            return details;
        }

  

        private bool m_Detection_View_PrivateData;

        private bool m_Detection_View_Shared;

        private bool m_Detection_View_User;


    }


    [Serializable]
    public abstract class BaseUser : BizObject
    {
        public BaseUser(UserDetails details)
        {
            this.Address = details.Address;
            this.Closed = details.Closed;
            this.Code = details.Code;
            this.Description = details.Description;
            this.Email = details.Email;
            this.Name = details.Name;
            this.Password = Decrypt(details.Password);
            this.Phone = details.Phone;
            
            this.Password = details.Password;

            //this.Provider = details.Provider;
            this.UserName = details.UserName;
            this.ID = details.ID;
            this.Name_UserName = details.Name_UserName;
            if (!string.IsNullOrEmpty(details.strAuditInfo))
            {
                CSAuditWindow cs = new CSAuditWindow();
                this.auditInfo = cs.ParseFromJsString(details.strAuditInfo);
            }
        }
        public BaseUser()
        { }
    
        private CSAuditWindow _auditInfo;

        public CSAuditWindow auditInfo
        {
            get { if (_auditInfo == null) _auditInfo = new CSAuditWindow(); return _auditInfo; }
            set { _auditInfo = value; }
        }
        private string m_ID;
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        private string m_UserName;
        public string UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }
        private string m_Name_UserName;
        public string Name_UserName
        {
            get { return m_Name_UserName; }
            set { m_Name_UserName = value; }
        }
        private string m_Description;
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }
        private string m_Password;
        public string Password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }


        private string m_Email;
        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }
        private string m_Phone;
        public string Phone
        {
            get { return m_Phone; }
            set { m_Phone = value; }
        }
        private string m_Address;
        public string Address
        {
            get { return m_Address; }
            set { m_Address = value; }
        }
        private bool m_Closed;
        public bool Closed
        {
            get { return m_Closed; }
            set { m_Closed = value; }
        }
        private string m_Provider;
        public string Provider
        {
            get { return m_Provider; }
            set { m_Provider = value; }
        }

        private string m_Code;
        public string Code
        {
            get { return m_Code; }
            set { m_Code = value; }
        }

        private string m_ProviderID;
        public virtual string ProviderID
        {
            get { return m_ProviderID; }
            set { m_ProviderID = value; }
        }


        

        public abstract UserType GetUserType();

        protected virtual void GetUserFromUserDetails(UserDetails details)
        {
            this.Address = details.Address;
            this.Closed = details.Closed;
            this.Code = details.Code;
            this.Description = details.Description;
            this.Email = details.Email;
            this.Name = details.Name;
            this.UserName = details.UserName;
            this.Password = Decrypt(details.Password);
            this.Phone = details.Phone;

            this.Provider = details.Provider;
            this.Name_UserName = details.Name_UserName;
        }
        protected virtual UserDetails GetUserDetails()
        {
            UserDetails details = new UserDetails();
            details.Address = this.Address;
            details.Closed = this.Closed;
            details.Code = this.Code;
            details.Description = this.Description;
            details.Email = this.Email;
            details.Name = this.Name;
            details.Password = Encrypt(this.Password);
            details.Phone = this.Phone;

            details.Provider = this.Provider;
            details.Type = (int)this.GetUserType();
            details.UserName = this.UserName;
            details.Name_UserName = this.Name_UserName;
            return details;
        }

        public static BaseUser GetUser(string userName, string password, UserType userType)
        {
            try
            {
                UserProvider up = new UserProvider();
                UserDetails details = up.GetUserDetails(userName, password, (int)userType);
                return new NormalUser(details);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                Globals.WriteLog("Login.log", ex.Message);
            }

            return null;
        }

        //Kiem tra user la closed?
        //true: closed
        public static bool IsClosedUser(string username, string type, ref string sProviderid)
        {

            string sID = "";
            UserProvider up = new UserProvider();
            bool bUser = up.IsClosedUser(username, type, ref sID);
            sProviderid = sID;
            return bUser;
        }
        public static BaseUser GetUser_ByName(string userName, UserType userType)
        {
            try
            {
                UserProvider up = new UserProvider();
                UserDetails details = up.GetUserDetailsNoPass(userName, (int)userType);
                return new NormalUser(details);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                Globals.WriteLog("Login.log", ex.Message);
            }

            return null;
        }
    }
    [Serializable]
    public class AgencyPermission
    {
        public AgencyPermission()
        {
            AgencyID = 0;
            isNewCase = false;
            isRequireInput = false;
        }
        private int m_AgencyID;
        public int AgencyID
        {
            get { return m_AgencyID; }
            set { m_AgencyID = value; }
        }
        private bool m_ClaimsReferenceID;
        public bool isNewCase
        {
            get { return m_ClaimsReferenceID; }
            set { m_ClaimsReferenceID = value; }
        }
        private bool requireInput;
        public bool isRequireInput
        {
            get { return requireInput; }
            set { requireInput = value; }
        }


        public bool isForceAudit { get; set; }
    }


    [Serializable]
    public class CSAuditWindow
    {

        public String ClaimsReferenceID = String.Empty;
        public String AuthorizedPurpose = String.Empty;
        public bool isAudited;

        public CSAuditWindow()
        {
            isAudited = false;
        }

        public String ToJsString()
        {
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            return ser.Serialize(this);
        }

        public CSAuditWindow ParseFromJsString(String jsString)
        {
            try
            {
                if (!String.IsNullOrEmpty(jsString))
                {
                    System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
                    return ser.Deserialize<CSAuditWindow>(jsString);
                }
            }
            catch { }

            return new CSAuditWindow();
        }
    }
}