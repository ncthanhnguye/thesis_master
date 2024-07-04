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
    public class DATA_CHECKLIST_VALID
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public Guid? RewardNameID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(250, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Description { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MinValue(1, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMinimumValue")]
        public short? OrderIdx { get; set; }
    }

    [MetadataType(typeof(DATA_CHECKLIST_VALID))]
    public partial class DATA_CHECKLIST
    {     
    }
}
