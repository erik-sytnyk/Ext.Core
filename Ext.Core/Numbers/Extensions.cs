using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Core.Numbers
{
    public static class Extensions
    {
        public static string ToTrimmedNumberString(this decimal number)
        {
            return TrimFractionalNumberString(number.ToString(CultureInfo.InvariantCulture), '.');
        }

        public static string TrimFractionalNumberString(string franctionalNumberStr, char fractionSeparator)
        {
            var result = franctionalNumberStr;

            if (String.IsNullOrEmpty(result)) return string.Empty;

            if (result.Contains(fractionSeparator))
            {
                result = result.TrimEnd('0');

                if (result.EndsWith(fractionSeparator.ToString(CultureInfo.InvariantCulture)))
                {
                    result = result.TrimEnd(fractionSeparator);
                }

            }

            return result;
        }
    }
}
