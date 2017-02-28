using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;


namespace ChakraCore.NET
{
    public partial class JSValueConverterService :ServiceBase, IJSValueConverterService
    {
        private SortedDictionary<Type, Tuple<object, object>> converters = new SortedDictionary<Type, Tuple<object, object>>(TypeComparer.Instance);
        public JSValueConverterService()
        {
            initDefault();
        }

        public bool CanConvert<T>()
        {
            return converters.ContainsKey(typeof(T));
        }

        public bool CanConvert(Type t)
        {
            return converters.ContainsKey(t);
        }

        public void RegisterConverter<T>(toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue, bool throewIfExists = true)
        {
            if (CanConvert<T>())
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


        //public void RegisterStructConverter<T>(toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue) where T : struct
        //{
        //    RegisterConverter<T>(toJSValue, fromJSValue);
        //}

        public JavaScriptValue ToJSValue<T>(T value)
        {
            if (CanConvert<T>())
            {
                var f = (converters[typeof(T)].Item1 as toJSValueDelegate<T>);
                if (f == null)
                {
                    throw new NotImplementedException($"type {typeof(T).FullName} does not support convert to JSValue");
                }
                else
                {
                    return f(serviceNode, value);

                }

            }
            else
            {
                throw new NotImplementedException($"type {typeof(T).FullName} not registered for convertion");
            }
        }

        public T FromJSValue<T>(JavaScriptValue value)
        {
            if (CanConvert<T>())
            {
                var f = (converters[typeof(T)].Item2 as fromJSValueDelegate<T>);
                if (f == null)
                {
                    throw new NotImplementedException($"type {typeof(T).FullName} does not support convert from JSValue");
                }
                else
                {
                    return f(serviceNode,value);
                }
            }
            else
            {
                throw new NotImplementedException($"type {typeof(T).FullName} not registered for convertion");
            }
        }

    }
}
