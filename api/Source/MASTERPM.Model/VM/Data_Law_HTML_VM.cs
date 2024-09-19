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
    public class Data_Law_HTML_VM_VALID
    {

    }

    [MetadataType(typeof(Data_Law_HTML_VM_VALID))]
    public partial class Data_Law_HTML_VM
    {
        public int ID { get; set; }
        public int LawID { get; set; }
        public string ContentHTML { get; set; }
    }
}
