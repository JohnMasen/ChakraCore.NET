using Chakra.NET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Chakra.NET
{
    public partial class JSValueConverter
    {
        public delegate JavaScriptValue toJSValueDelegate<T>(ValueConvertContext convertContext, T value);
        public delegate TResult fromJSValueDelegate<out TResult>(ValueConvertContext convertContext, JavaScriptValue value);


        private Dictionary<Type, Tuple<object, object>> converters = new Dictionary<Type, Tuple<object, object>>();

        internal void RegisterConverter<T>(toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue, bool throewIfExists = true)
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


        public void RegisterValueConverter<T>(toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue) where T : struct
        {
            RegisterConverter<T>(toJSValue, fromJSValue);
        }

        public JavaScriptValue ToJSValue<T>(ValueConvertContext context, T value)
        {
            if (converters.ContainsKey(typeof(T)))
            {
                var f = (converters[typeof(T)].Item1 as toJSValueDelegate<T>);
                if (f == null)
                {
                    throw new NotImplementedException($"type {typeof(T).FullName} does not support convert to JSValue");
                }
                else
                {
                    return f(context, value);

                }

            }
            else
            {
                throw new NotImplementedException($"type {typeof(T).FullName} not registered for convertion");
            }
        }

        public T FromJSValue<T>(ValueConvertContext context, JavaScriptValue value)
        {
            if (converters.ContainsKey(typeof(T)))
            {
                var f = (converters[typeof(T)].Item2 as fromJSValueDelegate<T>);
                if (f == null)
                {
                    throw new NotImplementedException($"type {typeof(T).FullName} does not support convert from JSValue");
                }
                else
                {
                    return f(context, value);
                }
            }
            else
            {
                throw new NotImplementedException($"type {typeof(T).FullName} not registered for convertion");
            }
        }

        #region Method Template
        //private JavaScriptValue toJSMethod<T1>(ValueConvertContext context, Action<T1> a)
        //{
        //    JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
        //    {
        //        if (argumentCount != 2)
        //        {
        //            throw new InvalidOperationException("call from javascript did not pass enough parameters");
        //        }
        //        context.JSClass = arguments[0];//put the caller object to context
        //        T1 para1 = FromJSValue<T1>(context, arguments[1]);

        //        context.Context.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

        //        a(para1);

        //        context.Context.Enter();//restore context
        //        return context.Context.JSValue_Undefined;
        //    };
        //    context.Handler.Hold(f);

        //    using (context.Context.With())
        //    {
        //        return JavaScriptValue.CreateFunction(f);
        //    }
        //}

        //private JavaScriptValue toJSFunction<T1,TResult>(ValueConvertContext context, Func<T1,TResult> function)
        //{
        //    JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
        //    {
        //        if (argumentCount != 2)
        //        {
        //            throw new InvalidOperationException("call from javascript did not pass enough parameters");
        //        }
        //        context.JSClass = arguments[0];//put the caller object to context
        //        T1 para1 = FromJSValue<T1>(context, arguments[1]);

        //        context.Context.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

        //        TResult result= function(para1);

        //        context.Context.Enter();//restore context
        //        return ToJSValue<TResult>(context,result);
        //    };
        //    context.Handler.Hold(f);

        //    using (context.Context.With())
        //    {
        //        return JavaScriptValue.CreateFunction(f);
        //    }
        //}
        //private Action<T> fromJSMethod<T>(ValueConvertContext context,JavaScriptValue value)
        //{
        //    Action<T> result = (T para1) =>
        //      {
        //          JavaScriptValue p1 = ToJSValue<T>(context,para1);

        //          using (context.Context.With())
        //          {
        //              value.CallFunction(context.JSClass, p1);
        //          }
        //      };
        //    return result;
        //}

        private Func<T1,TResult> fromJSFunction<T1,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            Func<T1, TResult> result = (T1 para1) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context, para1);
                JavaScriptValue r;
                using (context.Context.With())
                {
                    r=value.CallFunction(context.JSClass, p1);
                }
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }
        #endregion



    }
}
