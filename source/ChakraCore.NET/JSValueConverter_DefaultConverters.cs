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
        }
    }
}
