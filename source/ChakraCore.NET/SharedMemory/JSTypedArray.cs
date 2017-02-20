using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public class JSTypedArray : JSSharedMemoryObject
    {
        public uint Position { get; private set; }
        public JavaScriptTypedArrayType ArrayType { get; private set; }

        public uint ElementLength
        {
            get
            {
                switch (ArrayType)
                {
                    case JavaScriptTypedArrayType.Int8:
                    case JavaScriptTypedArrayType.Uint8:
                    case JavaScriptTypedArrayType.Uint8Clamped:
                        return 8;
                    case JavaScriptTypedArrayType.Int16:
                    case JavaScriptTypedArrayType.Uint16:
                        return 16;
                    case JavaScriptTypedArrayType.Int32:
                    case JavaScriptTypedArrayType.Uint32:
                    case JavaScriptTypedArrayType.Float32:
                        return 32;
                    case JavaScriptTypedArrayType.Float64:
                        return 64;
                    default:
                        throw new ArgumentException("not a valid TypedArrayType", nameof(ArrayType));
                }
            }
                
                
        }


        private JSTypedArray(JavaScriptTypedArrayType type,JSArrayBuffer source,uint position, uint size) : base(SharedBufferSourceEnum.CreateByDotnet, size)
        {
            ArrayType = type;
            Position = position;
        }

        private JSTypedArray(JavaScriptTypedArrayType type, uint position, uint size):base(SharedBufferSourceEnum.CreateByJavascript,size)
        {
            ArrayType = type;
            Position = position;
        }

        private JSTypedArray(JavaScriptTypedArrayType type, uint position, uint size, Action<SharedMemoryBuffer>init ):base(SharedBufferSourceEnum.CreateInJavascript,size)
        {
            ArrayType = type;
            Position = position;
        }

        public static JSTypedArray CreateFromArrayBuffer(JavaScriptTypedArrayType type, JSArrayBuffer source,uint position, uint size)
        {
            JSTypedArray result = new JSTypedArray(type,source, position, size);
            result.InitWindow(source.Buffer, position);
            return result;
        }

        internal static JSTypedArray CreateFromJS(JavaScriptTypedArrayType type, IntPtr data, uint size,JavaScriptValue source,ChakraContext context)
        {
            JSTypedArray result = new JSTypedArray(type,0,size);
            result.InitWindow(data, false);
            result.SetJSSource(source,context);
            return result;
        }

        public static JSTypedArray CreateInJS(JavaScriptTypedArrayType type,uint size,Action<SharedMemoryBuffer> init)
        {
            return new JSTypedArray(type, 0, size, init) { InitValue = init };
        }
    }
}
