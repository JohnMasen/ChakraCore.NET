using ChakraCore.NET;
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.SharedMemory
{
    public class JSTypedArray : JSSharedMemoryObject
    {
        public uint Position { get; private set; }
        public JavaScriptTypedArrayType ArrayType { get; private set; }

        public uint UnitSize { get; private set; }

        public uint UnitCount { get; private set; }


        private JSTypedArray(JavaScriptTypedArrayType type,JSArrayBuffer source,uint position, uint unitCount) : base(SharedBufferSourceEnum.CreateByDotnet, unitCount*GetUnitByteSizeByArrayType(type))
        {
            ArrayType = type;
            Position = position;
            UnitSize = GetUnitByteSizeByArrayType(type);
            UnitCount = unitCount;
        }

        private JSTypedArray(JavaScriptTypedArrayType type, uint position, uint unitCount):base(SharedBufferSourceEnum.CreateByJavascript, unitCount * GetUnitByteSizeByArrayType(type))
        {
            ArrayType = type;
            Position = position;
            UnitSize = GetUnitByteSizeByArrayType(type);
            UnitCount = unitCount;
        }

        private JSTypedArray(JavaScriptTypedArrayType type, uint position, uint unitCount, Action<SharedMemoryBuffer>init ):base(SharedBufferSourceEnum.CreateInJavascript, unitCount * GetUnitByteSizeByArrayType(type))
        {
            ArrayType = type;
            Position = position;
            UnitSize = GetUnitByteSizeByArrayType(type);
            UnitCount = unitCount;
        }

        public static JSTypedArray CreateFromArrayBuffer(JavaScriptTypedArrayType type, JSArrayBuffer source,uint position, uint unitCount)
        {
            JSTypedArray result = new JSTypedArray(type,source, position, unitCount);
            result.InitWindow(source.Buffer, position);
            return result;
        }

        internal static JSTypedArray CreateFromJS(JavaScriptTypedArrayType type, IntPtr data, uint unitCount, JavaScriptValue source,IContextSwitchService context)
        {
            JSTypedArray result = new JSTypedArray(type,0, unitCount);
            result.SetJSSource(source, context);
            result.InitWindow(data, false);
            return result;
        }

        public static JSTypedArray CreateInJS(JavaScriptTypedArrayType type,uint unitCount, Action<SharedMemoryBuffer> init)
        {
            var result= new JSTypedArray(type, 0, unitCount, init);
            result.SetupInitValueAction(init);
            return result;
        }

        public static uint GetUnitByteSizeByArrayType(JavaScriptTypedArrayType type)
        {
            switch (type)
            {
                case JavaScriptTypedArrayType.Int8:
                case JavaScriptTypedArrayType.Uint8:
                case JavaScriptTypedArrayType.Uint8Clamped:
                    return 1;
                case JavaScriptTypedArrayType.Int16:
                case JavaScriptTypedArrayType.Uint16:
                    return 2;
                case JavaScriptTypedArrayType.Int32:
                case JavaScriptTypedArrayType.Uint32:
                case JavaScriptTypedArrayType.Float32:
                    return 4;
                case JavaScriptTypedArrayType.Float64:
                    return 8;
                default:
                    throw new ArgumentException("not a valid TypedArrayType", nameof(ArrayType));
            }
        }
    }
}
