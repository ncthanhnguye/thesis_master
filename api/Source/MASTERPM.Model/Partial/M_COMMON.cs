using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MASTERPM.Model.GlobalResources;
using MASTERPM.Model.Validate;

namespace MASTERPM.Model
{
    [Bind(Exclude = "ID")]
    public class M_COMMON_VALID
    {

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public byte? Type { get; set; }


    }

    [MetadataType(typeof(M_COMMON_VALID))]
    public partial class M_COMMON
    {
        public string LangID { get; set; }
        public int CommonID { get; set; }
    }
}