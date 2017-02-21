using ChakraCore.NET.Extension.SharedMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChakraCore.NET.UnitTest
{
    public class ContextTestBase
    {
        protected static ChakraRuntime runtime;
        protected ChakraContext context;
        protected static TestContext TestContext;
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestContext = testContext;
            //runtime = ChakraRuntime.Create();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            runtime = ChakraRuntime.Create();
            context = runtime.CreateContext(true);
            SharedMemoryValueConvertHelper.Inject(context);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            context.Dispose();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            runtime.Dispose();
        }
    }
}