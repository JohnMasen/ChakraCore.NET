using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using System.Runtime.InteropServices;

namespace ChakraCore.NET
{
    public class JSArrayBuffer
    {
        public uint Size { get; private set; }
        public bool IsCreatedByJS { get; private set; }
        public byte[] Buffer { get; private set; }
        internal Action<byte[]> initAction;

        public JSArrayBuffer(uint size ,Action<byte[]> init=null)
        {
            IsCreatedByJS = false;
            Size = size;
            initAction = init;
        }

        internal JSArrayBuffer(byte[] data,uint size)
        {
            if (size!=data.Length)
            {
                throw new InvalidOperationException("buffer size does not match retreived data size");
            }
            IsCreatedByJS = true;
            Buffer = data;
            Size = size;
        }



    }
}
