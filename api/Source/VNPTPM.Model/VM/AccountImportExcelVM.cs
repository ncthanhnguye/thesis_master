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
    public class AccountImportExcelVM_VALID
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public int? Gender { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Email { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        //public int? PositionID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        public string UnitID { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(20, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        //[NumberValue(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequiredNumber")]
        public string Phone { get; set; }
    }

    [MetadataType(typeof(AccountImportExcelVM_VALID))]
    public partial class AccountImportExcelVM
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public int? Gender { get; set; }
        public string Email { get; set; }
        public int? PositionID { get; set; }
        public string UnitID { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDate { get; set; }


        public DateTime? CommunistPartyDate { get; set; }
        public DateTime? YouthGroupDate { get; set; }
        public int? CommunistPartyPosition { get; set; }
        public int? Ethnic { get; set; }
        public string HomeTown { get; set; }
        public int? PoliticalTheory { get; set; }
        public int? Qualification { get; set; }
        public string Specialized { get; set; }
    }
}
