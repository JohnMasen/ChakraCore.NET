using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ChakraCore.NET.SharedMemory
{
    public class SharedMemoryBuffer : SafeBuffer
    {
        public IntPtr DataAddress  =>  handle;
        public SharedMemoryBuffer(int size):base(true)
        {
            var h=Marshal.AllocHGlobal(size);
            SetHandle(h);
            Initialize((ulong)size);
        }
        public SharedMemoryBuffer(IntPtr data,int size) : base(false)
        {
            SetHandle(data);
            Initialize((ulong)size);
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }
    }
}
