using System;

namespace Ext.Core.Converting
{
    public static class ConvertExtensions
    {
        public static int ConvertToInt(this string str)
        {
            return Convert.ToInt32(str);
        }

        public static bool ConverToBoolean(this string str)
        {
            return Convert.ToBoolean(str);
        }

        public static decimal ConverToDecimal(this string str)
        {
            return Convert.ToDecimal(str);
        }

        public static double ConvertToDouble(this string str)
        {
            return Convert.ToDouble(str);
        }
    }
}
