using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ext.Core.Tests
{
    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void should_call_string_format_from_string_instance()
        {
            var formattedString = "My value: {0}".Format(34);

            Assert.AreEqual("My value: 34", formattedString);
        }

        [TestMethod]
        public void last_occurence_of_shold_work_when_there_is_occurence_of_specified_string()
        {
            var testStr = "bla / bla / /xx";
            var afterLastSlash = testStr.AfterLastOccurrenceOf("/");
            Assert.AreEqual("xx", afterLastSlash);

            testStr = "bla / bla/";
            afterLastSlash = testStr.AfterLastOccurrenceOf("/");
            Assert.AreEqual(string.Empty, afterLastSlash);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void last_occurence_of_shold_throw_when_there_is_no_occurence_of_specified_string()
        {
            var testStr = "bla / bla / /xx";
            var afterLastSlash = testStr.AfterLastOccurrenceOf("@");
        }
    }
}
