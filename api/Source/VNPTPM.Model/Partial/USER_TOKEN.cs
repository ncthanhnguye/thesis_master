using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VNPTPM.Model.GlobalResources;

namespace VNPTPM.Model
{
    [Bind(Exclude = "ID")]
    public class USER_TOKEN_VALID
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(700, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Token { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Username { get; set; }
    }
    [MetadataType(typeof(USER_TOKEN_VALID))]
    public partial class USER_TOKEN
    {
    }
}
