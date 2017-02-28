using ChakraCore.NET.Core.API;
using System;
using System.Threading;

namespace ChakraCore.NET.Core
{
    public class ChakraRuntime: ServiceConsumerBase,IDisposable
    {
        JavaScriptRuntime runtime;
        IProxyMapService mapService = new ProxyMapService();
        IRuntimeService runtimeService;
        //AutoResetEvent SyncHandler;
        private ChakraRuntime(JavaScriptRuntime runtime,IServiceNode service):base(service, "ChakraRuntime")
        {
            this.runtime = runtime;
            runtimeService = new RuntimeService(runtime);
            //inject service
            service.PushService(runtimeService);
            if (!service.CanGetService<IEventWaitHandlerService>())
            {
                service.PushService<IEventWaitHandlerService>(new EventWaitHandlerService());
            }
            
            service.PushService<IJSValueConverterService>(new JSValueConverterService());
            service.PushService<IProxyMapService>(mapService);
            service.PushService<IJSValueService>(new JSValueService());
        }

        public ChakraContext CreateContext(bool enableDebug)
        {
            try
            {
                var c = runtime.CreateContext();
                var result = new ChakraContext(c, this);
                result.Init(enableDebug);
                return result;
                
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        
        public static ChakraRuntime Create(JavaScriptRuntimeAttributes attributes= JavaScriptRuntimeAttributes.None, IServiceNode service=null)
        {
            JavaScriptRuntime result = JavaScriptRuntime.Create(attributes, JavaScriptRuntimeVersion.VersionEdge);
            if (service==null)
            {
                service = Core.ServiceNode.CreateRoot();
            }
            return new ChakraRuntime(result,service);
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
