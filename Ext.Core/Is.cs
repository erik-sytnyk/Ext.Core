using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Core
{
    public static class Is
    {
        public static class Not
        {
            public static bool Empty(string str)
            {
                return !Is.Empty(str);
            }

            public static bool Empty<T>(IEnumerable<T> collection)
            {
                return !Is.Empty(collection);
            }
        }

        public static bool Empty(string str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static bool Empty<T>(IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }
    }
}
