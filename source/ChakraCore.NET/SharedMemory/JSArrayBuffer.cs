using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using System.Runtime.InteropServices;
using System.IO;

namespace ChakraCore.NET
{
    public class JSArrayBuffer:IDisposable
    {
        public SharedMemoryBuffer Buffer { get; private set; }
        public ArrayBufferSourceEnum BufferSource { get; private set; }
        public JavaScriptValue JSSource { get; private set; }
        private Action releaseJSValue;
        internal Action<SharedMemoryBuffer> initDefault;

        private JSArrayBuffer(ArrayBufferSourceEnum source)
        {
            System.Diagnostics.Debug.WriteLine("JSArryBufferCreated");
            BufferSource = source;
        }

        internal void SetJSSource(JavaScriptValue value,ChakraContext context)
        {
            if (JSSource.IsValid)
            {
                throw new InvalidOperationException("cannot set jsvalue twice");
            }
            if (!value.IsValid)
            {
                throw new ArgumentException("not a valid javascriptvalue", nameof(value));
            }
            value.AddRef();
            JSSource = value;
            releaseJSValue = new Action(() => { context.With(()=>value.Release()); });
        }

        internal void InitBuffer(IntPtr handle,ulong size,bool ownsHandle)
        {
            Buffer = new SharedMemoryBuffer(handle, size, ownsHandle);
        }

        internal void InitBuffer(int size)
        {
            Buffer = new SharedMemoryBuffer(size);
        }

        internal void InitDefaultValueInJS()
        {
            initDefault?.Invoke(Buffer);
        }

        public static JSArrayBuffer Create(int size)
        {
            var result = new JSArrayBuffer(ArrayBufferSourceEnum.CreateByDotnet);
            result.InitBuffer(size);
            return result;
        }

        internal static JSArrayBuffer CreateFromJS(IntPtr handle,uint size,JavaScriptValue value,ChakraContext context)
        {
            var result = new JSArrayBuffer(ArrayBufferSourceEnum.CreateByJavascript);
            result.InitBuffer(handle, size, false);//memory owned by js, do not release when object is disposed
            result.SetJSSource(value,context);
            return result;
        }

        public static JSArrayBuffer CreateFromExternal(IntPtr handle,ulong size)
        {
            var result = new JSArrayBuffer(ArrayBufferSourceEnum.CreateByExternal);
            result.InitBuffer(handle, size, false);
            return result;
        }

        public static JSArrayBuffer CreateInJavascript(uint size,Action<SharedMemoryBuffer> init)
        {
            return new JSArrayBuffer(ArrayBufferSourceEnum.CreateInJavascript);
            //do not init the buffer, buffer will be initialized when it's passed to javascript
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
                    releaseJSValue?.Invoke();
                    Buffer.Dispose();
                    disposedValue = true;
                    System.Diagnostics.Debug.WriteLine("JSArryBufferReleased");
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~JSArrayBuffer() {
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
