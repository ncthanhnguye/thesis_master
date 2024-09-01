using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.Data;
using MOABSearch.Common;


namespace MOABSearch.Common
{
    public class MOABCredits
    {
        public static string MOABBrowsing = "Credits_MOABBrowsing";
        public static string MOABExtBrowsing = "Credits_MOABExternalBrowsing";
        public static string MOABReport = "Credits_MOABReport";
        public static string MOABWebService = "Credits_MOABWebService";
        public static string MOABReportQuickSearch = "Credits_DetectionReport";

    }
    public class AuditInfo : DataAccess
    {

        private string m_QueryID;
        public string QueryID
        {
            get { return m_QueryID; }
            set { m_QueryID = value; }
        }
        private string m_UserName;
        public string UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }
        private string m_UserID;
        public string UserID
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }
        private string m_DateTime;
        public string sDateTime
        {
            get { return m_DateTime; }
            set { m_DateTime = value; }
        }
        private string m_TypeQuery;
        public string TypeQuery
        {
            get { return m_TypeQuery; }
            set { m_TypeQuery = value; }
        }
        private string m_Requestor;
        public string Requestor
        {
            get { return m_Requestor; }
            set { m_Requestor = value; }
        }

        private string m_ReasonForQuery;
        public string ReasonForQuery
        {
            get { return m_ReasonForQuery; }
            set { m_ReasonForQuery = value; }
        }
        private string m_QueryPars;
        public string QueryPars
        {
            get { return m_QueryPars; }
            set { m_QueryPars = value; }
        }
        private string m_IPAddress;
        public string IPAddress
        {
            get { return m_IPAddress; }
            set { m_IPAddress = value; }
        }

        private string m_CaseNumber;
        public string CaseNumber
        {
            get { return m_CaseNumber; }
            set { m_CaseNumber = value; }
        }
        public string AgencyID { get; set; }
        public string PlateQuery { get; set; }
        public int CreditWeight { get; set; }
        public string VINQuery { get; set; }

    }
    public class TrustedCertificatePolicy : ICertificatePolicy
    {
        #region ICertificatePolicy Members

        public bool CheckValidationResult(ServicePoint srvPoint, System.Security.Cryptography.X509Certificates.X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            // TODO:  Add TrustedCertificatePolicy.CheckValidationResult implementation
            return true;
        }

        #endregion
    }

    public enum LogInType
    {
        LEARN = 1, MOABSearch = 4
    }
    public enum UserType
    {
        Normal = 1,
        Vehicle = 2,
        Admin = 3,
        Agency = 4,
        AgencyManager = 5
    }

    [Flags]
    public enum UserRight : long
    {
        Agency_EditDetectionData = 8,
        Agency_ManageSystemApplications = 4194304,
        Agency_Dashboards = 524288,
        Agency_Auditing = 2199023255552,
        Agency_Reporting = 1048576,
        PasswordChange = 33554432,
        Agency_AddToDataSharing = 1,
        Agency_ForceSharing = 8796093022208,
        Agency_Internal = 4294967296,
        Agency_ForceAudit = 4,
        Agency_DistributeMessage = 512,
        Agency_ShareHotlistAllAgencies = 17592186044416,
        Agency_ShareLPRAllAgencies = 35184372088832,  //tam thoi khong dung(ko co share all agencies cho event)  
        Agency_MobileHitHunter = 70368744177664,
        Agency_HotlistManagement = 140737488355328,

        ShareDetectionWithLPRD = 16777216,

        User_Detection_View_User = 1,
        User_Detection_View_Agency = 16777216,
        User_Detection_View_PrivateData = 2147483648,
        User_Detection_View_Shared = 64,

        User_Detection_Edit_User = 4,
        User_Detection_Edit_Agency = 536870912,
        User_Hot_Edit_Agency = 1024, // moi them

        User_Hit_View_User = 2048,
        User_Hit_View_Agency = 4096,
        User_Hit_View_Shared = 16384,

        User_Hit_GetAlert_User = 131072,
        User_Hit_GetAlert_Agency = 8192,
        User_Hit_GetAlert_PrivateData = 2097152,
        User_Hit_GetAlert_Shared = 8388608,
        User_ForceAudit = 8,

        User_Hot_View_Agency = 128,
        User_Hot_View_Shared = 262144,

        User_Hot_Upload_Private = 32,
        User_Hot_Upload_Agency = 256,

        User_Hot_Edit_Private = 1073741824,

        ViewDifferentState = 268435456,
        //PrivateMASOnly                      = 4398046511104,
        //MASOnly                             = 8589934592,         
        CustomHotlist = 17179869184,
        AdministrativeAccount = 34359738368,
        MapAssigned = 68719476736,
        ShareDetectionWithNVLS = 137438953472,
        RealTime_Historical = 274877906944,
        Plate12Match = 549755813888,
        HotlistStateDetection = 1099511627776,
        PrivateDataHitNofitication = 67108864,
        NotUseEmailNotificationService = 32768,
        ReceiveNotificationsFromAllUsers = 65536,
        VVCarDetector = 2,

        AssignZone = 16,
        LimitHistory = 134217728,

        User_EmailService = 8796093022208,
        User_TargetAlertService = 4398046511104,
        User_MobileCompanion = 67108864,
        User_MobileHitHunter = 35184372088832,

        ViewDetectionTab = 17592186044416,
        User_AllLPRData = 4294967296,
        User_AllHitData = 4194304,
        User_AllHotData = 137438953472,

        User_NotAllowAlertManagement = 70368744177664,

        Audit_RequireInput = 281474976710656,
        NVLSDataPool = 562949953421312,
        AcceptAllAgencySharingHL = 1125899906842624,
        AcceptAllAgencySharingLPR = 2251799813685248,
        Agency_AdminStaff = 9007199254740992,
        Agency_NotShowInfo = 18014398509481984,
        AssignAddressFromAgency = 36028797018963968,
        Agency_NotReceiveNotifications = 4503599627370496,
        APIAccess = 72057594037927936, //7205759403792792,
        Agency_SaleDemoAccount = 128,
        SharedNVLSData = 2,
        ApplyStatisticalData = 36028797018963968

    }

    public enum NormalUserType
    {
        CDM = 0,
        CDF = 1,
        Unknown = 2
    }

    public class LogFileName
    {
        public static string MOAB = "MOAB.log";
        public static string MOABError = "MOABError.log";
    }

    public class DefinedType
    {
    }
    public class MOABSearchInfo
    {
        public string Plate;
        public List<string> lstState;
        public int PageIndex = 0;
        public int PageCount;
        public int RecordPerPage;
        public bool isNewSearch = true;
        public bool isGetPrivateData;
        public bool isVINSearch;
        public string vin;

        public string regState;

        public MOABSearchInfo() { lstState = new List<string>(); }
    }
    [Serializable]
    public class UserEventPermission
    {
        public Boolean IsLocal { get; set; }
        public Boolean IsDRN { get; set; }
        public Boolean IsMC { get; set; } // companion
        public String LocalUsers { get; set; }
        public String MCUsers { get; set; }
        public String FieldsZone_LC { get; set; }
        public String FilterZone_LC { get; set; }
        public String Rect_LC { get; set; }
        public String NVLSShareUserIDs { get; set; }
        public String FieldsZone_DRN { get; set; }
        public String FilterZone_DRN { get; set; }
        public String Rect_DRN { get; set; }

        public UserEventPermission()
        {
            Type myType = this.GetType();
            System.Reflection.PropertyInfo[] props = myType.GetProperties();
            foreach (System.Reflection.PropertyInfo prop in props)
            {
                string name = prop.PropertyType.ToString();
                switch (name)
                {
                    case "System.String":
                        prop.SetValue(this, String.Empty, null);
                        break;
                    case "System.Boolean":
                        prop.SetValue(this, false, null);
                        break;
                    default:
                        prop.SetValue(this, null, null);
                        break;
                }
            }
        }
    }
    public enum RelationType_User
    {
        ViewHotlist = 1,
        ViewEvent = 2,
        ViewHit = 3,
        GetAlert = 4,
        EditEvent = 5,
        ViewEventLink = 6

    }

    public enum RelationType
    {
        ViewHotlist = 1,
        ViewEvent = 2,
        ViewForce = 4,
        ViewBoth = 8,
        ViewLinkServer = 16,
    }
    public class CSZoneData
    {
        private string _caseWhen_Local;
        private string _caseWhen_PD;
        private string _innerWhere_Local;
        private string _innerWhere_PD;
        private string _bound_Local;
        private string _bound_PD;

        public CSZoneData()
        {
            string name = string.Empty;
            Type myType = this.GetType();
            System.Reflection.PropertyInfo[] props = myType.GetProperties();
            foreach (System.Reflection.PropertyInfo prop in props)
            {
                name = prop.PropertyType.ToString();
                switch (name)
                {
                    case "System.String": prop.SetValue(this, String.Empty, null); break;
                    default: prop.SetValue(this, null, null); break;
                }
            }
        }

        public string caseWhen_Local
        {
            get { return _caseWhen_Local; }
            set { _caseWhen_Local = value; }
        }

        public string caseWhen_PD
        {
            get { return _caseWhen_PD; }
            set { _caseWhen_PD = value; }
        }

        public string innerWhere_Local
        {
            get { return _innerWhere_Local; }
            set { _innerWhere_Local = value; }
        }

        public string innerWhere_PD
        {
            get { return _innerWhere_PD; }
            set { _innerWhere_PD = value; }
        }

        public string bound_Local
        {
            get { return _bound_Local; }
            set { _bound_Local = value; }
        }

        public string bound_PD
        {
            get { return _bound_PD; }
            set { _bound_PD = value; }
        }
    }
    //public class Bounds
    //{
    //    public Double minX;
    //    public Double maxX;
    //    public Double minY;
    //    public Double maxY;

    //    public Bounds()
    //    {
    //        minX = maxX = minY = maxY = 0.0;
    //    }
    //}
    public class Bounds
    {
        public Double minX;
        public Double maxX;
        public Double minY;
        public Double maxY;

        public Bounds()
        {
            minX = maxX = minY = maxY = 0.0;
        }

   
    }
    public enum ShapeType
    {
        None = 0,
        Circle = 1,
        Rectangle = 2,
        Polygon = 3
    }
    public class ReportInfo
    {
        public ReportInfo() { PDF = new ReportPDF(); XSL = new ReportXSL(); }
        public ReportInfo(bool p)
        {
            if (p)
            {
                string path = System.Web.HttpContext.Current.Server.MapPath("Temp");
                Globals.CreateDirectory(path);
                TempFolder = path;
                path = System.Web.HttpContext.Current.Server.MapPath("Images");
                Globals.CreateDirectory(path);
                ImageFolder = path;
                PDF = new ReportPDF(); XSL = new ReportXSL();
            }
        }
        public string TempFolder { get; set; }

        public string ImageFolder { get; set; }

        public ReportType ReportType { get; set; }

        public string FileName { get; set; }
        public ReportPDF PDF { get; set; }
        public ReportXSL XSL { get; set; }

        public string Plate { get; set; }
      
        public List<MOABSearchResult> lstMOABResult = new List<MOABSearchResult>();


        public EventInfo Ev { get; set; }
        public EventInfo EvDayTime { get; set; }

        public DateTime FirstSeenDate { get; set; }

        public DateTime LastSeenDate { get; set; }

        public int TotalDetecions { get; set; }

        internal bool IsDefaultAddressType(string addressType)
        {
            string[] arr = new string[] { "Best Public Record Address", "DMV Address", "Most Recent Sighting", "Most Popular Sighting Location", "Second Most Popular Sighting Location", "Most Recent/ Most Popular Sighting", "Most Recent/ Second Most Popular Sighting", "Detections without GPS Coordinates" };
            return arr.Contains(addressType.TrimStart().TrimEnd());
        }

        public string DMVAddress { get { return "DMV Address"; } set { DMVAddress = value; } }

        public string BestPublicRecordAddress { get { return "Best Public Record Address"; } set { BestPublicRecordAddress = value; } }


        public List<EventInfo> listEventTotal { get; set; }
    }
    public enum ReportType
    {
        PDF, XSL
    }
    public class ReportPDF
    {
        public ReportPDF()
        {
            Radius = 50;
            pathWidth = 4;
            opacity = 255;
        }
        public PdfPCell cellHeader { get; set; }
        public PdfPCell cellFooter { get; set; }

        public iTextSharp.text.Image bgSummaryPage { get; set; }
        public iTextSharp.text.Image bgHelpPage { get; set; }
        public PdfPCell cellCarPlate { get; set; }

        public PdfPCell FirstSeenCell { get; set; }
        public PdfPCell LastSeenCell { get; set; }

        public PdfPCell LabelSummaryCell { get; set; }

        public int TotalRowsTableSummary { get; set; }

        public List<PdfPCell> lstSummaryHeadersCell { get; set; }
        public string[] ArrHeaders = new string[] { "Tag #", "Address Type", "Subject Address Input", "Location Type",
                "Times Subject Vehicle Sighted", "Total Site Visits with LPR","% Seen per Visit", "Vehicle Popularity at Location", "Vehicle First Seen Date", "Vehicle Last Seen Date", "Locator Score", "Vehicle Seen More During"};


        public int MaxRowInSummaryPage = 9;

        public int MaxRowInSummaryFirstPage = 9;

        public PdfPCell LabelMapCell { get; set; }

        public int MaxRowInMapFirstPage = 1;

        public int MaxRowInMapPage = 2;

        public int TotalRowsTableMap { get; set; }

        public iTextSharp.text.Image bgMapPage { get; set; }



        public int opacity { get; set; }

        public float pathWidth { get; set; }

        public double Radius { get; set; }

        public System.Drawing.Image LocationBallonRed { get; set; }

        public Image LocationBallonGreen { get; set; }

        public Image LocationBallonYellow { get; set; }

        public Image LocationBallonWhite { get; set; }

        public PdfPCell LabelMapViewOfCell { get; set; }
    }
    public class ReportXSL
    {
    }
    public class EventInfo
    {
        public string eventID;
        public string plate;
        public double X;
        public double Y;
        public DateTime localdatetime;
        public DateTime GMTdatetime;
        public int GMTdatetimeID;
        public Int64 PAid; // protection alert id
        public Byte drnsource; //DRNSource: 0 -> Local, 1 -> DRN, 2 -> Mobile companion
        public bool isChecked = false;
        public int iLocation;
        public EventInfo()
        {
            eventID = "";
            plate = "";
            X = 0;
            Y = 0;
            localdatetime = new DateTime(2000, 1, 1, 0, 0, 0);
            GMTdatetime = new DateTime(2000, 1, 1, 0, 0, 0);
            GMTdatetimeID = 0;
            drnsource = 0;
        }

    }
    public class MOABSearchResult
    {
        public string EventID { get; set; }
        public DateTime GMTDateTime { get; set; }
        public int GMTDateTimeID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool drnsource { get; set; }
    }


    public class CurrentAppConfig
    {
        public int MOABCredits_MOABBrowsing = -1;
        public bool isLoaded = false;
        public CurrentAppConfig()
        {
            int itry = 0;
        tryagain:
            try
            {
                MOABCredits_MOABBrowsing = int.Parse(Globals.GetConfig_Default(MOABCredits.MOABBrowsing, "4"));
                GoogleApiKey = Globals.GetConfig("GoogleClientID_Business");
                MaxLengthPlateSearch = int.Parse(MOABSearch.Common.Globals.GetConfig("MaxPlateLength", "15"));
                MaxState = int.Parse(MOABSearch.Common.Globals.GetConfig("MaxSateFilter", "5"));
                AnalysisSO_Zoom = Globals.GetConfig("AnalysisSO_Zoom", "16");
                GooglePrivateKey_Business = Globals.GetConfig("GooglePrivateKey_Business");
                GoogleClientID_Business = Globals.GetConfig("GoogleClientID_Business");
                creditWeight_MOABReport = int.Parse(Globals.GetConfig(MOABCredits.MOABReport, "4"));
               
           
                MOABSearch.BLL.UserProvider up = new BLL.UserProvider();
                LinkStakeout = up.GetlinkForRedirect("LinkStakeout");
                LinkLearnNewLanding = up.GetlinkForRedirect("LinkLearnNewLanding");
                Globals.ContactPage = LinkLearnNewLanding != "" ? LinkLearnNewLanding.TrimEnd('/') + "/Contact.aspx" : System.Configuration.ConfigurationManager.AppSettings["ContactPage"];
                if (LinkLearnNewLanding == "" && !string.IsNullOrEmpty(Globals.ContactPage) && Globals.ContactPage.Contains("/"))
                    LinkLearnNewLanding = Globals.ContactPage.Substring(0, Globals.ContactPage.LastIndexOf("/"));
                PlateImageURL = Globals.GetConfig("DSL_PlateImageURL", "https://images.drndata.com/webrepo/gui/GetImage.aspx");
                CarImageURL = Globals.GetConfig("DSL_CarImageURL", "https://images.drndata.com/webrepo/gui/GetCarImage.aspx");
                LimitStakeOutTabs = int.Parse(Globals.GetConfig_Default("LimitStakeOutTabs", "20"));
                lstState = new List<StateInfo>();
                DataSet ds = up.ExecuteDataset("select StateID, StateName from [state] with(nolock)");
                if (ds != null && ds.Tables.Count > 0)
                    foreach (DataRow r in ds.Tables[0].Rows)
                        lstState.Add(new StateInfo(r[0].ToString(), r[1].ToString()));
                dicVersion = new Dictionary<string, string>();
                ds = up.ExecuteDataset("select Name, Version from Js_CssVersion with(nolock)");
                if (ds != null && ds.Tables.Count > 0)
                    foreach (DataRow r in ds.Tables[0].Rows)
                        if (!dicVersion.ContainsKey(r[0].ToString().ToUpper()))
                            dicVersion.Add(r[0].ToString().ToUpper(), r[1].ToString());
                isLoaded = true;
            }
            catch (Exception ex)
            {
                if (itry < 3)
                {
                    itry++;
                    System.Threading.Thread.Sleep(1000);
                    goto tryagain;
                }
                Globals.WriteLog(LogFileName.MOABError, "Load CurrentAppConfig: " + ex.Message);
            }
        }
        public int LimitStakeOutTabs { get; set; }
        public string GoogleApiKey = "";

        public int MaxLengthPlateSearch = -1;
        public int MaxState = -1;
        public string AnalysisSO_Zoom { get; set; }

        public string GooglePrivateKey_Business { get; set; }

        public string GoogleClientID_Business { get; set; }

        public int creditWeight_MOABReport = -1;

        public string UrlFromDRNSubscrip { get; set; }

        public string LinkStakeout { get; set; }

        public string LinkLearnNewLanding { get; set; }

        public string PlateImageURL { get; set; }

        public string CarImageURL { get; set; }
        public List<StateInfo> lstState { get; set; }
        public Dictionary<String, String> dicVersion { get; set; }
    }
    public class StateInfo
    {
        public StateInfo(string StateID, string StateName)
        {
            // TODO: Complete member initialization
            this.StateID = StateID;
            this.StateName = StateName;
        }
        public string StateID { get; set; }
        public String StateName { get; set; }
    }
    public struct StatePolygon
    {
        public string Name;
        public List<System.Drawing.PointF> pts;
        public float MinX;
        public float MinY;
        public float MaxX;
        public float MaxY;
    }
   
}

public class PSSetting : DataAccess
{
    public int iLimitPlatePerQuery { get; set; }
    public int iblock { get; set; }
    public int maxVIN { get; set; }
    public int lenVIN { get; set; }
   
    public PSSetting()
    {
        try
        {
                  
        }
        catch { }

    }
}
public class VINInfoResponse
{
    public VINInfoResponse(string vin)
    {
        VIN = vin;
    }
    public string dataInfo;
    public string code;
    public string VIN;
}