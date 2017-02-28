using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using ChakraCore.NET.SharedMemory;
using ChakraCore.NET;

namespace ChakraCore.NET.UnitTest
{
    [TestClass]
    public  class CoreTest :UnitTestBase
    {
        

        protected override void SetupContext()
        {
            TestProxy.Inject(runtime);
        }

        [TestMethod]
        public void RunScript()
        {
            var result=runScript("RunScript");
            Assert.AreEqual<string>(result, "test");
        }


        [TestMethod]
        public void ReadWriteTest()
        {
            //System.Diagnostics.Debug.WriteLine("ReadWriteTest start");
            ReadWrite<string>("hello");
            ReadWrite<int>(1000);
            ReadWrite<float>(100.11f);
            ReadWrite<byte>(0x0f);
            ReadWrite<double>(-33.4455);
            ReadWrite<bool>(false);
            //System.Diagnostics.Debug.WriteLine("ReadWriteTest stop");
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
            runScript("JSCall");
            converter.RegisterFunctionConverter<int, int>();
            int result=context.GlobalObject.CallFunction<int, Func<int, int>,int>("addcallback",1,
                (value) => {
                    return value + value;
                }
                );
            Assert.AreEqual(result, 3);
        }

        private void CallMethod<T>(T value)
        {
            runScript("JSCall");
            context.GlobalObject.CallMethod<T>("add",value);
        }
        private void CallFunction<T>(T value,T expect)
        {
            runScript("JSCall");
            T result=context.GlobalObject.CallFunction<T,T>("add", value);
            Assert.AreEqual<T>(result, expect);
        }
        [TestMethod]
        public void ProxyCallback()
        {
            
            TestProxy stub = new TestProxy();
            context.GlobalObject.WriteProperty("myStub", stub);
            runScript("JSProxy");
            string s = context.GlobalObject.ReadProperty<string>("callProxyResult");
            Assert.AreEqual("hihi", s);
        }

        [TestMethod]
        public void ProxyTransferback()
        {
            TestProxy stub = new TestProxy();
            context.GlobalObject.WriteProperty("a", stub);
            runScript("JSValueTest");
            TestProxy b = context.GlobalObject.ReadProperty<TestProxy>("b");
            Assert.IsTrue(object.ReferenceEquals(stub, b));
        }

        




        private void ReadWrite<T>(T value)
        {
            context.GlobalObject.WriteProperty<T>("a", value);
            runScript("JSValueTest");
            T b = context.GlobalObject.ReadProperty<T>("b");
            Assert.AreEqual<T>(b, value);
        }

        
    }
}
