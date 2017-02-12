using Chakra.NET.API;
using System.Threading;

namespace Chakra.NET
{
    public class ChakraRuntime
    {
        JavaScriptRuntime runtime;
        AutoResetEvent syncHandler;
        private ChakraRuntime(JavaScriptRuntime runtime)
        {
            this.runtime = runtime;
            syncHandler = new AutoResetEvent(true);
        }

        public ChakraContext CreateContext(bool enableDebug)
        {
            var c = runtime.CreateContext();
            var result = new ChakraContext(c,syncHandler);
            result.init(enableDebug);

            if (enableDebug)
            {
                DebugHelper.Inject(result);
            }
            return result;
        }
        public static ChakraRuntime Create(JavaScriptRuntimeAttributes attributes)
        {
            JavaScriptRuntime result = JavaScriptRuntime.Create(attributes,JavaScriptRuntimeVersion.VersionEdge);
            return new ChakraRuntime(result);
        }

        public static ChakraRuntime Create()
        {
            return Create(JavaScriptRuntimeAttributes.None);
        }
    }
}
