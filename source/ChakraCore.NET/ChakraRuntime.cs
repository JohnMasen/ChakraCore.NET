using ChakraCore.NET.API;
using System;
using System.Threading;


namespace ChakraCore.NET
{
    public class ChakraRuntime: ServiceConsumerBase,IDisposable
    {
        JavaScriptRuntime runtime;
        IProxyMapService mapService = new ProxyMapService();
        JSValueConverterService converter= new JSValueConverterService();
        IRuntimeService runtimeService;
        EventWaitHandle SyncHandler;
        private ChakraRuntime(JavaScriptRuntime runtime,IServiceNode service,EventWaitHandle syncHandler):base(service, "ChakraRuntime")
        {
            this.runtime = runtime;
            SyncHandler = syncHandler;
            runtimeService = new RuntimeService(runtime, SyncHandler);
            //inject service
            ServiceNode.PushService(runtimeService);
            ServiceNode.PushService<IJSValueConverterService>(converter);
            ServiceNode.PushService<IProxyMapService>(mapService);
            ServiceNode.PushService<IJSValueService>(new JSValueService());
            ServiceNode.InjectShareMemoryObjects();
            ServiceNode.InjectTaskService();
            ServiceNode.InjecTimerService();
        }

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
        
        public static ChakraRuntime Create(JavaScriptRuntimeAttributes attributes= JavaScriptRuntimeAttributes.None, IServiceNode service=null,EventWaitHandle syncHandle=null)
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
            return new ChakraRuntime(result,service,syncHandle);
        }


        public void CollectGarbage()
        {
            runtimeService.CollectGarbage();
        }

        /// <summary>
        /// Force terminate running scripts without clean up varibles .
        /// this function cannot stop a infinite loop like (function hang() {while (true) { } })(); 
        /// unless runtime is created with AllowScriptInterrupt attribute
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
                    mapService.Dispose();
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
