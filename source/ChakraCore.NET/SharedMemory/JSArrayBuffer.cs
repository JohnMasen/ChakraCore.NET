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

        private Action releaseValue;
        private Action<SharedMemoryBuffer> init;
        public ArrayBufferSourceEnum Source { get; private set; }
        private JSArrayBuffer(IntPtr handle, ulong size, ArrayBufferSourceEnum source) :base(handle,size)
        {
            Source = source;

        }

        internal void SetJSSource(JavaScriptValue value,ChakraContext context)
        {
            if (!value.IsValid)
            {
                throw new ArgumentException("not a valid JavaScriptValue", nameof(value));
                    
            }
            if (JSSource.IsValid)
            {
                throw new ArgumentException("cannot set source more than once");
            }
            else
            {
                value.AddRef();
                JSSource = value;
                releaseValue = () =>
                  {
                      context.With(() => value.Release());

                  };
            }
        }

        private JSArrayBuffer(int size,Action<SharedMemoryBuffer> init, ArrayBufferSourceEnum source) : base(size)
        {
            Source = source;
            this.init = init;
        }

        internal void Init(SharedMemoryBuffer buffer)
        {
            if (JSSource.IsValid)
            {
                return;
            }
            else
            {
                init(buffer);
            }
        }
        protected override bool ReleaseHandle()
        {
            releaseValue?.Invoke();
            return base.ReleaseHandle();
        }

        /// <summary>
        /// create JSArryBuffer from pre-allocated memory address
        /// </summary>
        /// <param name="handle">address pointer</param>
        /// <param name="size">data size in bytes</param>
        /// <returns></returns>
        public static JSArrayBuffer CreateFromExternal(IntPtr handle,ulong size)
        {
            return new JSArrayBuffer(handle, size,ArrayBufferSourceEnum.CreateByExternal);
        }


        /// <summary>
        /// Create JSArryBuffer from javascriptvalue, should be used as a parameter from javascript call only
        /// Set source should be called immediately after this call
        /// </summary>
        /// <param name="handle">address pointer</param>
        /// <param name="size">data size in bytes</param>
        /// <param name="jsSource">original javascriptvalue</param>
        /// <returns></returns>
        internal static JSArrayBuffer CreateFromJavascript(IntPtr handle, ulong size)
        {
            var result = new JSArrayBuffer(handle, size, ArrayBufferSourceEnum.CreateByJavascript);
            return result;
        }

        /// <summary>
        /// Create JSArryBuffer and allocate memory, memory will be released when the object is disposed
        /// </summary>
        /// <param name="size">size in bytes</param>
        /// <returns></returns>
        public static JSArrayBuffer Create(int size)
        {
            return new JSArrayBuffer(size,null,ArrayBufferSourceEnum.CreateByDotnet);
        }

        /// <summary>
        /// Create a new JSArryBuffer inside javascript and set it's initial value. 
        /// useful for passing a ArryBuffer as parameter
        /// </summary>
        /// <param name="size">size in bytes</param>
        /// <param name="init">delegate for setting up initial value</param>
        /// <returns></returns>
        public static JSArrayBuffer CreateInJavascript(int size, Action<SharedMemoryBuffer> init)
        {
            return new JSArrayBuffer(size, init, ArrayBufferSourceEnum.CreateInJavascript);
        }




    }
}
