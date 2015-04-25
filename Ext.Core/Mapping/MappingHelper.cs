using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Ext.Core.InternalHelpers;
using Ext.Core.Reflection;

namespace Ext.Core.Mapping
{
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

        public static void Map(object source, object target, MappingOptions options)
        {
            DefaultInstance.MapObject(source, target, options);
        }

        #endregion

        public List<IMappingConverter> Converters { get; set; }

        public MappingHelper()
        {
            Converters = new List<IMappingConverter>();
        }

        public void MapObject(object source, object target)
        {
            MapObject(source, target, new MappingOptions());
        }

        public void MapObject(object source, object target, MappingOptions options)
        {
            MapObject(source, target, options, options.MappingLevel);
        }

        protected void MapObject(object source, object target, MappingOptions options, int recursionCounter)
        {
            if (recursionCounter < 0)
            {
                return;
            }

            Check.NotNull(target, "Target property for mapping should not be null");

            if (source == null)
            {
                return;
            }

            var props = ReflectionHelper.GetProperties(source.GetType());
            foreach (var sourceProperty in props)
            {
                var property = sourceProperty;

                var targetProperty = GetWritableProperties(target).FirstOrDefault(x => x.Name == sourceProperty.Name);

                if (targetProperty == null) continue;

                var sourcePropValue = sourceProperty.GetValue(source);

                if (sourcePropValue == null) continue;

                var sourcePropType = sourceProperty.PropertyType;

                var targetPropValue = targetProperty.GetValue(target);
                var targetPropType = targetProperty.PropertyType;

                
                if (targetPropType == sourcePropType) //simple type
                {
                    targetProperty.SetValue(target, sourcePropValue);
                    continue;
                }

                var converter = this.Converters.SingleOrDefault(x => x.SourceType == sourcePropType && x.TargetType == targetPropType);
                if (converter != null)
                {
                    var convertedResult = converter.Convert(sourcePropValue);
                    if (convertedResult.IsValid)
                    {
                        targetProperty.SetValue(target, convertedResult.Value);
                    }
                    continue;
                }

                if (options.InitNestedClasses && this.IsClonableType(targetPropType))
                {
                    if (targetPropValue == null && targetPropType.HasParameterlessConstructor())
                    {
                        targetPropValue = Activator.CreateInstance(targetPropType);
                        targetProperty.SetValue(target, targetPropValue);
                    }

                    if (targetPropValue != null)
                    {
                        this.MapObject(sourcePropValue, targetPropValue, options, recursionCounter - 1);
                    }
                    continue;
                }

                if (options.InitNestedCollections)
                {
                    var isList = sourcePropType.Implements<IEnumerable>() && this.IsCollectionT(targetPropType) &&
                                 targetPropType.Implements<IList>();
                    if (isList)
                    {
                        if (targetPropValue == null && targetPropType.HasParameterlessConstructor())
                        {
                            targetPropValue = Activator.CreateInstance(targetPropType);
                        }

                        var targetItemType = this.GetCollectionTElementType(targetPropType);
                        if (targetItemType.HasParameterlessConstructor())
                        {
                            var sourceList = sourcePropValue as IEnumerable;
                            var targetList = targetPropValue as IList;

                            foreach (var sourceItem in sourceList)
                            {
                                var targetItem = Activator.CreateInstance(targetItemType);
                                this.MapObject(sourceItem, targetItem, options, recursionCounter - 1);
                                targetList.Add(targetItem);
                            }

                            targetProperty.SetValue(target, targetList);
                        }
                        continue;
                    }
                }
            }

            if (options.FlattenProperties)
            {
                var flattenedProperties = this.GetPropertiesWithFlattenedNested(source, MappingOptions.DefaultFlatteningLevel);

                foreach (var sourceProperty in flattenedProperties)
                {
                    var property = sourceProperty;

                    var targetProperty = GetWritableProperties(target).FirstOrDefault(x => x.Name == sourceProperty.FlattenedName);

                    if (targetProperty == null) continue;

                    var sourcePropType = sourceProperty.PropertyType;

                    var targetPropValue = targetProperty.GetValue(target);
                    var targetPropType = targetProperty.PropertyType;


                    if (targetPropType == sourcePropType) //simple type
                    {
                        targetProperty.SetValue(target, sourceProperty.Value);
                        continue;
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

        public List<FlattenedPropertyDescription> GetPropertiesWithFlattenedNested(object source, int recursionLevel)
        {
            var result = new List<FlattenedPropertyDescription>();

            var properties = ReflectionHelper.GetProperties(source.GetType()).ToList();

            foreach (var propertyInfo in properties)
            {
                if (IsClonableType(propertyInfo.PropertyType))
                {
                    var value = propertyInfo.GetValue(source);

                    if (value != null)
                    {
                        LoadNestedFlattenedProperties(value, propertyInfo.Name, result, recursionLevel);
                    }
                }
            }

            return result;
        }

        protected List<FlattenedPropertyDescription> LoadNestedFlattenedProperties(object source, string prefix, List<FlattenedPropertyDescription> list, int recursionLevel)
        {
            if (recursionLevel < 0)
            {
                return list;
            }

            var props = ReflectionHelper.GetProperties(source.GetType()).ToList();

            foreach (var propertyInfo in props)
            {
                var value = propertyInfo.GetValue(source);

                if (IsClonableType(propertyInfo.PropertyType))
                {
                    if (value != null)
                    {
                        LoadNestedFlattenedProperties(value, prefix + propertyInfo.Name, list, recursionLevel - 1);
                    }
                }
                else
                {
                    var propertyDesc = new FlattenedPropertyDescription();

                    propertyDesc.IsNested = false;
                    propertyDesc.Value = propertyInfo.GetValue(source);
                    propertyDesc.FlattenedName = prefix + propertyInfo.Name;
                    propertyDesc.PropertyType = propertyInfo.PropertyType;

                    list.Add(propertyDesc);
                }
            }

            return list;
        }

        public static IEnumerable<PropertyInfo> GetWritableProperties(object o)
        {
            return ReflectionHelper.GetProperties(o.GetType()).Where(p => p.CanWrite);
        }

        #endregion
    }

    public class MappingOptions
    {
        public const int DefaultMappingLevel = 2;
        public const int DefaultFlatteningLevel = 3;

        public int MappingLevel { get; set; }
        public bool FlattenProperties { get; set; }
        public bool InitNestedClasses { get; set; }
        public bool InitNestedCollections { get; set; }

        public MappingOptions()
        {
            MappingLevel = DefaultMappingLevel;
            FlattenProperties = true;
            InitNestedClasses = true;
            InitNestedCollections = true;
        }

        public MappingOptions WithMappingLevel(int mappingLevel)
        {
            MappingLevel = mappingLevel;
            return this;
        }

        public MappingOptions WithFlattenedProperties(bool enable)
        {
            this.FlattenProperties = enable;
            return this;
        }

        public MappingOptions WithNestedClassesProperties(bool enable)
        {
            this.InitNestedClasses = enable;
            return this;
        }

        public MappingOptions WithNestedCollections(bool enable)
        {
            this.InitNestedCollections = enable;
            return this;
        }
    }

    public class FlattenedPropertyDescription
    {
        public string FlattenedName { get; set; }
        public bool IsNested { get; set; }
        public Type PropertyType { get; set; }
        public object Value { get; set; }
    }
}