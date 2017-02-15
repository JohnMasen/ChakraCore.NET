using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Microsoft.Extensions.Logging;
namespace Chakra.NET.UnitTest
{
    [TestClass,TestCategory("Core Features")]
    public class CoreFeatureTest
    {
        static ChakraRuntime runtime;
        ChakraContext context;
        static TestContext TestContext;
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestContext = testContext;
            runtime = ChakraRuntime.Create();
            ChakraLogging.LoggerFactory.AddDebug(LogLevel.Debug);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            context = runtime.CreateContext(true);
        }



        [DeploymentItem(@"Script\Core\")]
        [TestMethod]
        [Ignore]
        public void TestDeploymentItem()
        {
            Assert.IsTrue(File.Exists("RunScript.js"));
        }


        

        [TestMethod]
        public void RunScript()
        {
            var result=context.RunScript(TestHelper.JSRunScript);
            Assert.AreEqual<string>(result, "test");
        }


        [TestMethod]
        public void ReadWriteTest()
        {
            ReadWrite<string>("hello");
            ReadWrite<int>(1000);
            ReadWrite<float>(100.11f);
            ReadWrite<byte>(0x0f);
            ReadWrite<double>(-33.4455);
            ReadWrite<bool>(false);
        }

        [TestMethod]
        public void CallMethodTest()
        {
            CallMethod<string>("hello");
            CallMethod<int>(1000);
            CallMethod<float>(100.11f);
            CallMethod<byte>(0x0f);
            CallMethod<double>(-33.4455);
            CallMethod<bool>(false);
        }

        [TestMethod]
        public void CallFunctionTest()
        {
            CallFunction<string>("hi", "hihi");
            CallFunction<int>(1, 2);
            CallFunction<float>(1.1f, 2.2f);
            CallFunction<byte>(0x01, 0x02);
            CallFunction<double>(10.1, 20.2);
        }


        [TestMethod]
        public void CallBackTest()
        {
            context.RunScript(TestHelper.JSCall);
            context.ValueConverter.RegisterCallbackFunctionConverter<int, int>();
            int result=context.RootObject.CallFunction<int, Func<int, int>,int>("addcallback",1,
                (value) => {
                    return value + value;
                }
                );
            Assert.AreEqual(result, 3);
        }

        private void CallMethod<T>(T value)
        {
            context.RunScript(TestHelper.JSCall);
            context.RootObject.CallMethod<T>("add",value);
        }
        private void CallFunction<T>(T value,T expect)
        {
            context.RunScript(TestHelper.JSCall);
            T result=context.RootObject.CallFunction<T,T>("add", value);
            Assert.AreEqual<T>(result, expect);
        }
        [TestMethod]
        public void ProxyCallback()
        {
            TestStub.RegisterValueConverter(context);
            TestStub stub = new TestStub();
            context.RootObject.WriteProperty("myStub", stub);
            var result=context.RunScript(TestHelper.JSProxy);
            string s = context.RootObject.ReadProperty<string>("callProxyResult");
            Assert.AreEqual("hihi", s);
        }

        private void ReadWrite<T>(T value)
        {
            context.RootObject.WriteProperty<T>("a", value);
            context.RunScript(TestHelper.JSValueTest);
            T b = context.RootObject.ReadProperty<T>("b");
            Assert.AreEqual<T>(b, value);
        }

    }
}
