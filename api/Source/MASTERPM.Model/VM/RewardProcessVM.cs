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
    #region RewardProcessVM
    [Bind(Exclude = "ID")]
    public class RewardProcessVM_Valid
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public Guid? ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public string Description { get; set; }
    }

    [MetadataType(typeof(RewardProcessVM_Valid))]
    public class RewardProcessVM
    {
        public Guid? ID { get; set; }
        public string Description { get; set; }
    }

    #endregion

    #region RewardProcessApprovedVM

    [Bind(Exclude = "ID")]
    public class RewardProcessApprovedVM_Valid
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public Guid? ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public string SKNote { get; set; }
    }

    [MetadataType(typeof(RewardProcessApprovedVM_Valid))]
    public class RewardProcessApprovedVM
    {
        public Guid? ID { get; set; }
        public string SKNote { get; set; }
    }

    #endregion
}
