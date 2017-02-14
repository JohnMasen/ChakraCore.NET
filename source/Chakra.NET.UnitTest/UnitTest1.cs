using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chakra.NET.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }
        [TestMethod,DeploymentItem(@"Script\")]
        public void TestMethod1()
        {
            
            System.IO.File.ReadAllText(@"Script\CoreFeature.js");
        }
    }
}
