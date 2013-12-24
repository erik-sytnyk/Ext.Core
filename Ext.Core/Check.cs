using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Core
{
    public static class Check
    {
        public static void That<TE>(bool condition, string message) where TE : Exception
        {
            if (!condition)
            {
                throw (TE)Activator.CreateInstance(typeof(TE), message);
            }
        }

        public static void That(bool condition, string message)
        {
            That<Exception>(condition, message);
        }

        public static void NotReachable(string message)
        {
            throw new Exception(message);
        }

        public static void NotNull(object obj, string message)
        {
            Check.That(obj != null, message);
        }
    }
}
