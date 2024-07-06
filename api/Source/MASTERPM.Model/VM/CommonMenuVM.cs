using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASTERPM.Model.VM
{
    public class CommonMenuVM
    {
        public string searchText { get; set; }
        public byte? type { get; set; }
        public string langID { get; set; }
    }

    public class CommonObjVM
    {
        public int key { get; set; }
        public int value { get; set; }
    }
}
