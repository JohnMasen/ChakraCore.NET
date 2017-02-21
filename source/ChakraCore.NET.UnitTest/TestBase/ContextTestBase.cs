using ChakraCore.NET.Extension.SharedMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace ChakraCore.NET.UnitTest
{
    public abstract class UnitTestBase
    {
        protected static ChakraRuntime runtime;
        protected static ChakraContext context;
        protected static TestContext TestContext;

        public bool SharedRuntime { get; private set; }
        public bool SharedContext { get; private set; }
        public UnitTestBase(bool shareRuntime,bool shareContext)
        {
            SharedRuntime = shareRuntime;
            SharedContext = shareContext;
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestContext = testContext;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            if (!SharedRuntime)
            {
                runtime = ChakraRuntime.Create();
            }
            else if (runtime==null)
            {
                runtime = ChakraRuntime.Create();
            }
            if (!SharedContext)
            {
                context = runtime.CreateContext(true);
                SetupContext();
                
            }
            else if(context==null)
            {
                context = runtime.CreateContext(true);
                SetupContext();
            }
            
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

        protected string runScript(string fileName)
        {
            return context.RunScript(Properties.Resources.ResourceManager.GetString(fileName));
        }

        protected abstract void SetupContext();
    }
}