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
    public partial class DATA_ACCOUNT: ICloneable
    {
    	public DATA_ACCOUNT () {}
    
    	#region ICloneable Members
        public DATA_ACCOUNT Clone()
        {
            return (DATA_ACCOUNT)this.MemberwiseClone(); 
        }
    
        object ICloneable.Clone()
        {
            return (DATA_ACCOUNT)this.MemberwiseClone();
        }
        #endregion
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public string NameModify { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public Nullable<int> Gender { get; set; }
        public string Email { get; set; }
        public Nullable<int> PositionID { get; set; }
        public string UnitID { get; set; }
        public string Phone { get; set; }
        public string AvatarUrl { get; set; }
        public string TokenDevice { get; set; }
        public Nullable<bool> LockFlg { get; set; }
        public Nullable<System.DateTime> CreateAt { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> UpdateAt { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> DeleteAt { get; set; }
        public string DeleteBy { get; set; }
        public Nullable<bool> DelFlg { get; set; }
        public string HomeTown { get; set; }
        public Nullable<int> Ethnic { get; set; }
        public string Specialized { get; set; }
        public Nullable<int> PoliticalTheory { get; set; }
        public Nullable<System.DateTime> YouthGroupDate { get; set; }
        public Nullable<System.DateTime> CommunistPartyDate { get; set; }
        public Nullable<int> CommunistPartyPosition { get; set; }
        public string Note { get; set; }
        public Nullable<int> Qualification { get; set; }
    }
}
