using System.Runtime.InteropServices;

using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using static ChakraCore.NET.API.JavaScriptContext;
namespace ChakraCore.NET
{
public partial class JSArrayBuffer
{
        public void ReadInt32(Int32[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(data, buffer, 0, memorySize);
        }

        public void WriteInt32(Int32[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(buffer, 0, data, memorySize);
        }



        public void ReadInt16(Int16[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(data, buffer, 0, memorySize);
        }

        public void WriteInt16(Int16[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(buffer, 0, data, memorySize);
        }



        public void ReadInt64(Int64[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(data, buffer, 0, memorySize);
        }

        public void WriteInt64(Int64[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(buffer, 0, data, memorySize);
        }



        public void ReadDouble(Double[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(data, buffer, 0, memorySize);
        }

        public void WriteDouble(Double[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(buffer, 0, data, memorySize);
        }



        public void ReadChar(Char[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(data, buffer, 0, memorySize);
        }

        public void WriteChar(Char[] buffer)
        {
            if (!checkSize(buffer, out int arraySize))
            {
                throw new ArgumentException($"buffer size is {arraySize} does not match defined size, expect {Size}");
            }
            Marshal.Copy(buffer, 0, data, memorySize);
        }




    }
}
