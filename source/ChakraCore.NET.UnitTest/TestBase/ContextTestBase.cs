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
        protected static ChakraRuntime runtime;
        protected static ChakraContext context;
        protected static TestContext TestContext;
        private static string logPrefix = string.Empty;
        private static Stack<string> prefixStack = new Stack<string>();


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
            logPrefix = string.Empty;
            prefixStack = new Stack<string>();
        }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            if (!SharedRuntime)
            {
                if (runtime!=null)
                {
                    runtime.Dispose();
                    runtime = null;
                    LogAndPop("Runtime released");
                }
                runtime = ChakraRuntime.Create();
                LogAndPush("Runtime Created");
            }
            else if (runtime==null)
            {
                runtime = ChakraRuntime.Create();
                LogAndPush("Runtime Created");
            }
            if (!SharedContext)
            {
                if (context!=null)
                {
                    CleanupContext();
                    context.Dispose();
                    context = null;
                    LogAndPop("Context released");
                }
                context = runtime.CreateContext(true);
                LogAndPush("Context created");
                SetupContext();
                Log("Context setup complete");
            }
            else if(context==null)
            {
                context = runtime.CreateContext(true);
                LogAndPush("Context created");
                SetupContext();
                Log("Context setup complete");
            }
            
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            if (!SharedContext)
            {
                CleanupContext();
                context.Dispose();
                context = null;
                LogAndPop("Context released");
            }
            if (!SharedRuntime)
            {
                runtime.Dispose();
                runtime = null;
                LogAndPop("Runtime released");
            }
            
        }

        [ClassCleanup]
        public static void ClassCleanup()//this method is never called??
        {
            context.Dispose();
            context = null;
            LogAndPop("Context released");
            runtime.Dispose();
            runtime = null;
            LogAndPop("Runtime released");
        }

        protected string runScript(string fileName)
        {
            return context.RunScript(Properties.Resources.ResourceManager.GetString(fileName));
        }
        protected static void Log(string text)
        {
            Debug.WriteLine(logPrefix + text);
        }
        protected static void LogAndPush(string text, string prefix="--")
        {
            Log(text);
            PushPrefix(prefix);
        }

        protected static void LogAndPop(string text)
        {
            PopPrefix();
            Log(text);
        }

        protected static void PushPrefix(string prefix)
        {
            prefixStack.Push(logPrefix);
            logPrefix = logPrefix+prefix;
        }

        protected static void PopPrefix()
        {
            logPrefix=prefixStack.Pop();
        }
        protected abstract void SetupContext();
        protected abstract void CleanupContext();
    }
}