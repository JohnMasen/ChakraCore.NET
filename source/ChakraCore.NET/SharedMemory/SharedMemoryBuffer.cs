using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ChakraCore.NET
{
    public class SharedMemoryBuffer : SafeBuffer
    {
        public SharedMemoryBuffer(int size):base(true)
        {
            var h=Marshal.AllocHGlobal(size);
            SetHandle(h);
            Initialize((ulong)size);
        }
        public SharedMemoryBuffer(IntPtr data,ulong size) : base(false)
        {
            SetHandle(data);
            Initialize(size);
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }

        public override bool Equals(object obj)
        {
            
            if (obj is SharedMemoryBuffer)
            {
                return (obj as SharedMemoryBuffer == this);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (SharedMemoryBuffer lhs, SharedMemoryBuffer rhs)
        {
            return lhs.handle == rhs.handle && lhs.ByteLength == rhs.ByteLength;
        }

        public static bool operator !=(SharedMemoryBuffer lhs, SharedMemoryBuffer rhs)
        {
            return !(lhs == rhs);
        }
    }
}
