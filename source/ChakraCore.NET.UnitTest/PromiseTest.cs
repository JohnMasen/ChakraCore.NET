using ChakraCore.NET.Timer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET.UnitTest
{
    [TestClass]
    public class PromiseTest : UnitTestBase
    {
        private TestProxy proxy = new TestProxy();
        protected override void SetupContext()
        {
            TestProxy.Inject(runtime);
            context.GlobalObject.WriteProperty<TestProxy>("test", proxy);
            runScript("Promise");

        }

        [TestMethod]
        public async Task CallPromiseTest()
        {
            int result = await context.GlobalObject.CallFunctionAsync<int>("SimplePromise");
            Assert.AreEqual(1, result);
        }




        [TestMethod]
        public void PromiseCallFromJS()
        {
            context.GlobalObject.CallMethod("CallAsync");
            var n = DateTime.Now;
            while ((DateTime.Now -n).Seconds<5)
            {
                bool isHold = context.GlobalObject.ReadProperty<bool>("hold");
                if (!isHold)
                {
                    int result = context.GlobalObject.ReadProperty<int>("result");
                    Assert.AreEqual(1, result);
                    return;
                }
                Task.Delay(500).Wait();
            }
            throw new TimeoutException("wait for promise callback timedout");
        }


        [TestMethod,ExpectedException(typeof(Promise.PromiseRejectedException))]
        public async Task PromiseReject()
        {
            await context.GlobalObject.CallMethodAsync("PromiseReject");
        }

        [TestMethod, ExpectedException(typeof(Promise.PromiseRejectedException))]
        public async Task PromiseThrowError()
        {
            await context.GlobalObject.CallMethodAsync("PromiseThrowError");
        }
    }
}
