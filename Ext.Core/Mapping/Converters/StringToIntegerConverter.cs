using System;

namespace Ext.Core.Mapping.Converters
{
    public class StringToIntegerConverter : IMappingConverter
    {
        public Type SourceType 
        {
            get { return typeof (string); }
        }

        public Type TargetType 
        {
            get { return typeof (int); }
        }
        
        public ValidatedResult<object> Convert(object source)
        {
            var result = new ValidatedResult<object>();

            var str = source.ToString();

            if (str == "Y" || str == "N")
            {
                result.Value = str == "Y";
                result.IsValid = true;
            }

            return result;
        }
    }
}