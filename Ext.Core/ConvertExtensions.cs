using System;

namespace Ext.Core
{
    public static class ConvertExtensions
    {
        public static int ConvertToInt(this string str)
        {
            return Convert.ToInt32(str);
        }

        public static bool ConvertToBoolean(this string str)
        {
            return Convert.ToBoolean(str);
        }

        public static decimal ConvertToDecimal(this string str)
        {
            return Convert.ToDecimal(str);
        }

        public static double ConvertToDouble(this string str)
        {            
            return Convert.ToDouble(str);
        }       
    }
}
