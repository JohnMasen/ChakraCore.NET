using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.SharedMemory
{
    public class JSDataView : JSSharedMemoryObject
    {
        public uint Position { get; private set; }
        public JSArrayBuffer ArrayBuffer { get; private set; }
        public JSDataView(SharedBufferSourceEnum source,JSArrayBuffer arrayBuffer, uint position, uint size) : base(source, size)
        {
            Position = position;
            ArrayBuffer = arrayBuffer;
        }


        public static JSDataView CreateFromArrayBuffer(JSArrayBuffer source, uint position, uint size,Action<SharedMemoryBuffer> init)
        {
            JSDataView result;
            if (source.Buffer==null)
            {
                result = new JSDataView(SharedBufferSourceEnum.CreateInJavascript, source, position, size);
                result.SetupInitValueAction(init);
            }
            else
            {
                result = new JSDataView(SharedBufferSourceEnum.CreateByDotnet, source, position, size);
                result.InitWindow(source.Buffer, position);
            }
            return result;
        }

        internal static JSDataView CreateFromJS(JavaScriptValue value,IntPtr handle,uint size,ChakraContext context)
        {
            JSDataView result = new JSDataView(SharedBufferSourceEnum.CreateByJavascript, null,0, size);
            result.SetJSSource(value,context);
            result.InitWindow(handle, false);
            return result;
        }
    }
}
