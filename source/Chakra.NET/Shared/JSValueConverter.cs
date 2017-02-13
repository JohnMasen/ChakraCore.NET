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


        public void RegisterStructConverter<T>(toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue) where T : struct
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

        public void RegisterProxyConverter<T>(Action<JSValueBinding> callback)
        {

        }


        #region Method Template
        //private JavaScriptValue toJSMethod(ValueConvertContext context, Action a)
        //{
        //    JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
        //    {
        //        if (argumentCount != 1)
        //        {
        //            throw new InvalidOperationException("call from javascript did not pass enough parameters");
        //        }
        //        context.JSClass = arguments[0];//put the caller object to context


        //        context.Context.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

        //        a();

        //        context.Context.Enter();//restore context
        //        return context.Context.JSValue_Undefined;
        //    };
        //    context.Handler.Hold(f);

        //    using (context.Context.With())
        //    {
        //        return JavaScriptValue.CreateFunction(f);
        //    }
        //}

        //private JavaScriptValue toJSFunction<TResult>(ValueConvertContext context, Func<bool, TResult> callback)
        //{
        //    JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
        //    {
        //        if (argumentCount != 1)
        //        {
        //            throw new InvalidOperationException("call from javascript did not pass enough parameters");
        //        }
        //        context.JSClass = arguments[0];//put the caller object to context


        //        context.Context.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

        //        TResult result = callback(isConstructCall);

        //        context.Context.Enter();//restore context
        //        return ToJSValue<TResult>(context, result); ;
        //    };
        //    context.Handler.Hold(f);

        //    using (context.Context.With())
        //    {
        //        return JavaScriptValue.CreateFunction(f);
        //    }
        //}

        //private Action fromJSMethod(ValueConvertContext context, JavaScriptValue value)
        //{
        //    Action result = () =>
        //    {




        //        using (context.Context.With())
        //        {
        //            value.CallFunction(context.JSClass);
        //        }
        //    };
        //    return result;
        //}

        //private Func<bool, TResult> fromJSFunction<TResult>(ValueConvertContext context, JavaScriptValue value)
        //{
        //    Func<bool, TResult> result = (bool isConstruct) =>
        //    {


        //        JavaScriptValue r;
        //        using (context.Context.With())
        //        {
        //            if (isConstruct)
        //            {
        //                r = value.ConstructObject(context.Context.JSValue_Undefined);
        //            }
        //            else
        //            {
        //                r = value.CallFunction(context.JSClass);
        //            }
        //        }
        //        return FromJSValue<TResult>(context, r);
        //    };
        //    return result;
        //}

        //public void RegisterMethodConverter<T>()
        //{
        //    RegisterConverter<Action<T>>(toJSMethod<T>, fromJSMethod<T>, false);
        //}

        //public void RegisterFunctionConverter<T>()
        //{
        //    RegisterConverter<Func<bool, T>>(toJSFunction<T>, fromJSFunction<T>);
        //}
        #endregion



    }
}
