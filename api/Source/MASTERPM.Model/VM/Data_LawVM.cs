using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MASTERPM.Model.VM
{
    [Bind(Exclude = "ID")]
    public class Data_LawVM_VALID
    {

    }

    [MetadataType(typeof(Data_LawVM_VALID))]
    public partial class Data_LawVM
    {
        public int ID { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

    }
}
