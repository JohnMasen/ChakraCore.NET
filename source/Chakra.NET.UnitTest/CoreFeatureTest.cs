using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Chakra.NET.UnitTest
{
    [TestClass]
    public class CoreFeatureTest
    {
        [DeploymentItem(@"Script\Core\")]
        [TestMethod]
        [Ignore]
        public void TestDeploymentItem()
        {
            Assert.IsTrue(File.Exists("RunScript.js"));
        }


        [TestMethod]
        public void CreateContext()
        {
            TestHelper.CreateContext();
        }

        [TestMethod]
        public void RunScript()
        {
            var result=TestHelper.CreateContext().RunScript(TestHelper.JSRunScript);
            Assert.AreEqual<string>(result, "test");
        }
    }
}
