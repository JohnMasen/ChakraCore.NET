using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Extension.SharedMemory
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
                    releaseJSValue?.Invoke();
                    Buffer?.Dispose();
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
