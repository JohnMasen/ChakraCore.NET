using Chakra.NET.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chakra.NET
{
    public partial class JSValueConverter
    {
        ChakraContext context;
        //public delegate TResult toJSValueDelegate<out TResult>(ChakraContext context,JavaScriptValue value,JavaScriptValue owner);
        //public delegate TResult fromJSValueDelegate<out TResult>(ChakraContext context, JavaScriptValue value, JavaScriptValue owner);
        public JSValueConverter(ChakraContext context)
        {
            this.context = context;
            createDefaultConveters();
        }

        private void createDefaultConveters()
        {

            RegisterConverter<float>(
                    (value, helper) =>
                    {
                        using (helper.With())
                        {
                            return JavaScriptValue.FromDouble(Convert.ToDouble(value));
                        }

                    }
                    ,
                    (value, helper) =>
                    {
                        using (helper.With())
                        {
                            return Convert.ToSingle(value.ToDouble());
                        }

                    }
                    );
            RegisterConverter<double>(
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return JavaScriptValue.FromDouble(value);
                    }

                }
                ,
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return value.ToDouble();
                    }

                }
                );
            RegisterConverter<int>(
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return JavaScriptValue.FromInt32(value);
                    }

                }
                ,
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return value.ToInt32();
                    }

                }
                );
            RegisterConverter<byte>(
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return JavaScriptValue.FromDouble(Convert.ToInt32(value));
                    }

                }
                ,
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return BitConverter.GetBytes(value.ToInt32())[0];
                    }

                }
                );
            RegisterConverter<string>(
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return JavaScriptValue.FromString(value);
                    }

                }
                ,
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return value.ToString();
                    }

                }
                );
            RegisterConverter<bool>(
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return JavaScriptValue.FromBoolean(value);
                    }

                }
                ,
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return value.ToBoolean();
                    }

                }
                );
            RegisterConverter<Action>(
                (value, helper) =>
                {

                    return context.CallContext.StackInfo.Holder.CreateCallback(helper, (JavaScriptValue callee, bool isConstructCall, JavaScriptValue[] arguments,
       ushort argumentCount, IntPtr callbackData) =>
                     {
                         value();
                         return JavaScriptValue.Undefined;
                     });
                }
                ,
                (value, helper) =>
                {
                    return new Action(() =>
                    {
                        using (context.With())
                        {
                            value.CallFunction(JavaScriptValue.Undefined);//first parameer should always be undefined, why???
                        }
                    }
                    );
                }
                );
            //a useful trick
            RegisterConverter<JavaScriptValue>(
                (value, helper) => { return value; },
                (value, helper) => { return value; }
                );

        }

        private Dictionary<Type, Tuple<object, object>> converters = new Dictionary<Type, Tuple<object, object>>();

        public void RegisterConverter<T>(Func<T, ChakraContext, JavaScriptValue> toJSValue, Func<JavaScriptValue, ChakraContext, T> fromJSValue, bool throewIfExists = true)
        {
            if (converters.ContainsKey(typeof(T)))
            {
                if (throewIfExists)
                {
                    throw new ArgumentException($"type {typeof(T).FullName} already registered");
                }
                else
                {
                    return;
                }
            }
            converters.Add(typeof(T), new Tuple<object, object>(toJSValue, fromJSValue));
        }

        //public void RegisterProxyConverter<T>(Action<JSValueWithContext> initMeta)
        //{
        //    RegisterConverter<T>(
        //        (value,helper)=>
        //        {
        //            using (helper.With(value))
        //            {
        //                JavaScriptValue result=context.CreateProxyObject<>
        //            }
        //        }

        //        );
        //}

        public JavaScriptValue ToJSValue<T>(T value)
        {
            if (converters.ContainsKey(typeof(T)))
            {
                var f = (converters[typeof(T)].Item1 as Func<T, ChakraContext, JavaScriptValue>);
                if (f == null)
                {
                    throw new NotImplementedException($"type {typeof(T).FullName} does not support convert to JSValue");
                }
                else
                {
                    return f(value, context);

                }

            }
            else
            {
                throw new NotImplementedException($"type {typeof(T).FullName} not registered for convertion");
            }
        }

        public T FromJSValue<T>(JavaScriptValue value)
        {
            if (converters.ContainsKey(typeof(T)))
            {
                var f = (converters[typeof(T)].Item2 as Func<JavaScriptValue, ChakraContext, T>);
                if (f == null)
                {
                    throw new NotImplementedException($"type {typeof(T).FullName} does not support convert from JSValue");
                }
                else
                {
                    return f(value, context);
                }
            }
            else
            {
                throw new NotImplementedException($"type {typeof(T).FullName} not registered for convertion");
            }
        }

        public JavaScriptValue ToJSArray<T>(IEnumerable<T> source)
        {

            var result = JavaScriptValue.CreateArray(Convert.ToUInt32(source.Count()));
            int index = 0;
            foreach (var item in source)
            {
                result.SetIndexedProperty(ToJSValue<int>(index++), ToJSValue<T>(item));
            }
            return result;
        }

        public IEnumerable<T> FromJSArray<T>(JavaScriptValue value)
        {
            int length = FromJSValue<int>(value.GetProperty("length"));
            for (int i = 0; i < length; i++)
            {
                yield return FromJSValue<T>(value.GetIndexedProperty(ToJSValue<int>(i)));
            }
        }

        public JSValueConverter ReigsterArrayConverter<T>()
        {
            RegisterConverter<IEnumerable<T>>(
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return helper.ValueConverter.ToJSArray<T>(value);
                    }

                },
                (value, helper) =>
                {
                    using (helper.With())
                    {
                        return helper.ValueConverter.FromJSArray<T>(value);
                    }

                }
                , false);
            return this;
        }


    }
}
