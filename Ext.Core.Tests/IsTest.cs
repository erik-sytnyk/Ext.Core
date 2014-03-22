using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ext.Core.Tests
{
    [TestClass]
    public class IsTest
    {
        [TestMethod]
        public void empty_should_work_for_strings()
        {
            var myStr = (string)null;

            Assert.IsTrue(Is.Empty(myStr));
            Assert.IsFalse(Is.Not.Empty(myStr));

            myStr = "";

            Assert.IsTrue(Is.Empty(myStr));
            Assert.IsFalse(Is.Not.Empty(myStr));

            myStr = "test";

            Assert.IsFalse(Is.Empty(myStr));
            Assert.IsTrue(Is.Not.Empty(myStr));
        }

        [TestMethod]
        public void empty_should_work_for_collections()
        {
            var myList = (List<string>)null;

            Assert.IsTrue(Is.Empty(myList));
            Assert.IsFalse(Is.Not.Empty(myList));

            myList = new List<string>();

            Assert.IsTrue(Is.Empty(myList));
            Assert.IsFalse(Is.Not.Empty(myList));

            myList = new List<string>();
            myList.Add("my_string");

            Assert.IsFalse(Is.Empty(myList));
            Assert.IsTrue(Is.Not.Empty(myList));
        }
    }
}
