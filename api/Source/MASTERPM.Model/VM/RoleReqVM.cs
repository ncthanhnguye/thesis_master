using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASTERPM.Model.VM
{
    [Serializable]
    public class RoleReqVM
    {
        public string RoleID { get; set; }
        public string PageID { get; set; }
        public string PageNm { get; set; }
        public string ControlStr { get; set; }
    }
}
