
using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
using ChakraCore.NET.SharedMemory;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public static class SharedMemoryRuntimeInjecter
    {
        public static void InjectShareMemoryObjects(this ChakraRuntime target)
        {
            //create/release all these objects inside the runtime internal context, this make sure all objects can be destroyed even user context is released
            var converter=target.ServiceNode.GetService<IJSValueConverterService>();
            var switchService = target.ServiceNode.GetService<IRuntimeService>().InternalContextSwitchService;
            var jsValueService = target.ServiceNode.GetService<IJSValueService>();
            
            converter.RegisterConverter<JSArrayBuffer>(
                (node, value) =>
                {
                    return jsValueService.CreateArrayBuffer(value);
                },
                (node, value) =>
                {
                    return switchService.With<JSArrayBuffer>(() =>
                    {
                        if (value.ValueType != JavaScriptValueType.ArrayBuffer)
                        {
                            throw new InvalidOperationException("source type should be ArrayBuffer");
                        }
                        IntPtr buffer = JavaScriptValue.GetArrayBufferStorage(value, out uint size);
                        var result = JSArrayBuffer.CreateFromJS(buffer, size, value, switchService);
                        return result;
                    });

                }, false
                );
            converter.RegisterConverter<JSTypedArray>(
                (node, value) =>
                {
                    return jsValueService.CreateTypedArray(value);
                },
                (node, value) =>
                {
                    return switchService.With<JSTypedArray>(() =>
                    {
                        if (value.ValueType != JavaScriptValueType.TypedArray)
                        {
                            throw new InvalidOperationException("source type should be TypedArray");
                        }
                        JavaScriptValue.GetTypedArrayStorage(value, out IntPtr data, out uint bufferLength, out JavaScriptTypedArrayType type, out int elementSize);
                        var result = JSTypedArray.CreateFromJS(type, data, bufferLength, value, switchService);
                        return result;
                    });

                }, false
                );
            converter.RegisterConverter<JSDataView>(
                (node, value) =>
                {
                    JavaScriptValue arrayBuffer = converter.ToJSValue<JSArrayBuffer>(value.ArrayBuffer);
                    return jsValueService.CreateDataView(arrayBuffer, value);
                },
                (node, value) =>
                {
                    return switchService.With<JSDataView>(() =>
                    {
                        if (value.ValueType != JavaScriptValueType.DataView)
                        {
                            throw new InvalidOperationException("source type should be DataView");
                        }
                        JavaScriptValue.GetDataViewStorage(value, out IntPtr data, out uint bufferLength);
                        var result = JSDataView.CreateFromJS(value, data, bufferLength, switchService);
                        return result;
                    });

                }, false
                );
        }
    }
}
