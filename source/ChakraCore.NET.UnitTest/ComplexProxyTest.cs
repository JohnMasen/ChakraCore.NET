using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ChakraCore.NET;

namespace ChakraCore.NET.UnitTest
{
    [TestClass]
    public class ComplexProxyTest : UnitTestBase
    {
        protected override void SetupContext()
        {
            TestProxy.Inject(runtime);
            converter.RegisterArrayConverter<TestProxy>();
            context.GlobalObject.WriteProperty<TestProxy>("proxy", new TestProxy());
            runScript("ComplexProxy");
        }

        [TestMethod]
        public void MultiProxyTransfer()
        {
            int size = 10;
            TestProxy[] items = new TestProxy[size];
            string target = string.Empty;
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new TestProxy(i.ToString());
                target = target + i.ToString() + ",";
            }
            context.GlobalObject.WriteProperty<IEnumerable<TestProxy>>("proxies", items);

            string result=context.GlobalObject.CallFunction<string>("MultiTransfer");
            Assert.AreEqual<string>(target, result);
        }

        [TestMethod]
        public void CallBackFromJS()
        {
            context.GlobalObject.CallMethod("callBackToProxy");
        }
    }
}
