//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MASTERPM.Model
{
    using System;
    using System.Collections.Generic;
    
    [Serializable]
    public partial class DATA_5_Khoan: ICloneable
    {
    	public DATA_5_Khoan () {}
    
    	#region ICloneable Members
        public DATA_5_Khoan Clone()
        {
            return (DATA_5_Khoan)this.MemberwiseClone(); 
        }
    
        object ICloneable.Clone()
        {
            return (DATA_5_Khoan)this.MemberwiseClone();
        }
        #endregion
        public int ID { get; set; }
        public Nullable<int> Index { get; set; }
        public Nullable<int> LuatID { get; set; }
        public Nullable<int> ChuongID { get; set; }
        public Nullable<int> MucID { get; set; }
        public Nullable<int> DieuID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Nullable<System.Guid> LuatUUID { get; set; }
        public Nullable<System.Guid> ChuongUUID { get; set; }
        public Nullable<System.Guid> MucUUID { get; set; }
        public Nullable<System.Guid> DieuUUID { get; set; }
        public Nullable<System.Guid> KhoanUUID { get; set; }
    }
}
