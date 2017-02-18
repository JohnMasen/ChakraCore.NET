using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using System.Runtime.InteropServices;
using System.IO;

namespace ChakraCore.NET
{
    public class JSArrayBuffer: SharedMemoryBuffer
    {
        public JavaScriptValue JSSource { get; private set; }
        public IntPtr Handle => handle;
        internal Action<SharedMemoryBuffer> Init { get; private set; }
        public ArrayBufferSourceEnum Source { get; private set; }
        private JSArrayBuffer(IntPtr handle, ulong size, ArrayBufferSourceEnum source) :base(handle,size)
        {
            Source = source;

        }

        private JSArrayBuffer(int size,Action<SharedMemoryBuffer> init, ArrayBufferSourceEnum source) : base(size)
        {
            Source = source;
            Init = init;
        }


        public static JSArrayBuffer CreateFromExternal(IntPtr handle,ulong size)
        {
            return new JSArrayBuffer(handle, size,ArrayBufferSourceEnum.CreateByExternal);
        }

        internal static JSArrayBuffer CreateFromJavascript(IntPtr handle, ulong size,JavaScriptValue jsSource)
        {
            return new JSArrayBuffer(handle, size, ArrayBufferSourceEnum.CreateByJavascript) { JSSource = jsSource };
        }

        public static JSArrayBuffer Create(int size)
        {
            return new JSArrayBuffer(size,null,ArrayBufferSourceEnum.CreateByDotnet);
        }
        public static JSArrayBuffer CreateInJavascript(int size, Action<SharedMemoryBuffer> init)
        {
            return new JSArrayBuffer(size, init, ArrayBufferSourceEnum.CreateInJavascript);
        }




    }
}
