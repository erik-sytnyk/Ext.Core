using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ext.Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ext.Core.Tests
{
    [TestClass]
    public class NumberExtensionsTest
    {
        [TestMethod]
        public void Test()
        {
            var fractionNumberStr = "3.00";
            Assert.AreEqual("3", Extensions.TrimFractionalNumberString(fractionNumberStr, '.'));

            fractionNumberStr = "-38";
            Assert.AreEqual("-38", Extensions.TrimFractionalNumberString(fractionNumberStr, '.'));

            fractionNumberStr = "-5.000";
            Assert.AreEqual("-5", Extensions.TrimFractionalNumberString(fractionNumberStr, '.'));

            decimal fractionNumber = -58.00m;
            Assert.AreEqual("-58", fractionNumber.ToTrimmedNumberString());
        } 
    }
}
