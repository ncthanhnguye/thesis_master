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
    public partial class QuestionType: ICloneable
    {
    	public QuestionType () {}
    
    	#region ICloneable Members
        public QuestionType Clone()
        {
            return (QuestionType)this.MemberwiseClone(); 
        }
    
        object ICloneable.Clone()
        {
            return (QuestionType)this.MemberwiseClone();
        }
        #endregion
        public int ID { get; set; }
        public string QuestionType1 { get; set; }
        public string Prefix { get; set; }
        public string AfterFix { get; set; }
        public string Contain { get; set; }
    }
}
