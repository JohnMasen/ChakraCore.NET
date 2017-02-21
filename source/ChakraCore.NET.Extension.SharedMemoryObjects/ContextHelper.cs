using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Extension.SharedMemory
{
    class ContextHelper
    {
        public static JavaScriptValue CreateArrayBuffer(ChakraContext context, JSArrayBuffer source)
        {
            return context.With<JavaScriptValue>(() =>
            {
                if (source.JSSource.IsValid)
                {
                    return source.JSSource;
                }
                switch (source.BufferSource)
                {
                    case SharedBufferSourceEnum.CreateByJavascript:
                        throw new InvalidOperationException("invalid source array buffer");//create by javascript should already have JavaScriptValue assigned
                    case SharedBufferSourceEnum.CreateInJavascript:
                        {
                            JavaScriptValue result = JavaScriptValue.CreateArrayBuffer((uint)source.Size);
                            source.SetJSSource(result, context);//hold the varient
                            var data = JavaScriptValue.GetArrayBufferStorage(result, out uint bufferSize);
                            source.InitWindow(data, false);
                            source.InitBeforeConvert(source.Buffer);
                            return result;
                        }
                    case SharedBufferSourceEnum.CreateByDotnet:
                    case SharedBufferSourceEnum.CreateByExternal:
                        {
                            var result = JavaScriptValue.CreateExternalArrayBuffer(source.Buffer.Handle, (uint)source.Buffer.ByteLength, null, IntPtr.Zero);//do not handle GC callback, user should control the varient life cycle
                            source.SetJSSource(result, context);//hold the varient
                            return result;
                        }

                    default:
                        throw new ArgumentOutOfRangeException("Invalid BufferSource property in JSArryBuffer object");
                }
            }
                );
        }



        public static JavaScriptValue CreateTypedArray(ChakraContext context, JSTypedArray source)
        {
            if (source.JSSource.IsValid)
            {
                return source.JSSource;
            }
            switch (source.BufferSource)
            {
                case SharedBufferSourceEnum.CreateByDotnet:
                    return context.With<JavaScriptValue>(
                        () =>
                        {
                            var result = JavaScriptValue.CreateTypedArray(source.ArrayType, source.JSSource, source.Position, source.UnitCount);
                            source.SetJSSource(result, context);
                            return result;
                        }
                        );
                case SharedBufferSourceEnum.CreateByJavascript:
                    throw new InvalidOperationException("invalid source typed array");//create by javascript should already have JavaScriptValue assigned
                case SharedBufferSourceEnum.CreateInJavascript:
                    return context.With<JavaScriptValue>(
                        () =>
                        {
                            var result = JavaScriptValue.CreateTypedArray(source.ArrayType, JavaScriptValue.Invalid, source.Position, source.UnitCount);
                            source.SetJSSource(result, context);//hold the objec
                            //get the internal storage
                            JavaScriptValue.GetTypedArrayStorage(result, out IntPtr data, out uint bufferLength, out JavaScriptTypedArrayType type, out int elementSize);
                            source.InitWindow(data, false);
                            source.InitBeforeConvert(source.Buffer);
                            return result;
                        }
                        );
                case SharedBufferSourceEnum.CreateByExternal:
                    throw new ArgumentException("TypedArray does not support create from external");
                default:
                    throw new ArgumentOutOfRangeException("Invalid BufferSource property in JSTypedArray object");
            }
        }

        /// <summary>
        /// Create DataView in javascript
        /// this method requires a sourceArrayBuffer which make sure the arraybuffer is initialized before transfer to javascript
        /// </summary>
        /// <param name="sourceArrayBuffer">ArrayBuffer object</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static JavaScriptValue CreateDataView(ChakraContext context, JavaScriptValue sourceArrayBuffer, JSDataView source)
        {
            if (source.JSSource.IsValid)
            {
                return source.JSSource;
            }
            switch (source.BufferSource)
            {
                case SharedBufferSourceEnum.CreateByDotnet:
                    return context.With<JavaScriptValue>(
                        () =>
                        {
                            var result = JavaScriptValue.CreateDataView(sourceArrayBuffer, source.Position, (uint)source.Size);
                            source.SetJSSource(result, context);
                            return result;
                        }
                        );
                case SharedBufferSourceEnum.CreateByJavascript:
                    throw new InvalidOperationException("invalid source typed array");//create by javascript should already have JavaScriptValue assigned
                case SharedBufferSourceEnum.CreateInJavascript:
                    return context.With<JavaScriptValue>(
                        () =>
                        {
                            var result = JavaScriptValue.CreateDataView(sourceArrayBuffer, source.Position, (uint)source.Size);
                            source.SetJSSource(result, context);
                            JavaScriptValue.GetDataViewStorage(result, out IntPtr data, out uint bufferLength);
                            source.InitWindow(data, false);
                            source.InitBeforeConvert(source.Buffer);
                            return result;
                        }
                        );
                case SharedBufferSourceEnum.CreateByExternal:
                    throw new ArgumentException("DataView does not support create from external");
                default:
                    throw new ArgumentOutOfRangeException("Invalid BufferSource property in JSDataView object");
            }

        }

    }
}
