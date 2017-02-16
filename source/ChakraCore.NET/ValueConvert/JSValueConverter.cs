using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ChakraCore.NET
{
    public partial class JSValueConverter
    {
        public delegate JavaScriptValue toJSValueDelegate<T>(JSValueConvertContext convertContext, T value);
        public delegate TResult fromJSValueDelegate<out TResult>(JSValueConvertContext convertContext, JavaScriptValue value);
        partial void initDefault();
        public JSValueConverter()
        {
            initDefault();
        }

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

        public JavaScriptValue ToJSValue<T>(JSValueConvertContext context, T value)
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

        public T FromJSValue<T>(JSValueConvertContext context, JavaScriptValue value)
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

        public void RegisterProxyConverter<T>(Action<JSValueBinding,T> callback)where T:class
        {
            toJSValueDelegate<T> tojs = (JSValueConvertContext convertContext, T value) =>
              {
                  JavaScriptValue? result = convertContext.RuntimeContext.ProxyMapManager.GetProxy<T>(value);
                  if (result.HasValue)
                  {
                      return result.Value;//proxy exists, no transfer required.
                  }
                  else
                  {
                      //create proxy item, CreateProxy<T> will auto cache the created proxy 
                      JavaScriptValue tResult = convertContext.RuntimeContext.CreateProxy<T>(value, out GC.DelegateHandler handler);
                      JSValueBinding binding = new JSValueBinding(
                          convertContext.RuntimeContext,
                          tResult, new JSValueConvertContext(convertContext.RuntimeContext, handler, tResult));//create value binding object for user setup binding
                      callback(binding, value);
                      return tResult;
                  }
              };
            fromJSValueDelegate<T> fromjs = (JSValueConvertContext convertContext, JavaScriptValue value) =>
              {
                  return convertContext.RuntimeContext.ProxyMapManager.GetSource<T>(value.ExternalData);
              };
            RegisterConverter<T>(tojs, fromjs);
        }

        public void RegisterArrayConverter<T>()
        {
            toJSValueDelegate<IEnumerable<T>> tojs = (context, value) =>
              {
                  return context.RuntimeContext.With<JavaScriptValue>(()=>
                  {
                      var result =JavaScriptValue.CreateArray(Convert.ToUInt32(value.Count()));
                      int index = 0;
                      foreach (T item in value)
                      {
                          result.SetIndexedProperty(ToJSValue<int>(context,index++), ToJSValue<T>(context,item));
                      }
                      return result;
                  }
                  );
              };
            fromJSValueDelegate<IEnumerable<T>> fromjs = (context, value) =>
              {
                  return context.RuntimeContext.With<IEnumerable<T>>(() =>
                  {
                      var length = FromJSValue<int>(context, value.GetProperty(JavaScriptPropertyId.FromString("length")));
                      List<T> result = new List<T>(length);//copy the data to avoid context switch in user code
                      for (int i = 0; i < length; i++)
                        {
                              result.Add(
                                  FromJSValue<T>(context, 
                                      value.GetIndexedProperty(
                                              ToJSValue<int>(context,i))
                                              )
                                       );
                        }
                      return result;
                  }
                  );
              };
            RegisterConverter<IEnumerable<T>>(tojs, fromjs,false);
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
        //        //context.JSClass = arguments[0];//put the caller object to context
        //        T1 para1 = FromJSValue<T1>(context, arguments[1]);

        //        context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

        //        a(para1);

        //        context.RuntimeContext.Enter();//restore context
        //        return context.RuntimeContext.JSValue_Undefined;
        //    };
        //    context.Handler.Hold(f);

        //    return context.RuntimeContext.With<JavaScriptValue>(() =>
        //    {
        //        return JavaScriptValue.CreateFunction(f);
        //    }
        //    );
        //}

        //private JavaScriptValue toJSFunction<T1, TResult>(ValueConvertContext context, Func<bool, T1, TResult> callback)
        //{
        //    JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
        //    {
        //        if (argumentCount != 2)
        //        {
        //            throw new InvalidOperationException("call from javascript did not pass enough parameters");
        //        }
        //        //context.JSClass = arguments[0];//put the caller object to context
        //        T1 para1 = FromJSValue<T1>(context, arguments[1]);

        //        context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

        //        TResult result = callback(isConstructCall, para1);

        //        context.RuntimeContext.Enter();//restore context
        //        return ToJSValue<TResult>(context, result); ;
        //    };
        //    context.Handler.Hold(f);

        //    return context.RuntimeContext.With<JavaScriptValue>(() =>
        //    {
        //        return JavaScriptValue.CreateFunction(f);
        //    }
        //    );
        //}

        //private Action<T1> fromJSMethod<T1>(ValueConvertContext context, JavaScriptValue value)
        //{
        //    Action<T1> result = (T1 para1) =>
        //    {

        //        JavaScriptValue p1 = ToJSValue<T1>(context, para1);


        //        context.RuntimeContext.With(() =>
        //        {
        //            value.CallFunction(context.JSClass, p1);
        //        });
        //    };
        //    return result;
        //}

        //private Func<bool, T1, TResult> fromJSFunction<T1, TResult>(ValueConvertContext context, JavaScriptValue value)
        //{
        //    Func<bool, T1, TResult> result = (bool isConstruct, T1 para1) =>
        //    {
        //        JavaScriptValue p1 = ToJSValue<T1>(context, para1);

        //        JavaScriptValue r = context.RuntimeContext.With<JavaScriptValue>(() =>
        //        {
        //            if (isConstruct)
        //            {
        //                return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
        //            }
        //            else
        //            {
        //                return value.CallFunction(context.JSClass, p1);
        //            }
        //        });
        //        return FromJSValue<TResult>(context, r);
        //    };
        //    return result;
        //}



        //public void RegisterMethodConverter<T1>()
        //{
        //    RegisterConverter<Action<T1>>(toJSMethod<T1>, fromJSMethod<T1>, false);
        //}

        //public void RegisterFunctionConverter<T1, TResult>()
        //{
        //    RegisterConverter<Func<bool, T1, TResult>>(toJSFunction<T1, TResult>, fromJSFunction<T1, TResult>, false);
        //}
        #endregion



    }
}
