using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using System.Runtime.InteropServices;
using System.IO;

namespace ChakraCore.NET
{
    public class JSArrayBuffer:JSSharedMemoryObject
    {

        private JSArrayBuffer(SharedBufferSourceEnum source,ulong size):base(source,size)
        {
            if (source==SharedBufferSourceEnum.CreateInJavascript && size>uint.MaxValue)
            {
                throw new ArgumentOutOfRangeException("create in javascript cannot have size large than uint.MaxValue");
            }
            System.Diagnostics.Debug.WriteLine("JSArryBufferCreated");
        }


        //internal void InitBuffer(IntPtr handle,bool ownsHandle)
        //{
        //    InitWindow(handle, ownsHandle);
        //}

        //internal void InitBuffer()
        //{
        //    InitNew();
        //}

        

        public static JSArrayBuffer Create(long size)
        {
            var result = new JSArrayBuffer(SharedBufferSourceEnum.CreateByDotnet,(ulong)size);
            result.InitNew();
            return result;
        }

        internal static JSArrayBuffer CreateFromJS(IntPtr handle,uint size,JavaScriptValue value,ChakraContext context)
        {
            var result = new JSArrayBuffer(SharedBufferSourceEnum.CreateByJavascript,(ulong)size);
            result.InitWindow(handle,  false);//memory owned by js, do not release when object is disposed
            result.SetJSSource(value,context);
            return result;
        }

        public static JSArrayBuffer CreateFromExternal(IntPtr handle,ulong size)
        {
            var result = new JSArrayBuffer(SharedBufferSourceEnum.CreateByExternal,size);
            result.InitWindow(handle,  false);
            return result;
        }

        public static JSArrayBuffer CreateInJavascript(uint size,Action<SharedMemoryBuffer> init)
        {
            return new JSArrayBuffer(SharedBufferSourceEnum.CreateInJavascript,(ulong)size) { InitValue = init };
            //do not init the buffer, buffer will be initialized when it's passed to javascript
        }




    }
}
