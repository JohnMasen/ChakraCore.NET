using ChakraCore.NET.API;
using System;
using System.Threading;


namespace ChakraCore.NET
{
    /// <summary>
    /// A helper class wraps the key feature of chakracore runtime
    /// </summary>
    public class ChakraRuntime: ServiceConsumerBase,IDisposable
    {
        JavaScriptRuntime runtime;
        JSValueConverterService converter= new JSValueConverterService();
        IRuntimeService runtimeService;
        EventWaitHandle SyncHandler;
        private ChakraRuntime(JavaScriptRuntime runtime,IServiceNode service,EventWaitHandle syncHandler):base(service, "ChakraRuntime")
        {
            this.runtime = runtime;
            SyncHandler = syncHandler;
            runtimeService = new RuntimeService(runtime);
            //inject service
            ServiceNode.PushService(runtimeService);
            ServiceNode.PushService<IJSValueConverterService>(converter);
            ServiceNode.PushService<IJSValueService>(new JSValueService());
            ServiceNode.PushService<IRuntimeDebuggingService>(new RuntimeDebuggingService(runtime));
            ServiceNode.InjectShareMemoryObjects();
            ServiceNode.InjecTimerService();
            converter.RegisterTask();
        }

        /// <summary>
        /// Create a new context for script execution
        /// </summary>
        /// <param name="enableDebug">Not used by now</param>
        /// <returns></returns>
        public ChakraContext CreateContext(bool enableDebug)
        {
            try
            {
                var c = runtime.CreateContext();
                var result = new ChakraContext(c, this, SyncHandler);
                result.Init(enableDebug);
                return result;
                
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        
        /// <summary>
        /// Create a ChakraRuntime
        /// </summary>
        /// <param name="attributes">Runtime creation attributes</param>
        /// <param name="service">Parent serviceNode, default is null</param>
        /// <param name="syncHandle">Runtime thread sync hanlder, default is null</param>
        /// <param name="memoryLimit">Memory limit for the runtime in bytes, default is ulong.MaxValue</param>
        /// <returns></returns>
        public static ChakraRuntime Create(JavaScriptRuntimeAttributes attributes= JavaScriptRuntimeAttributes.None, IServiceNode service=null,EventWaitHandle syncHandle=null,ulong memoryLimit=ulong.MaxValue)
        {
            JavaScriptRuntime result = JavaScriptRuntime.Create(attributes, JavaScriptRuntimeVersion.VersionEdge);
            if (service==null)
            {
                service = ChakraCore.NET.ServiceNode.CreateRoot();
            }
            if (syncHandle==null)
            {
                syncHandle = new AutoResetEvent(true);
            }
            if (memoryLimit!=ulong.MaxValue)
            {
                result.MemoryLimit = new UIntPtr(memoryLimit);
            }
            return new ChakraRuntime(result,service,syncHandle);
        }

        /// <summary>
        /// Force runtime perform the garbage collection (GC) operation
        /// </summary>
        public void CollectGarbage()
        {
            runtimeService.CollectGarbage();
        }

        /// <summary>
        /// Force terminate running scripts without clean up varibles .
        /// <para> this function cannot stop a infinite loop like (function hang() {while (true) { } })(); </para>
        /// <para>unless runtime is created with AllowScriptInterrupt attribute</para>
        /// </summary>
        public void TerminateRuningScript()
        {
            runtimeService.TerminateRuningScript();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    runtime.Dispose();
                }
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
