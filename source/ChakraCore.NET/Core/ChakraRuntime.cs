using ChakraCore.NET.API;
using System;
using System.Threading;

namespace ChakraCore.NET
{
    public class ChakraRuntime: IDisposable
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
                    // TODO: dispose managed state (managed objects).
                    runtime.Dispose();
                    syncHandler.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ChakraRuntime() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
