using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ext.Core.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ext.Core.Tests
{
    [TestClass]
    public class DictionaryExtensionsTest
    {
        [TestMethod]
        public void add_if_no_entry_test()
        {
            var dictionary = new Dictionary<int, string>();

            dictionary.AddIfNoEntry(1, "First");
            dictionary.AddIfNoEntry(1, "First item");
            dictionary.AddIfNoEntry(2, "Second");

            Assert.AreEqual(dictionary[1], "First");
        }
    }
}
