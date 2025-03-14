﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MASTERPM.Model.Commons;
using MASTERPM.Model.GlobalResources;

namespace MASTERPM.Model.VM
{
    public class LoginVM
    {
        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.Login_PhoneNotNull)]
        [RegularExpression(@"^(([0-9]*)|(([0-9]*)\.([0-9]*)))$", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "MsgErrorRegularExpressionNumber")]
        [MinLength(9, ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.Login_PhoneMinLength)]
        public string Phone { get; set; }
        public string DeviceName { get; set; }
        public string IMEICode { get; set; }
        public string TokenDevice { get; set; }
    }

    public class OTPVM
    {
        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.Login_IDNotNull)]
        public Guid ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.Login_PhoneNotNull)]
        [RegularExpression(@"^(([0-9]*)|(([0-9]*)\.([0-9]*)))$", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.MsgErrorRegularExpressionNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.Login_OTPNotNull)]
        [MaxLength(6, ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.Login_OTPOutOfSize)]
        [RegularExpression(@"^(([0-9]*)|(([0-9]*)\.([0-9]*)))$", ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.MsgErrorRegularExpressionNumber)]
        public string OTPCode { get; set; }
    }

    public class LogOutVM
    {
        [Required(ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = MASTERResources.ID.Login_IDNotNull)]
        public Guid AccLogID { get; set; }
        public Guid AccountID { get; set; }
        public string LangID { get; set; }
    }

    public class LoginWebVM
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
