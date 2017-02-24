using ChakraCore.NET.API;
using System;
using System.Threading;

namespace ChakraCore.NET
{
    public class ChakraRuntime: IDisposable
    {
        JavaScriptRuntime runtime;
        public EventWaitHandle SyncHandler { get; private set; }
        //AutoResetEvent SyncHandler;
        private ChakraRuntime(JavaScriptRuntime runtime)
        {
            this.runtime = runtime;
            SyncHandler = new AutoResetEvent(true);
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
            
            //if (enableDebug)
            //{
            //    DebugHelper.Inject(result);
            //}
            
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
