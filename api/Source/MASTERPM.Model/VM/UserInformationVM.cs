using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASTERPM.Model.VM
{
    public class UserInformationVM
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleID { get; set; }
        public string UnitID { get; set; }
        public Guid AccountID { get; set; }
        public string RoleName { get; set; }
        public string FilePath { get; set; }
        public string Phone { get; set; }
        public string Msg { get; set; }
        public string WebPortalUrl { get; set; }
        public string WebClientUrl { get; set; }
    }
}
