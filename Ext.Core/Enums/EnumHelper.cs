using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ext.Core.Enums
{
    public static class EnumHelper
    {
        //chached information about system enums, key is enum full type name
        private static Dictionary<string, Dictionary<object, EnumItemAttribute>> _enumsCache = new Dictionary<string, Dictionary<object, EnumItemAttribute>>();

        private static Dictionary<object, EnumItemAttribute> GetEnumInfo(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new Exception("Type of enum expected");
            }
            Dictionary<object, EnumItemAttribute> result = null;
            string key = enumType.FullName;

            if (string.IsNullOrEmpty(key))
            {
                return result;
            }

            if (_enumsCache.ContainsKey(key))
            {
                result = _enumsCache[key];
            }
            else
            {
                result = GetEnumInfoByType(enumType);
                if (!_enumsCache.ContainsKey(key))
                {
                    _enumsCache.Add(key, result);
                }
            }

            return result;
        }

        public static Dictionary<object, EnumItemAttribute> GetEnumInfoByType(Type enumType)
        {
            Dictionary<object, EnumItemAttribute> result = new Dictionary<object, EnumItemAttribute>();

            FieldInfo[] fieldsInfo = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);

            bool isSortOrderInUse = false;

            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                object itemValue = fieldInfo.GetValue(null);
                EnumItemAttribute enumItemAttribute = new EnumItemAttribute();
                foreach (object attribute in fieldInfo.GetCustomAttributes(false))
                {
                    if (attribute is EnumItemAttribute)
                    {
                        enumItemAttribute = attribute as EnumItemAttribute;
                        if (!isSortOrderInUse && enumItemAttribute.SortOrder != Int32.MaxValue)
                        {
                            isSortOrderInUse = true;
                        }
                    }
                }
                if (string.IsNullOrEmpty(enumItemAttribute.ItemName))
                {
                    enumItemAttribute.ItemName = System.Enum.GetName(enumType, itemValue);
                }
                result.Add(itemValue, enumItemAttribute);
            }

            if (isSortOrderInUse)
            {
                List<KeyValuePair<object, EnumItemAttribute>> dicItems = new List<KeyValuePair<object, EnumItemAttribute>>(result);
                dicItems.Sort(
                    delegate(KeyValuePair<object, EnumItemAttribute> firstPair,
                             KeyValuePair<object, EnumItemAttribute> secondPair)
                        {
                            return firstPair.Value.SortOrder.CompareTo(secondPair.Value.SortOrder);
                        });
                result.Clear();
                foreach (KeyValuePair<object, EnumItemAttribute> keyValuePair in dicItems)
                {
                    result.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return result;
        }

        public static Dictionary<TEnumType, EnumItemAttribute> GetEnumInfo<TEnumType>()
        {
            Dictionary<TEnumType, EnumItemAttribute> result = new Dictionary<TEnumType, EnumItemAttribute>();

            foreach (KeyValuePair<object, EnumItemAttribute> info in GetEnumInfo(typeof(TEnumType)))
            {
                result.Add((TEnumType)info.Key, info.Value);
            }

            return result;
        }

        public static string GetTagValueByEnumItem(object enumItem)
        {
            return GetEnumInfo(enumItem.GetType())[enumItem].TagValue;
        }

        public static string GetNameByEnumItem(object enumItem)
        {
            return GetEnumInfo(enumItem.GetType())[enumItem].ItemName;
        }

        public static Dictionary<string, string> GetNames<TEnumType>()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (KeyValuePair<object, EnumItemAttribute> item in GetEnumInfo(typeof(TEnumType)))
            {
                data.Add(item.Key.ToString(), item.Value.ItemName);
            }
            return data;
        }

        public static TEnumType GetEnumItemByTagValue<TEnumType>(string TagValue)
        {
            return (TEnumType)GetEnumItemByTagValue(TagValue, typeof(TEnumType));
        }

        public static object GetEnumItemByTagValue(string TagValue, Type enumType)
        {
            foreach (KeyValuePair<object, EnumItemAttribute> info in GetEnumInfo(enumType))
            {
                if (info.Value.TagValue == TagValue)
                {
                    return info.Key;
                }
            }
            throw new Exception(String.Format("There is no enum item with such TagValue: {0}", TagValue));
        }

        public static TEnumType Parse<TEnumType>(string value)
        {
            return (TEnumType)System.Enum.Parse(typeof(TEnumType), value);
        }

        public static TEnumType GetDefaultValue<TEnumType>()
        {
            return (TEnumType)GetDefaultValueByEnumType(typeof(TEnumType));
        }

        public static object GetDefaultValueByEnumType(Type enumType)
        {
            foreach (KeyValuePair<object, EnumItemAttribute> info in GetEnumInfo(enumType))
            {
                if (info.Value.IsDefaultItem)
                {
                    return info.Key;
                }
            }
            //if there is no default member specified return first one
            foreach (KeyValuePair<object, EnumItemAttribute> info in GetEnumInfo(enumType))
            {
                return info.Key;
            }
            throw new Exception("Enum does not have default value");
        }

        public static List<TEnumType> GetEnumItems<TEnumType>()
        {
            List<TEnumType> result = new List<TEnumType>();

            foreach (KeyValuePair<object, EnumItemAttribute> info in GetEnumInfo(typeof(TEnumType)))
            {
                result.Add((TEnumType)info.Key);
            }

            return result;
        }
    }
}