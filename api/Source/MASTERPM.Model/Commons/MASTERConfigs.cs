using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASTERPM.Model.Commons
{
    public class MASTERConfigs
    {
        public static string DirDocumentUpload { get; set; }
        public static string AccoutSMS_UserName { get; set; }
        public static string AccoutSMS_Password { get; set; }
        public static string OTP_TEST { get; set; }
        public static string SMS_ForceSendFlg { get; set; }
        public static string PathAvatar { get; set; }

        //--- FIREBASE---
        public static string Firebase_URL { get; set; }
        public static string Firebase_ServerKey { get; set; }
        public static string Firebase_SenderID { get; set; }
        //--- MODE DEBUG FIREBASE---
        public static string Firebase_ForceSendFlg { get; set; }
        
        public static int TokenExpireTime { get; set; }
        public static string PortalUrl { get; set; }

        public static int RemindMeetingTimeInterval { get; set; }
        public static string MailServerInfo { get; set; }
        public static string WebPortalUrl { get; set; }
        public static string WebClientUrl { get; set; }

        //OCR
        public static string ApiDomainOCR { get; set; }
    }
}
