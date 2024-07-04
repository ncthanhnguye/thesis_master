using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.VM
{
    public class UserRoleVM
    {
        public string RoleID { get; set; }
        public List<UserRole> Items { get; set; }
    }
    public class UserRole
    {
        public string ParentID { get; set; }
        public string ParentName { get; set; }
        public Nullable<bool> ActiveFlg { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
