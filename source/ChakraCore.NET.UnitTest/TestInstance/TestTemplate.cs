using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NewRuntime.NewContext.CoreTest
{
    [TestClass]
    public class CoreTest_NewRuntime_NewContext : ChakraCore.NET.UnitTest.TestDefinition.CoreTest
    {
        public CoreTest_NewRuntime_NewContext() : base(false, false)
        {
        }
    }
}

namespace NewRuntime.SharedContext.CoreTest
{
    [TestClass]
    public class CoreTest_NewRuntime_SharedContext : ChakraCore.NET.UnitTest.TestDefinition.CoreTest
    {
        public CoreTest_NewRuntime_SharedContext() : base(false, true)
        {
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
    }
}

namespace NewRuntime.SharedContext.SharedMemoryObjectsTest
{
    [TestClass]
    public class SharedMemoryObjectsTest_NewRuntime_SharedContext : ChakraCore.NET.UnitTest.TestDefinition.SharedMemoryObjectsTest
    {
        public SharedMemoryObjectsTest_NewRuntime_SharedContext() : base(false, true)
        {
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
    }
}

