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
    public partial class AD_AUTONUMBER: ICloneable
    {
    	public AD_AUTONUMBER () {}
    
    	#region ICloneable Members
        public AD_AUTONUMBER Clone()
        {
            return (AD_AUTONUMBER)this.MemberwiseClone(); 
        }
    
        object ICloneable.Clone()
        {
            return (AD_AUTONUMBER)this.MemberwiseClone();
        }
        #endregion
        public string ID { get; set; }
        public string Format { get; set; }
        public short Step { get; set; }
        public Nullable<long> Current { get; set; }
    }
}
