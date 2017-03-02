using ChakraCore.NET;
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.SharedMemory
{
    public static class JSValueServiceHelper
    {
        public static JavaScriptValue CreateArrayBuffer(this IJSValueService valueService, JSArrayBuffer source)
        {
            IContextSwitchService switchService = valueService.CurrentNode.GetService<IRuntimeService>().InternalContextSwitchService;
            return switchService.With<JavaScriptValue>(() =>
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
                            source.SetJSSource(result, switchService);//hold the varient
                            var data = JavaScriptValue.GetArrayBufferStorage(result, out uint bufferSize);
                            source.InitWindow(data, false);
                            source.InitBeforeConvert(source.Buffer);
                            return result;
                        }
                    case SharedBufferSourceEnum.CreateByDotnet:
                    case SharedBufferSourceEnum.CreateByExternal:
                        {
                            var result = JavaScriptValue.CreateExternalArrayBuffer(source.Buffer.Handle, (uint)source.Buffer.ByteLength, null, IntPtr.Zero);//do not handle GC callback, user should control the varient life cycle
                            source.SetJSSource(result, switchService);//hold the varient
                            return result;
                        }

                    default:
                        throw new ArgumentOutOfRangeException("Invalid BufferSource property in JSArryBuffer object");
                }
            }
                );
        }



        public static JavaScriptValue CreateTypedArray(this IJSValueService valueService, JSTypedArray source,JavaScriptValue? arrayBufferSource)
        {
            IContextSwitchService switchService = valueService.CurrentNode.GetService<IRuntimeService>().InternalContextSwitchService;
            if (source.JSSource.IsValid)
            {
                return source.JSSource;
            }
            switch (source.BufferSource)
            {
                case SharedBufferSourceEnum.CreateByDotnet:
                    return switchService.With<JavaScriptValue>(
                        () =>
                        {
                            if (arrayBufferSource==null)
                            {
                                throw new ArgumentNullException(nameof(arrayBufferSource));
                            }
                            var result = JavaScriptValue.CreateTypedArray(source.ArrayType, arrayBufferSource.Value, source.Position, source.UnitCount);
                            source.SetJSSource(result, switchService);
                            return result;
                        }
                        );
                case SharedBufferSourceEnum.CreateByJavascript:
                    throw new InvalidOperationException("invalid source typed array");//create by javascript should already have JavaScriptValue assigned
                case SharedBufferSourceEnum.CreateInJavascript:
                    return switchService.With<JavaScriptValue>(
                        () =>
                        {
                            var result = JavaScriptValue.CreateTypedArray(source.ArrayType, JavaScriptValue.Invalid, source.Position, source.UnitCount);
                            source.SetJSSource(result, switchService);//hold the objec
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
        public static JavaScriptValue CreateDataView(this IJSValueService valueService, JavaScriptValue sourceArrayBuffer, JSDataView source)
        {
            IContextSwitchService switchService = valueService.CurrentNode.GetService<IRuntimeService>().InternalContextSwitchService;
            if (source.JSSource.IsValid)
            {
                return source.JSSource;
            }
            switch (source.BufferSource)
            {
                case SharedBufferSourceEnum.CreateByDotnet:
                    return switchService.With<JavaScriptValue>(
                        () =>
                        {
                            var result = JavaScriptValue.CreateDataView(sourceArrayBuffer, source.Position, (uint)source.Size);
                            source.SetJSSource(result, switchService);
                            return result;
                        }
                        );
                case SharedBufferSourceEnum.CreateByJavascript:
                    throw new InvalidOperationException("invalid source typed array");//create by javascript should already have JavaScriptValue assigned
                case SharedBufferSourceEnum.CreateInJavascript:
                    return switchService.With<JavaScriptValue>(
                        () =>
                        {
                            var result = JavaScriptValue.CreateDataView(sourceArrayBuffer, source.Position, (uint)source.Size);
                            source.SetJSSource(result, switchService);
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
