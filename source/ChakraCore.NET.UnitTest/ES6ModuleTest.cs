using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.UnitTest
{
    [TestClass]
    public class ES6ModuleTest : UnitTestBase
    {
        protected override void SetupContext()
        {
        }

        //[TestMethod]
        //public void RunModuleTest()
        //{
        //    var result = runModule("RunScript");
        //    Assert.AreEqual<string>(result.ToString(), "test");
        //}

        [TestMethod]
        public void BasicClassProject()
        {
            //var y=context.GlobalObject.CallFunction<int, int>("add", 1);
            var value = projectModuleClass("BasicExport", "TestClass");
            var result=value.CallFunction<int, int>("Test1", 1);
            Assert.AreEqual(2, result);
        }
    }
}
