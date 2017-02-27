using ChakraCore.NET.Core.API;
using System;
using System.Threading;

namespace ChakraCore.NET.Core
{
    public class ChakraRuntime: ServiceConsumerBase,IDisposable
    {
        JavaScriptRuntime runtime;
        public EventWaitHandle SyncHandler { get; private set; }
        //AutoResetEvent SyncHandler;
        private ChakraRuntime(JavaScriptRuntime runtime,IServiceNode service):base(service, "ChakraRuntime")
        {
            this.runtime = runtime;
            SyncHandler = new AutoResetEvent(true);
            //inject service
            service.PushService<IJSValueConverterService>(new JSValueConverterService());
            service.PushService<IProxyMapService>(new ProxyMapService());
            service.PushService<IJSValueService>(new JSValueService());
        }

        public ChakraContext CreateContext(bool enableDebug)
        {
            try
            {
                var c = runtime.CreateContext();
                var result = new ChakraContext(c, SyncHandler,ServiceNode);
                result.Init(enableDebug);
                return result;
                
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            
            //if (enableDebug)
            //{
            //    DebugHelper.Inject(result);
            //}
            
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    runtime.Dispose();
                    SyncHandler.Dispose();
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
