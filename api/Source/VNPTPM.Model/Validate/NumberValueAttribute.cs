using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Validate
{
    public class NumberValueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    return true;
                }

                double rs = -1;

                Double.TryParse(value.ToString(), out rs);

                return rs > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
