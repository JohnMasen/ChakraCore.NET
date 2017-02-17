using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ChakraCore.NET
{
    /// <summary>
    /// a varient which shares memory with chakra
    /// allows fast data exchange between dotnet and javascript
    /// </summary>
    public partial class JSExternalArrayBuffer:IDisposable
    {
        public int Size { get; private set; }
        internal readonly IntPtr data;
        internal readonly int memorySize;
        /// <summary>
        /// init an arraybuffer
        /// </summary>
        /// <param name="size">buffer size in bytes</param>
        public JSExternalArrayBuffer(int size)
        {
            this.Size = size;
            memorySize = Marshal.SizeOf<byte>() * Size;
            data = Marshal.AllocHGlobal(memorySize);
        }


        /// <summary>
        /// read data from shared memory 
        /// </summary>
        public void ReadBuffer(byte[] buffer)
        {
            if (!checkSize(buffer,out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(data, buffer, 0, memorySize);
        }

        public void WriteBuffer(byte[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(buffer, 0, data, memorySize);
        }

        private bool checkSize<T>(T[] array,out int arraySize)
        {
            arraySize = Marshal.SizeOf<T>() * array.Length;
            return arraySize == memorySize;
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                Marshal.FreeHGlobal(data);
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~JSExternalArrayBuffer()
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
}
