using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.UnitTest
{
    [TestClass]
    public class RequireTest : UnitTestBase
    {
        protected override void SetupContext()
        {
            JSRequireLoader.EnableRequire(context,"Script\\Require");
            Log("Promise enabled");
        }
        [TestMethod]
        public void TestRequire()
        {
            runScript("RequestTest");
            string output = context.GlobalObject.ReadProperty<string>("output");
            Assert.AreEqual(output, "abcabc");
        }
    }
}
