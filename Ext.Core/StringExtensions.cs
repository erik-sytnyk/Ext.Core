using System;

namespace Ext.Core
{
    public static class StringExtensions
    {
        public static string Format(this string inputStr, params object[] args)
        {
            return String.Format(inputStr, args);
        }

        public static string AfterLastOccurrenceOf(this string inputStr, string separatorStr)
        {
            Check.That(inputStr.Contains(separatorStr), String.Format(@"""{0}"" does not have occurences of ""{1}""", inputStr, separatorStr));

            var lastIndex = inputStr.LastIndexOf(separatorStr, StringComparison.Ordinal);

            return inputStr.Substring(lastIndex + 1);
        }
    }
}
