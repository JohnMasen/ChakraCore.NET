using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Extension.Promise;
using System.Threading.Tasks;

namespace ChakraCore.NET.UnitTest
{
    [TestClass]
    public class PromiseTest : UnitTestBase
    {
        protected override void SetupContext()
        {
            context.ValueConverter.RegisterTask<int>();
            TestProxy.Inject(context);
            TimerHelper.RegisterConverter(context);
            context.RootObject.WriteProperty<TestProxy>("test", new TestProxy());
            context.RootObject.WriteProperty<TimerHelper>("timer", new TimerHelper());
            runScript("Promise");
            
        }

        [TestMethod]
        public async Task CallPromiseTest()
        {
            Task <int> t= context.RootObject.ReadProperty<Task<int>>("SimplePromise");
            int result = await t;
            Assert.AreEqual(1, result);
        }
        

        [TestMethod]
        public void PromiseCallFromJS()
        {
            context.RootObject.CallMethod("CallAsync");
            while (true)
            {
                bool isHold = context.RootObject.ReadProperty<bool>("hold");
                if (!isHold)
                {
                    int result = context.RootObject.ReadProperty<int>("result");
                    Assert.AreEqual(1, result);
                    return;
                }
            }
        }
    }
}
