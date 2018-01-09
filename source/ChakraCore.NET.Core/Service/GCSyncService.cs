using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class GCSyncService : ServiceBase, IGCSyncService
    {
        private Dictionary<long, object> objectPool = new Dictionary<long, object>();
        private long id = 0;
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

        public void SyncWithJsValue(object obj, JavaScriptValue jsValue)
        {
            contextSwitch.With(() =>
            {
                var currentID = id++;
                JavaScriptContext.SetObjectBeforeCollectCallback(jsValue, new IntPtr(currentID), callback);
                objectPool.Add(currentID, obj);
            });
        }

        private  void JsValueCollectCallback(JavaScriptValue reference, IntPtr callbackState)
        {
            long objID = callbackState.ToInt64();
            if (objectPool.ContainsKey(objID))
            {
                objectPool.Remove(objID);
                return;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Failed to release managed object, invalid object id");
            }

        }
    }
}
