using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASTERPM.Model.Commons
{
    public class Constants
    {
        public static readonly string FormatDate = "yyyyMMddHHmmss";
        public static readonly string LBM_FORMAT_SAVE_NAME_FILE = "yyyyMMddHHmmssFFF";
        public static readonly int LBM_SCORE_GET_WITH_KM = 6371;
        public static readonly int LBM_SCORE_GET_WITH_MI = 3959;
        public static readonly int LBM_NUM_RECORDS_GET_LIMIT = 10;
        public static readonly int LBM_NUM_RECORDS_GET_OFFSET = 0;
        public static readonly int LBM_NUM_RECORDS_GET_MIN = 3;
        public static readonly string LBM_SETTING_RADIUS = "0.5";//KM
        public static readonly string LBM_SETTING_MERCHANDISE = null;//select all
        public static readonly string LBM_PREFIX_CODE_BRANCH = "BRA";
        public static readonly string LBM_PREFIX_FILE_COPY = "COPY";
        public static readonly string LBM_TYPE_DETECT_VIDEO = "video";
        public static readonly string LBM_TYPE_DETECT_IMAGE = "image";
        public static readonly string LBM_TYPE_DETECT_FILES = "files";
        public static readonly string LBM_PATH_SAVE_FILES = "/Upload/Document/{0}/";
        public static readonly string LBM_PATH_SAVE_VIDEO_DEAL = "/Upload/Document/{0}/Video/Deal/";
        public static readonly string LBM_PATH_SAVE_VIDEO_BRANCH = "/Upload/Document/{0}/Video/Branch/";
        public static readonly string LBM_PATH_SAVE_IMAGE_DEAL = "/Upload/Document/{0}/Image/Deal/";
        public static readonly string LBM_PATH_SAVE_IMAGE_BRANCH = "/Upload/Document/{0}/Image/Branch/";
        public static readonly string LBM_PATH_SAVE_IMAGE_ITEM = "/Upload/Document/{0}/Image/Item/";
        public static readonly string LBM_PATH_SAVE_IMAGE_ACCOUNT = "/Upload/Document/{0}/Image/Avatar/";
        public static readonly string LBM_PATH_SAVE_IMAGE_MERCHANDISE = "Upload/Merchandise/";
        public static readonly string LBM_PATH_SAVE_IMAGE_DISCOUNT = "Upload/Discount/";
        public static readonly string LBM_PATH_SAVE_FILE_TMP = "Upload/Tmp/Account/{0}/";
        public static readonly string LBM_ICON_DEFAUL_WITH_MANY_MER = "icon_all.png";
        public static readonly string LBM_HISTORY_ACTION_DEFAULT = null;
        public static readonly string LBM_HISTORY_ACTION_SEEN_DEAL = "Seen deal";
        public static readonly string LBM_HISTORY_ACTION_SEEN_BRANCH = "Seen branch";
        public static readonly string LBM_HISTORY_ACTION_SEARCH = "Search {0}: {1}";
        public static readonly string LBM_NAME_ACCOUNT_DEAFAULT = "Customer";
        public static readonly string LBM_NAME_SYSTEM_PAYMENT = "System LBM";
        public static readonly string LBM_NAME_SYSTEM_PAYMENT_VNPAY = "System VNPAY";
        public static readonly string LBM_VALUE_DETECT_INPUT_BLANK_STRING = "none";
        public static readonly int LBM_VALUE_DETECT_INPUT_BLANK_INT = -1;
        public static readonly DateTime LBM_VALUE_DETECT_INPUT_BLANK_DATE = new DateTime(0001,01,01);
        public static readonly long LBM_MAX_SIZE_FILES = 52428800; //UNIT: 1073741824 BYTES => 1GB
        public static readonly long LBM_MAX_SIZE_VIDEO = 314572800; //UNIT: 314572800 BYTES => 300MB
        public static readonly long LBM_MAX_SIZE_IMAGE = 3145728; //UNIT: 3145728 BYTES => 3MB
        public static readonly string WO_NAME_SYSTEM = "System";
        public static readonly string TYPE_LOGIN_WITH_EMAIL = "mail";

        //PROJECT
        public static readonly string WO_FORMAT_SAVE_NAME_FILE = "yyyyMMddHHmmssFFF";
        public static readonly DateTime WO_VALUE_DETECT_INPUT_BLANK_DATE = new DateTime(0001, 01, 01);
        public static readonly string WO_PATH_SAVE_FILES = "/Upload/Document/";
        public static readonly string WO_PATH_SAVE_FILES_ARCHIVE = "/Upload/Archive/";
        public static readonly string WO_PATH_SAVE_POST = "/Upload/Post/{0}/";
        public static readonly string WO_PATH_SAVE_SOCIALNETWORK = "/Upload/SocialNetwork/{0}/";
        public static readonly string WO_PATH_SAVE_MEETINGCOMMENT = "/Upload/MeetingComment/{0}/";
        public static readonly string WO_PATH_SAVE_MEETING_DOCUMENT = "/Upload/MeetingDocument/{0}/";
        public static readonly string WO_PATH_SAVE_FILES_AS = "/Upload/Document/FileQuestion/{0}/";
        public static readonly string WO_PATH_SAVE_PAGE = "/Upload/Page/{0}/";
        public static readonly string WO_PATH_SAVE_AVATAR = "/Upload/Profile/{0}/Avatar/";
        public static readonly string WO_PATH_SAVE_PROFILES = "/Upload/Profile/{0}/Document/";
        public static readonly string WO_PATH_SAVE_APPOINTMENT = "/Upload/Document/{0}/Appointment/";
		public static readonly string WO_PATH_SAVE_AVATAR_APP = "/Upload/Profile/";
        public static readonly string WO_PATH_SAVE_DOCUMENT = "/Upload/Document/{0}/";
        public static readonly string WO_PATH_FONT = "/Upload/Font/{0}";
        public static readonly string WO_PATH_SAVE_ICON = "/Upload/Icon/{0}/";
        public static readonly string WO_PATH_SAVE_FILE_TMP = "Upload/Tmp/Account/{0}/";
        public static readonly string WO_TYPE_DETECT_VIDEO = "video";
        public static readonly string WO_TYPE_DETECT_IMAGE = "image";
        public static readonly string WO_TYPE_DETECT_FILES = "files";
        public static readonly long WO_MAX_SIZE_FILES = 52428800; //UNIT: 52428800 BYTES => 50MB
        public static readonly long WO_MAX_SIZE_VIDEO = 31457280; //UNIT: 314572800 BYTES => 30MB
        public static readonly long WO_MAX_SIZE_IMAGE = 31457280; //UNIT: 314572800 BYTES => 30MB
        public static readonly int WO_ACCTION_TYPE_RESOLVE = 0;
        public static readonly int WO_ACCTION_TYPE_RATING = 1;
        public static readonly string PATH_SAVE_POLICY = "/Upload/Policy/policy.pdf";
        public static readonly string PATH_SAVE_HELP_DOCUMENT = "/Upload/Help/help.pdf";
        public static readonly string NOT_SECRETARY = "-1";

        //
        public static readonly string CONST_PATH_SAVE_OFFICE = "/Upload/Office/{0}/";
        public static readonly string CONST_PATH_SAVE_FILE_TMP = "Upload/Tmp/Account/{0}/";
        public static readonly string PROJECT_APP_VERSION = "AppVersion";
        public static readonly string WO_PATH_SAVE_NEWS = "/Upload/News/{0}/";
        //string Action

        public static readonly string WO_PATH_SAVE_MEDIA = "/Upload/Media/";
        public static readonly List<string> ACCEPT_EXTENSION_FILES = new List<string>() { "pdf", "xlsx", "xls", ".png", ".jpg", ".jpeg" };

        public const string SMS_Of_Login = " Ma xac thuc dang nhap ung dung Dai Hoi Doan: ";

        public const string Page_PortalPostDetail = "post-detail";
        public const string Page_SocialNetworkDetail = "social-network-detail";
        public const string FLUTTER_NOTIFICATION_CLICK = "FLUTTER_NOTIFICATION_CLICK";
    }
}
