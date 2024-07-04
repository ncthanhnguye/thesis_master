using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using VNPTPM.Model.Commons;
using VNPTPM.Model.GlobalResources;
using VNPTPM.Model.Validate;

namespace VNPTPM.Model
{
    [Bind(Exclude = "ID")]
    public class M_UNIT_VALID
    {
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Name { get; set; }

        [MaxLength(150, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        //[EmailAddress(ErrorMessageResourceName = "_MsgEmailFormat", ErrorMessageResourceType = typeof(Resources))]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Phone { get; set; }

        [MaxLength(250, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string ImageUrl { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        //[MinValue(0, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMinimumValue")]
        //public short? Type { get; set; }
    }

    [MetadataType(typeof(M_UNIT_VALID))]
    public partial class M_UNIT
    {
        public int Loop { get; set; } = 0;
    }
}
