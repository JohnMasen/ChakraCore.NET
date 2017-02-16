using ChakraCore.NET.API;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace ChakraCore.NET
{
    public class ChakraRuntime:LoggableObjectBase<ChakraRuntime>
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
            try
            {
                var c = runtime.CreateContext();
                var result = new ChakraContext(c, syncHandler);
                result.Init(enableDebug);
                log.LogInformation("context created");
                return result;
                
            }
            catch (System.Exception ex)
            {
                log.LogCritical(ChakraLogging.EventIds.CoreEvent, ex, "failed creating context");
                throw ex;
            }
            
            //if (enableDebug)
            //{
            //    DebugHelper.Inject(result);
            //}
            
        }
        public static ChakraRuntime Create(JavaScriptRuntimeAttributes attributes,ILoggerFactory loggerFactory=null)
        {
            if (loggerFactory==null)
            {
                loggerFactory = new LoggerFactory();
            }
            JavaScriptRuntime result = JavaScriptRuntime.Create(attributes,JavaScriptRuntimeVersion.VersionEdge);
            return new ChakraRuntime(result);
        }

        public static ChakraRuntime Create()
        {
            return Create(JavaScriptRuntimeAttributes.None);
        }

        public void CollectGarbage()
        {
            runtime.CollectGarbage();
        }

        /// <summary>
        /// Force terminate running scripts without clean up varibles .
        /// this function cannot stop a infinite loop like (function hang() {while (true) { } })(); 
        /// unless runtime is created with AllowScriptInterrupt attribute
        /// </summary>
        public void TerminateRuningScript()
        {
            runtime.Disabled = true;
            runtime.Disabled = false;
        }

    }
}
