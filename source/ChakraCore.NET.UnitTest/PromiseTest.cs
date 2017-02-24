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
        private TestProxy proxy = new TestProxy();
        private TimerHelper helper = new TimerHelper();
        protected override void SetupContext()
        {
            context.ValueConverter.RegisterTask<int>();
            TestProxy.Inject(context);
            TimerHelper.RegisterConverter(context);
            context.RootObject.WriteProperty<TestProxy>("test", proxy);
            context.RootObject.WriteProperty<TimerHelper>("timer", helper);
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
