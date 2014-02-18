using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ext.Core.Mapping
{
    public static class MappingCache
    {
        private static readonly IDictionary<Type, IEnumerable<PropertyInfo>> PropertiesCache = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        private static readonly object Lock = new object();

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            if (!PropertiesCache.ContainsKey(type))
            {
                lock (Lock)
                {
                    if (!PropertiesCache.ContainsKey(type))
                    {
                        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);//.Union(type.GetInterfaces().SelectMany(t => t.GetProperties()));
                        PropertiesCache.Add(type, props);
                    }
                }
            }
            return PropertiesCache[type];
        }

        public static IEnumerable<PropertyInfo> GetProperties(object o)
        {
            return GetProperties(o.GetType());
        }

        public static IEnumerable<PropertyInfo> GetWritableProperties(object o)
        {
            return GetProperties(o.GetType()).Where(p => p.CanWrite);
        }
    }

    public class MappingHelper
    {
        #region Static members
        
        private static MappingHelper DefaultInstance { get; set; }

        static MappingHelper()
        {
            DefaultInstance = new MappingHelper();
            DefaultInstance.RegisterConverter(new StringToIntegerConverter());
        }

        public static void Map(object source, object target)
        {
            DefaultInstance.MapObject(source, target);
        }
        
        #endregion

        public List<IMappingConverter> Converters { get; set; }

        public MappingHelper()
        {
            Converters = new List<IMappingConverter>();
        }

        public void MapObject(object source, object target)
        {
            if (source == null)
            {
                return;
            }

            var props = MappingCache.GetProperties(source).ToList();

            foreach (var sourceProperty in props)
            {
                PropertyInfo property = sourceProperty;
                var targetProperty = MappingCache.GetWritableProperties(target).FirstOrDefault(x => x.Name == property.Name);

                var sourcePropValue = sourceProperty.GetValue(source);
                var sourcePropType = sourceProperty.PropertyType;

                Dictionary<string, int> dic = new Dictionary<string, int>();

                if (targetProperty != null && sourcePropValue != null)
                {
                    var trargetPropValue = targetProperty.GetValue(target);
                    var targetPropType = targetProperty.PropertyType;

                    var converter =
                        this.Converters.SingleOrDefault(x => x.SourceType == sourcePropType && x.TargetType == targetPropType);

                    if (converter != null)
                    {
                        var convertedResult = converter.Convert(sourcePropValue);
                        if (convertedResult.IsValid)
                        {
                            targetProperty.SetValue(target, convertedResult.Value);
                        }
                    } 
                    else if (sourcePropType.Implements<IEnumerable>() && this.IsCollectionT(targetPropType) && targetPropType.Implements<IList>())
                    {
                        if (trargetPropValue == null && targetPropType.HasParameterlessConstructor())
                        {
                            trargetPropValue = Activator.CreateInstance(targetPropType);
                        }

                        var targetItemType = this.GetCollectionTElementType(targetPropType);
                        if (targetItemType.HasParameterlessConstructor())
                        {
                            var sourceList = sourcePropValue as IEnumerable;
                            var targetList = trargetPropValue as IList;

                            foreach (var sourceItem in sourceList)
                            {
                                var targetItem = Activator.CreateInstance(targetItemType);
                                this.MapObject(sourceItem, targetItem);
                                targetList.Add(targetItem);
                            }

                            targetProperty.SetValue(target, targetList);
                        }
                    }
                    else if (this.IsClonableType(targetPropType))
                    {
                        if (trargetPropValue == null && targetPropType.HasParameterlessConstructor())
                        {
                            trargetPropValue = Activator.CreateInstance(targetPropType);
                            targetProperty.SetValue(target, trargetPropValue);
                        }

                        this.MapObject(sourcePropValue, trargetPropValue);
                    }
                    else if (targetPropType == sourcePropType) //simple type
                    {
                        targetProperty.SetValue(target, sourcePropValue);
                    }
                }
            }
        }

        public void RegisterConverter(IMappingConverter converter)
        {        
            this.UnregisterConverter(converter.SourceType, converter.TargetType);
            this.Converters.Add(converter);
        }

        public void UnregisterConverter(Type sourceType, Type targetType)
        {
            this.Converters.RemoveAll(x => x.TargetType == targetType && x.SourceType == sourceType);
        }

        #region Helper methods

        protected bool IsClonableType(Type type)
        {
            if (type.IsValueType || type == typeof(string) || type.Implements<IEnumerable>())
            {
                return false;
            }

            return true;
        }

        public bool IsCollectionT(Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public Type GetCollectionTElementType(Type type)
        {
            return type.GetGenericArguments().Single();
        }

        #endregion
    }
}