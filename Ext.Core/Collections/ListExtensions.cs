using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Core.Collections
{
    public static class ListExtensions
    {
        public static bool AddIfNotThere<T>(this IList<T> list, T value)
        {
            if (list.Contains(value))
            {
                return false;
            }
            list.Add(value);
            return true;
        }
    }
}
