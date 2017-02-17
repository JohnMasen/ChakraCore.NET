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
            RegisterConverter<JSExternalArrayBuffer>(
                (context, value) =>
                {
                    return context.RuntimeContext.CreateExternalArrayBuffer(value);
                },
                (context, value) =>
                {
                    throw new NotSupportedException("cannot retrieve ExternalArrayBuffer from javascript runtime");
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
                        var buffer=JavaScriptValue.GetArrayBufferStorage(value, out uint bufferSize);
                        JSArrayBuffer result = new JSArrayBuffer(buffer, bufferSize);
                        return result;
                    });
                    
                }
                );

            #endregion

        }
    }
}
