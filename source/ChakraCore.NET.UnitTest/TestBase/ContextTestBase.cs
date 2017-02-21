using ChakraCore.NET.Extension.SharedMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace ChakraCore.NET.UnitTest
{
    public abstract class UnitTestBase
    {
        protected  ChakraRuntime runtime;
        protected  ChakraContext context;
        protected  static TestContext TestContext;
        private  string logPrefix = string.Empty;
        private  Stack<string> prefixStack = new Stack<string>();


        public bool SharedRuntime { get; private set; }
        public bool SharedContext { get; private set; }
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestContext = testContext;
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
                runtime = ChakraRuntime.Create();
                LogAndPush("Runtime Created");
                context = runtime.CreateContext(true);
                LogAndPush("Context created");
                SetupContext();
                Log("Context setup complete");
            
            
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            CleanupContext();
            context.Dispose();
            context = null;
            LogAndPop("Context released");
            runtime.Dispose();
            runtime = null;
            LogAndPop("Runtime released");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {

        }

        protected string runScript(string fileName)
        {
            return context.RunScript(Properties.Resources.ResourceManager.GetString(fileName));
        }
        protected  void Log(string text)
        {
            Debug.WriteLine(logPrefix + text);
        }
        protected  void LogAndPush(string text, string prefix="--")
        {
            Log(text);
            PushPrefix(prefix);
        }

        protected  void LogAndPop(string text)
        {
            PopPrefix();
            Log(text);
        }

        protected  void PushPrefix(string prefix)
        {
            prefixStack.Push(logPrefix);
            logPrefix = logPrefix+prefix;
        }

        protected  void PopPrefix()
        {
            logPrefix=prefixStack.Pop();
        }
        protected abstract void SetupContext();
        protected virtual void CleanupContext()
        {

        }
    }
}