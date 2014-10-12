using System;

namespace Ext.Core.Mapping
{
    public interface IMappingConverter
    {
        Type SourceType { get; }
        Type TargetType { get; }
        ValidatedResult<object> Convert(object source);
    }
}