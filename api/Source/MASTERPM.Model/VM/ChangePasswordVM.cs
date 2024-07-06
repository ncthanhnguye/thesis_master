using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MASTERPM.Model.Commons;
using MASTERPM.Model.GlobalResources;
using MASTERPM.Model.Validate;

namespace MASTERPM.Model.VM
{
    [Bind(Exclude = "UserName")]
    public class ChangePasswordVM_Valid
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public string PasswordOld { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public string PasswordNew { get; set; }
    }

    [MetadataType(typeof(ChangePasswordVM_Valid))]
    public class ChangePasswordVM
    {
        public string UserName { get; set; }
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
    }
}
