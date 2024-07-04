using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VNPTPM.Model.GlobalResources;
using VNPTPM.Model.Validate;

namespace VNPTPM.Model
{
    [Bind(Exclude = "ID")]
    public class M_NATIONAL_VALID
    {

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MinLength(2, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorLength")]
        [MaxLength(2, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorLength")]
        public string ID { get; set; }
    }

    [MetadataType(typeof(M_NATIONAL_VALID))]
    public partial class M_NATIONAL
    {
        public string LangID { get; set; }
    }
}