using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Extension.SharedMemory
{
    public class SharedMemoryValueConvertHelper
    {
        public static void Inject(ChakraContext target)
        {
            target.ValueConverter.RegisterConverter<JSArrayBuffer>(
                (context, value) =>
                {
                    return ContextHelper.CreateArrayBuffer(context.RuntimeContext,value);
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<JSArrayBuffer>(() =>
                    {
                        if (value.ValueType != JavaScriptValueType.ArrayBuffer)
                        {
                            throw new InvalidOperationException("source type should be ArrayBuffer");
                        }
                        IntPtr buffer = JavaScriptValue.GetArrayBufferStorage(value, out uint size);
                        var result = JSArrayBuffer.CreateFromJS(buffer, size, value, context.RuntimeContext);
                        return result;
                    });

                }
                );
            target.ValueConverter.RegisterConverter<JSTypedArray>(
                (context, value) =>
                {
                    return ContextHelper.CreateTypedArray(context.RuntimeContext,value);
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<JSTypedArray>(() =>
                    {
                        if (value.ValueType != JavaScriptValueType.TypedArray)
                        {
                            throw new InvalidOperationException("source type should be TypedArray");
                        }
                        JavaScriptValue.GetTypedArrayStorage(value, out IntPtr data, out uint bufferLength, out JavaScriptTypedArrayType type, out int elementSize);
                        var result = JSTypedArray.CreateFromJS(type, data, bufferLength, value, context.RuntimeContext);
                        return result;
                    });

                }
                );
            target.ValueConverter.RegisterConverter<JSDataView>(
                (context, value) =>
                {
                    JavaScriptValue arrayBuffer = context.RuntimeContext.ValueConverter.ToJSValue<JSArrayBuffer>(context, value.ArrayBuffer);
                    return ContextHelper.CreateDataView(context.RuntimeContext, arrayBuffer, value);
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<JSDataView>(() =>
                    {
                        if (value.ValueType != JavaScriptValueType.DataView)
                        {
                            throw new InvalidOperationException("source type should be DataView");
                        }
                        JavaScriptValue.GetDataViewStorage(value, out IntPtr data, out uint bufferLength);
                        var result = JSDataView.CreateFromJS(value, data, bufferLength, context.RuntimeContext);
                        return result;
                    });

                }
                );
        }
    }
}
