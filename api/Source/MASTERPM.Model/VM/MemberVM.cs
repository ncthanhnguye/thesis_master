using System;
using System.Collections.Generic;

namespace MASTERPM.Model.VM
{
    public class MemberVM
    {
        public string Object { get; set; }
        public int Type { get; set; }
    }


    public class UserMeetingOutput
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string PersonalID { get; set; }
        public int Type { get; set; }
        public string AvatarUrl { get; set; }
        public bool Selected { get; set; }
    }
}
