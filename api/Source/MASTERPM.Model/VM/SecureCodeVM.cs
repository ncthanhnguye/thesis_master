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
    [Bind(Exclude = "Data")]
    public class SecureCodeVM_Valid
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public string Email { get; set; }
    }

    [MetadataType(typeof(SecureCodeVM_Valid))]
    public class SecureCodeVM
    {
        public string Email { get; set; }
    }
}
