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
        private TimerHelper helper = new TimerHelper();
        protected override void SetupContext()
        {
            runtime.InjectTaskService();
            TestProxy.Inject(runtime);
            TimerHelper.RegisterTimer(runtime);
            context.GlobalObject.WriteProperty<TestProxy>("test", proxy);
            context.GlobalObject.WriteProperty<TimerHelper>("timer", helper);
            runScript("Promise");

        }

        [TestMethod]
        public async Task CallPromiseTest()
        {
            Task<int> t = context.GlobalObject.ReadProperty<Task<int>>("SimplePromise");
            int result = await t;
            Assert.AreEqual(1, result);
        }




        [TestMethod]
        public void PromiseCallFromJS()
        {
            context.GlobalObject.CallMethod("CallAsync");
            while (true)
            {
                bool isHold = context.GlobalObject.ReadProperty<bool>("hold");
                if (!isHold)
                {
                    int result = context.GlobalObject.ReadProperty<int>("result");
                    Assert.AreEqual(1, result);
                    return;
                }
            }
        }
    }
}
