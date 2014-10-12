using System;

namespace Ext.Core.Mapping
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

            int intValue;

            if (Int32.TryParse((string)source, out intValue))
            {
                result.IsValid = true;
                result.Value = intValue;
            }

            return result;
        }
    }
}