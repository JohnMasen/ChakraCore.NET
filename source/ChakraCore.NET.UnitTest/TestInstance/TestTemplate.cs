using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NewRuntime.NewContext.CoreTest
{
    [TestClass]
    public class CoreTest_NewRuntime_NewContext : ChakraCore.NET.UnitTest.TestDefinition.CoreTest
    {
        public CoreTest_NewRuntime_NewContext() : base(false, false)
        {
        }

        protected override void CleanupContext()
        {
            LogAndPop("CoreTest_NewRuntime_NewContext Start");
        }

        protected override void SetupContext()
        {
            base.SetupContext();
            LogAndPush("CoreTest_NewRuntime_NewContext Start");
        }
    }
}

namespace SharedRuntime.NewContext.CoreTest
{
    [TestClass]
    public class CoreTest_SharedRuntime_NewContext : ChakraCore.NET.UnitTest.TestDefinition.CoreTest
    {
        public CoreTest_SharedRuntime_NewContext() : base(true, false)
        {
        }

        protected override void CleanupContext()
        {
            LogAndPop("CoreTest_SharedRuntime_NewContext Start");
        }

        protected override void SetupContext()
        {
            base.SetupContext();
            LogAndPush("CoreTest_SharedRuntime_NewContext Start");
        }
    }
}

namespace SharedRuntime.SharedContext.CoreTest
{
    [TestClass]
    public class CoreTest_SharedRuntime_SharedContext : ChakraCore.NET.UnitTest.TestDefinition.CoreTest
    {
        public CoreTest_SharedRuntime_SharedContext() : base(true, true)
        {
        }

        protected override void CleanupContext()
        {
            LogAndPop("CoreTest_SharedRuntime_SharedContext Start");
        }

        protected override void SetupContext()
        {
            base.SetupContext();
            LogAndPush("CoreTest_SharedRuntime_SharedContext Start");
        }
    }
}

namespace NewRuntime.NewContext.SharedMemoryObjectsTest
{
    [TestClass]
    public class SharedMemoryObjectsTest_NewRuntime_NewContext : ChakraCore.NET.UnitTest.TestDefinition.SharedMemoryObjectsTest
    {
        public SharedMemoryObjectsTest_NewRuntime_NewContext() : base(false, false)
        {
        }

        protected override void CleanupContext()
        {
            LogAndPop("SharedMemoryObjectsTest_NewRuntime_NewContext Start");
        }

        protected override void SetupContext()
        {
            base.SetupContext();
            LogAndPush("SharedMemoryObjectsTest_NewRuntime_NewContext Start");
        }
    }
}

namespace SharedRuntime.NewContext.SharedMemoryObjectsTest
{
    [TestClass]
    public class SharedMemoryObjectsTest_SharedRuntime_NewContext : ChakraCore.NET.UnitTest.TestDefinition.SharedMemoryObjectsTest
    {
        public SharedMemoryObjectsTest_SharedRuntime_NewContext() : base(true, false)
        {
        }

        protected override void CleanupContext()
        {
            LogAndPop("SharedMemoryObjectsTest_SharedRuntime_NewContext Start");
        }

        protected override void SetupContext()
        {
            base.SetupContext();
            LogAndPush("SharedMemoryObjectsTest_SharedRuntime_NewContext Start");
        }
    }
}

namespace SharedRuntime.SharedContext.SharedMemoryObjectsTest
{
    [TestClass]
    public class SharedMemoryObjectsTest_SharedRuntime_SharedContext : ChakraCore.NET.UnitTest.TestDefinition.SharedMemoryObjectsTest
    {
        public SharedMemoryObjectsTest_SharedRuntime_SharedContext() : base(true, true)
        {
        }

        protected override void CleanupContext()
        {
            LogAndPop("SharedMemoryObjectsTest_SharedRuntime_SharedContext Start");
        }

        protected override void SetupContext()
        {
            base.SetupContext();
            LogAndPush("SharedMemoryObjectsTest_SharedRuntime_SharedContext Start");
        }
    }
}

