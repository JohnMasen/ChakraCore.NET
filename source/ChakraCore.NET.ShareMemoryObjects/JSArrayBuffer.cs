using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using System.Runtime.InteropServices;
using System.IO;
using ChakraCore.NET;

namespace ChakraCore.NET.SharedMemory
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

        
    /// <summary>
    /// Create a ArrayBuffer and assign memory inside dotnet caller, the memory will be released when the JSArrayBuffer is disposed
    /// </summary>
    /// <param name="size">arraybuffer size in bytes</param>
    /// <returns>JSArrayBuffer</returns>
        public static JSArrayBuffer Create(long size)
        {
            var result = new JSArrayBuffer(SharedBufferSourceEnum.CreateByDotnet,(ulong)size);
            result.InitNew();
            return result;
        }

        internal static JSArrayBuffer CreateFromJS(IntPtr handle,uint size,JavaScriptValue value,IContextSwitchService context)
        {
            var result = new JSArrayBuffer(SharedBufferSourceEnum.CreateByJavascript,(ulong)size);
            result.SetJSSource(value, context);
            result.InitWindow(handle,  false);//memory owned by js, do not release when object is disposed
            return result;
        }

        /// <summary>
        /// Create a ArrayBuffer from a pointer to an pre-allocated memory cache, the memory will not be released when the JSArrayBuffer is disposed
        /// </summary>
        /// <param name="handle">pointer to memory cache</param>
        /// <param name="size">arraybuffer size in bytes</param>
        /// <returns></returns>
        public static JSArrayBuffer CreateFromExternal(IntPtr handle,ulong size)
        {
            var result = new JSArrayBuffer(SharedBufferSourceEnum.CreateByExternal,size);
            result.InitWindow(handle,  false);
            return result;
        }

        /// <summary>
        /// Create a JSArrayBuffer and let javascript allocate memory for it. Read/Write to the JSArrayBuffer is not available until the object is passed to a javascript context
        /// </summary>
        /// <param name="size">arraybuffer size in bytes</param>
        /// <param name="init">Action to setup initial value when the JSArrayBuffer is initialized in the javascript context</param>
        /// <returns>JSArrayBuffer</returns>
        public static JSArrayBuffer CreateInJavascript(uint size,Action<SharedMemoryBuffer> init)
        {
            var result = new JSArrayBuffer(SharedBufferSourceEnum.CreateInJavascript, (ulong)size);
            if (init!=null)
            {
                result.SetupInitValueAction(init);
            }
            return result;
        }




    }
}
