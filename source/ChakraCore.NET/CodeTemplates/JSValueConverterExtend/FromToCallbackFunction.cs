
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using static ChakraCore.NET.API.JavaScriptContext;
namespace ChakraCore.NET
{
public partial class JSValueConverter
{
private Func<TResult> fromJSCallbackFunction<TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            return () =>
            {
                return fromJSFunction<TResult>(context, value)(false);
            };
        }

        private JavaScriptValue toJSCallbackFunction<TResult> (ValueConvertContext context, Func<TResult> callback)
        {
            return toJSFunction<TResult>(context, (b) =>
              {
                  return callback();
              }
            );
        }



private Func<T1,TResult> fromJSCallbackFunction<T1,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            return (T1 para1) =>
            {
                return fromJSFunction<T1,TResult>(context, value)(false,para1);
            };
        }

        private JavaScriptValue toJSCallbackFunction<T1,TResult> (ValueConvertContext context, Func<T1,TResult> callback)
        {
            return toJSFunction<T1,TResult>(context, (b,para1) =>
              {
                  return callback(para1);
              }
            );
        }



private Func<T1,T2,TResult> fromJSCallbackFunction<T1,T2,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            return (T1 para1,T2 para2) =>
            {
                return fromJSFunction<T1,T2,TResult>(context, value)(false,para1,para2);
            };
        }

        private JavaScriptValue toJSCallbackFunction<T1,T2,TResult> (ValueConvertContext context, Func<T1,T2,TResult> callback)
        {
            return toJSFunction<T1,T2,TResult>(context, (b,para1,para2) =>
              {
                  return callback(para1,para2);
              }
            );
        }



private Func<T1,T2,T3,TResult> fromJSCallbackFunction<T1,T2,T3,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3) =>
            {
                return fromJSFunction<T1,T2,T3,TResult>(context, value)(false,para1,para2,para3);
            };
        }

        private JavaScriptValue toJSCallbackFunction<T1,T2,T3,TResult> (ValueConvertContext context, Func<T1,T2,T3,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,TResult>(context, (b,para1,para2,para3) =>
              {
                  return callback(para1,para2,para3);
              }
            );
        }



private Func<T1,T2,T3,T4,TResult> fromJSCallbackFunction<T1,T2,T3,T4,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3,T4 para4) =>
            {
                return fromJSFunction<T1,T2,T3,T4,TResult>(context, value)(false,para1,para2,para3,para4);
            };
        }

        private JavaScriptValue toJSCallbackFunction<T1,T2,T3,T4,TResult> (ValueConvertContext context, Func<T1,T2,T3,T4,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,T4,TResult>(context, (b,para1,para2,para3,para4) =>
              {
                  return callback(para1,para2,para3,para4);
              }
            );
        }



private Func<T1,T2,T3,T4,T5,TResult> fromJSCallbackFunction<T1,T2,T3,T4,T5,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5) =>
            {
                return fromJSFunction<T1,T2,T3,T4,T5,TResult>(context, value)(false,para1,para2,para3,para4,para5);
            };
        }

        private JavaScriptValue toJSCallbackFunction<T1,T2,T3,T4,T5,TResult> (ValueConvertContext context, Func<T1,T2,T3,T4,T5,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,T4,T5,TResult>(context, (b,para1,para2,para3,para4,para5) =>
              {
                  return callback(para1,para2,para3,para4,para5);
              }
            );
        }



private Func<T1,T2,T3,T4,T5,T6,TResult> fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6) =>
            {
                return fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>(context, value)(false,para1,para2,para3,para4,para5,para6);
            };
        }

        private JavaScriptValue toJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult> (ValueConvertContext context, Func<T1,T2,T3,T4,T5,T6,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,T4,T5,T6,TResult>(context, (b,para1,para2,para3,para4,para5,para6) =>
              {
                  return callback(para1,para2,para3,para4,para5,para6);
              }
            );
        }



private Func<T1,T2,T3,T4,T5,T6,T7,TResult> fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7) =>
            {
                return fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(context, value)(false,para1,para2,para3,para4,para5,para6,para7);
            };
        }

        private JavaScriptValue toJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult> (ValueConvertContext context, Func<T1,T2,T3,T4,T5,T6,T7,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(context, (b,para1,para2,para3,para4,para5,para6,para7) =>
              {
                  return callback(para1,para2,para3,para4,para5,para6,para7);
              }
            );
        }




    }
}
