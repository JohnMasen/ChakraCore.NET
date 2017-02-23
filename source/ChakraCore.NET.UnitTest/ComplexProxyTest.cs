using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ChakraCore.NET.UnitTest
{
    [TestClass]
    public class ComplexProxyTest : UnitTestBase
    {
        protected override void SetupContext()
        {
            TestProxy.Inject(context);
            context.ValueConverter.RegisterArrayConverter<TestProxy>();
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
            context.RootObject.WriteProperty<IEnumerable<TestProxy>>("proxies", items);

            var items_back=context.RootObject.ReadProperty<IEnumerable<TestProxy>>("proxies").ToArray();
            Assert.AreEqual(items_back.Length, items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                Assert.AreSame(items[i], items_back[i]);
            }
            string result=context.RootObject.CallFunction<string>("MultiTransfer");
            Assert.AreEqual<string>(target, result);
        }
    }
}
