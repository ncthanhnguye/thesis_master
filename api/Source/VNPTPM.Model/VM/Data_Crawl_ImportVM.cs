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
    public class Data_Crawl_ImportVM_VALID
    {
  
    }

    [MetadataType(typeof(Data_Crawl_ImportVM_VALID))]
    public partial class Data_Crawl_ImportVM
    {
        public int ID { get; set; }
        public string TenCauHoi { get; set; }
        public string LinhVuc { get; set; }
        public string NoiDungCauHoi { get; set; }
        public string CauTraLoi { get; set; }
        public string Luat { get; set; }
        public string KeyWords { get; set; }

    }
}
