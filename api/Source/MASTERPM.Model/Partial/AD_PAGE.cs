using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MASTERPM.Model.Commons;
using MASTERPM.Model.GlobalResources;

namespace MASTERPM.Model
{
    [Bind(Exclude = "ID")]
    public class AD_PAGE_VALID
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "_MsgRequired")]
        [MaxLength(150, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Name { get; set; }
        
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string Icon { get; set; }
        
        [MaxLength(500, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "MsgErrorMaximumLength")]
        public string FileUrls { get; set; }
    }

    [MetadataType(typeof(AD_PAGE_VALID))]
    public partial class AD_PAGE
    {
        public string LangID { get; set; }
        public string PageID { get; set; }
    }
}
