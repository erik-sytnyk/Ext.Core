using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ext.Core.Tests
{
    [TestClass]
    public class MetaTest
    {
        public class MyClass
        {
            public int Number { get; set; }
        }

        [TestMethod]
        public void should_obtain_property_name()
        {
            var myClass = new MyClass();

            var propName = Meta.Name(() => myClass.Number);
            Assert.AreEqual(propName, "Number");

            propName = myClass.GetPropertyName(x => x.Number);
            Assert.AreEqual(propName, "Number");
        }

        [TestMethod]
        public void check_for_not_null_by_lambda()
        {
            var myClass = new MyClass();

            Check.NotNull(() => myClass.Number);

            string str = "sdf";

            Check.NotNull(() => str);
        }
    }
}
