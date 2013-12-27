using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ext.Core.Types
{
    public class ValidatedResult<T>
    {
        public bool IsValid { get; set; }
        public T Value { get; set; }
    }
}
