using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class GCSyncService : ServiceBase, IGCSyncService
    {
        private JavaScriptObjectBeforeCollectCallback callback;
        public GCSyncService()
        {
            callback = new JavaScriptObjectBeforeCollectCallback(JsValueCollectCallback);
        }
        
        private class JsValueHolder:IDisposable
        {
            IContextSwitchService contextSwitch;
            JavaScriptValue value;
            public JsValueHolder(IContextSwitchService contextSwitch,JavaScriptValue value)
            {
                this.contextSwitch = contextSwitch;
                contextSwitch.With(()=> 
                {
                    value.AddRef();
                });
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
                        contextSwitch.With(() =>
                        {
                            value.Release();
                        });
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            //TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
             ~JsValueHolder()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(false);
            }

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
        public IDisposable CreateJsGCWrapper(JavaScriptValue jsValue)
        {
            return new JsValueHolder(contextSwitch, jsValue);
        }

        public GCHandle SyncWithJsValue(object obj, JavaScriptValue jsValue)
        {
            GCHandle handle = GCHandle.Alloc(obj);
            IntPtr p = GCHandle.ToIntPtr(handle);
            contextSwitch.With(() =>
            {
                JavaScriptContext.SetObjectBeforeCollectCallback(jsValue, p, callback);
                
            });
            return handle;
        }

        private  void JsValueCollectCallback(JavaScriptValue reference, IntPtr callbackState)
        {
            GCHandle handle = GCHandle.FromIntPtr(callbackState);
            handle.Free();
        }
    }
}
