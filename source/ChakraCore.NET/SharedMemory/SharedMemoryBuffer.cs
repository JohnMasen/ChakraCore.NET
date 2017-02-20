using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ChakraCore.NET
{
    public class SharedMemoryBuffer : SafeBuffer
    {
        internal IntPtr Handle { get; set; }
        public SharedMemoryBuffer(IntPtr size):base(true)
        {
            var h=Marshal.AllocHGlobal(size);
            Handle = h;
            SetHandle(h);
            Initialize((ulong)size);
            System.Diagnostics.Debug.WriteLine($"Buffer created, ownsHandle=True,Address={h}");
        }
        public SharedMemoryBuffer(IntPtr data,ulong size,bool ownsHandle) : base(ownsHandle)
        {
            handle = data;
            SetHandle(data);
            Initialize(size);
            System.Diagnostics.Debug.WriteLine($"Buffer created, ownsHandle={ownsHandle},Address={data}");
        }


        public SharedMemoryBuffer CreateView(long position, ulong numBytes)
        {
            if (position < 0 || numBytes < 0)
            {
                throw new ArgumentOutOfRangeException("position or length cannot be negtive value");
            }
            long BufferStart = Handle.ToInt64();
            long BufferEnd = BufferStart + (long)ByteLength;

            long start = position;
            long end = start + (long)numBytes;//potencial overflow, let dotnet handle this :)

            if (start > BufferEnd || end > BufferEnd)
            {
                throw new ArgumentException("cannot project data accessor outside data area");
            }
            SharedMemoryBuffer result = new SharedMemoryBuffer((IntPtr)start, numBytes, false);
            return result;
        }
        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            System.Diagnostics.Debug.WriteLine("Buffer released");
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
