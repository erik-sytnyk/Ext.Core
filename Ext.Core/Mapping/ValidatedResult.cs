namespace Ext.Core.Mapping
{
    public class ValidatedResult<T>
    {
        public bool IsValid { get; set; }
        public T Value { get; set; }
    }
}
