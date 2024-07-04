using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNPTPM.Model.Validate
{
    public class MinValueAttribute : ValidationAttribute
    {
        private readonly int _minValue;

        public MinValueAttribute(int minValue)
        {
            _minValue = minValue;
        }

        public override bool IsValid(object value)
        {
            try
            {
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    return true;
                }

                var val = -1;
                int.TryParse(value.ToString(), out val);

                return val >= _minValue;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            var errMsg = base.FormatErrorMessage(_minValue.ToString());
            return errMsg;
        }
    }
}
