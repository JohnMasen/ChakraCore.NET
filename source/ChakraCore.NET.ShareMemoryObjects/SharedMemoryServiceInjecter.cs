
using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
using ChakraCore.NET.SharedMemory;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public static class SharedMemoryServiceInjecter
    {
        public static void InjectShareMemoryObjects(this IServiceNode target)
        {
            //create/release all these objects inside the runtime internal context, this make sure all objects can be destroyed even user context is released
            var converter = target.GetService<IJSValueConverterService>();
            //var switchService = target.ServiceNode.GetService<IRuntimeService>().InternalContextSwitchService;
            if (converter.CanConvert<JSArrayBuffer>())
            {
                return;
            }
            
            converter.RegisterConverter<JSArrayBuffer>(
                (node, value) =>
                {
                    var jsValueService = node.GetService<IJSValueService>();
                    return jsValueService.CreateArrayBuffer(value);
                },
                (node, value) =>
                {
                    var switchService = node.GetService<IRuntimeService>().InternalContextSwitchService;
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
                    var jsValueService = node.GetService<IJSValueService>();
                    var convertService = node.GetService<IJSValueConverterService>();
                    JavaScriptValue? arryBuffer=null;
                    if (value.ArrayBuffer!=null)
                    {
                        arryBuffer = convertService.ToJSValue<JSArrayBuffer>(value.ArrayBuffer);
                    }
                    return jsValueService.CreateTypedArray(value,arryBuffer);
                },
                (node, value) =>
                {
                    var switchService = node.GetService<IRuntimeService>().InternalContextSwitchService;
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
                    var jsValueService = node.GetService<IJSValueService>();
                    var convertService = node.GetService <IJSValueConverterService>();
                    JavaScriptValue arrayBuffer = convertService.ToJSValue<JSArrayBuffer>(value.ArrayBuffer);
                    return jsValueService.CreateDataView(arrayBuffer, value);
                },
                (node, value) =>
                {
                    var switchService = node.GetService<IRuntimeService>().InternalContextSwitchService;
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
