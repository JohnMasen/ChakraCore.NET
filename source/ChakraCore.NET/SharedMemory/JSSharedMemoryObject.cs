using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public abstract class JSSharedMemoryObject:IDisposable
    {
        public SharedBufferSourceEnum BufferSource { get; private set; }
        public ulong Size { get; private set; }
        public SharedMemoryBuffer Buffer { get; protected set; }
        public JavaScriptValue JSSource { get; protected set; }
        private Action releaseJSValue;

        private Action<SharedMemoryBuffer> InitValue;
        public JSSharedMemoryObject(SharedBufferSourceEnum source,ulong size)
        {
            Size = size;
            BufferSource = source;
        }

        internal virtual void InitNew()
        {
            if (Size>long.MaxValue)
            {
                throw new ArgumentOutOfRangeException("cannot allocate memory larger than long.MaxValue");
            }
            Buffer = new SharedMemoryBuffer((IntPtr)(long)Size);
            Buffer.Initialize(Size);
        }

        internal virtual void InitWindow(IntPtr handle, bool ownsHandle)
        {
            Buffer = new SharedMemoryBuffer(handle, Size, ownsHandle);
        }

        protected virtual void InitWindow(SharedMemoryBuffer source,long position)
        {
            Buffer = source.CreateView(position, Size);
        }

        internal virtual void InitBeforeConvert(SharedMemoryBuffer buffer)
        {
            if (InitValue!=null)
            {
                InitValue(buffer);
                InitValue = null;
            }
        }

        internal void SetupInitValueAction(Action<SharedMemoryBuffer> action)
        {
            if (InitValue!=null)
            {
                throw new InvalidOperationException("cannot call SetupInitValueAction twice");
            }
            if (JSSource.IsValid)
            {
                throw new InvalidOperationException("JSSource already initialized");
            }
            InitValue = action;
        }

        internal void SetJSSource(JavaScriptValue value, ChakraContext context)
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
            releaseJSValue = new Action(() => { context.With(() => value.Release()); });
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
                    Buffer?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~JSSharedMemoryObject() {
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
