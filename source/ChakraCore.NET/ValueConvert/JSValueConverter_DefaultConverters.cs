using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public partial class JSValueConverter
    {
        partial void initDefault()
        {
            RegisterConverter<string>(
                (context, value) =>
                {
                    return context.RuntimeContext.With<JavaScriptValue>(() =>
                    {
                        return JavaScriptValue.FromString(value);
                    });
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<string>(() =>
                    {
                        return value.ConvertToString().ToString();
                    });
                }
                );
            RegisterArrayConverter<string>();

            RegisterStructConverter<int>(
                (context, value) =>
                {
                    return context.RuntimeContext.With<JavaScriptValue>(() =>
                    {
                        return JavaScriptValue.FromInt32(value);
                    });
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<int>(() =>
                    {
                        return value.ConvertToNumber().ToInt32();
                    });
                }
                );
            RegisterArrayConverter<int>();

            RegisterStructConverter<double>(
                (context, value) =>
                {
                    return context.RuntimeContext.With<JavaScriptValue>(() =>
                    {
                        return JavaScriptValue.FromDouble(value);
                    });
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<double>(() =>
                    {
                        return value.ConvertToNumber().ToDouble();
                    });
                }
                );
            RegisterArrayConverter<double>();

            RegisterStructConverter<bool>(
                (context, value) =>
                {
                    return context.RuntimeContext.With<JavaScriptValue>(() =>
                    {
                        return JavaScriptValue.FromBoolean(value);
                    });
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<bool>(() =>
                    {
                        return value.ConvertToBoolean().ToBoolean();
                    });
                }
                );
            RegisterArrayConverter<bool>();

            RegisterStructConverter<Single>(
                (context, value) =>
                {
                    return context.RuntimeContext.With<JavaScriptValue>(() =>
                    {
                        return JavaScriptValue.FromDouble(value);
                    });
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<Single>(() =>
                    {
                        return Convert.ToSingle(value.ConvertToNumber().ToDouble());
                    });
                }
                );
            RegisterArrayConverter<Single>();

            RegisterStructConverter<byte>(
                (context, value) =>
                {
                    return context.RuntimeContext.With<JavaScriptValue>(() =>
                    {
                        return JavaScriptValue.FromDouble(Convert.ToDouble(value));
                    });
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<byte>(() =>
                    {
                        return BitConverter.GetBytes(value.ToInt32())[0];
                    });
                }
                );
            RegisterArrayConverter<byte>();

            RegisterStructConverter<decimal>(
                (context, value) =>
                {
                    return context.RuntimeContext.With<JavaScriptValue>(() =>
                    {
                        return JavaScriptValue.FromDouble(Convert.ToDouble(value));
                    });
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<decimal>(() =>
                    {
                        return Convert.ToDecimal(value.ConvertToNumber().ToDouble());
                    });
                }
                );
            RegisterArrayConverter<decimal>();



            #region Special Convert
            RegisterStructConverter<JavaScriptValue>(
                (context, value) =>
                {
                    return value;
                },
                (context, value) =>
                {
                    return value;
                }
                );
            RegisterConverter<JSArrayBuffer>(
                (context, value) =>
                {
                    return context.RuntimeContext.CreateArrayBuffer(value);
                },
                (context, value) =>
                {
                    return context.RuntimeContext.With<JSArrayBuffer>(() =>
                    {
                        if (value.ValueType!=JavaScriptValueType.ArrayBuffer)
                        {
                            throw new InvalidOperationException("source type should be ArrayBuffer");
                        }
                        IntPtr buffer=JavaScriptValue.GetArrayBufferStorage(value, out uint size);
                        var result = JSArrayBuffer.CreateFromJS(buffer, size, value, context.RuntimeContext);
                        return result;
                    });
                    
                }
                );
            RegisterConverter<JSTypedArray>(
                (context, value) =>
                {
                    return context.RuntimeContext.CreateTypedArray(value);
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
            RegisterConverter<JSDataView>(
                (context, value) =>
                {
                    JavaScriptValue arrayBuffer = ToJSValue<JSArrayBuffer>(context,value.ArrayBuffer);
                    return context.RuntimeContext.CreateDataView(arrayBuffer,value);
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
            #endregion

        }
    }
}
