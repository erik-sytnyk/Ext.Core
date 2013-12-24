using System.Collections.Generic;

namespace Ext.Core.Collections
{
    public static class DictionaryExtensions
    {
        public static void AddIfNoEntry<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }
    }
}
