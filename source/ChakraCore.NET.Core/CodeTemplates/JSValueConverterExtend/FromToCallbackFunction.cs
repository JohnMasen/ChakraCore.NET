
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
namespace ChakraCore.NET
{
public static partial class JSValueConverterHelper
{
        private static Func<TResult> fromJSCallbackFunction<TResult>(IServiceNode node, JavaScriptValue value)
        {
            return () =>
            {
                return fromJSFunction<TResult>(node, value)(false);
            };
        }

        private static JavaScriptValue toJSCallbackFunction<TResult> (IServiceNode node, Func<TResult> callback)
        {
            return toJSFunction<TResult>(node, (b) =>
              {
                  return callback();
              }
            );
        }



        private static Func<T1,TResult> fromJSCallbackFunction<T1,TResult>(IServiceNode node, JavaScriptValue value)
        {
            return (T1 para1) =>
            {
                return fromJSFunction<T1,TResult>(node, value)(false,para1);
            };
        }

        private static JavaScriptValue toJSCallbackFunction<T1,TResult> (IServiceNode node, Func<T1,TResult> callback)
        {
            return toJSFunction<T1,TResult>(node, (b,para1) =>
              {
                  return callback(para1);
              }
            );
        }



        private static Func<T1,T2,TResult> fromJSCallbackFunction<T1,T2,TResult>(IServiceNode node, JavaScriptValue value)
        {
            return (T1 para1,T2 para2) =>
            {
                return fromJSFunction<T1,T2,TResult>(node, value)(false,para1,para2);
            };
        }

        private static JavaScriptValue toJSCallbackFunction<T1,T2,TResult> (IServiceNode node, Func<T1,T2,TResult> callback)
        {
            return toJSFunction<T1,T2,TResult>(node, (b,para1,para2) =>
              {
                  return callback(para1,para2);
              }
            );
        }



        private static Func<T1,T2,T3,TResult> fromJSCallbackFunction<T1,T2,T3,TResult>(IServiceNode node, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3) =>
            {
                return fromJSFunction<T1,T2,T3,TResult>(node, value)(false,para1,para2,para3);
            };
        }

        private static JavaScriptValue toJSCallbackFunction<T1,T2,T3,TResult> (IServiceNode node, Func<T1,T2,T3,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,TResult>(node, (b,para1,para2,para3) =>
              {
                  return callback(para1,para2,para3);
              }
            );
        }



        private static Func<T1,T2,T3,T4,TResult> fromJSCallbackFunction<T1,T2,T3,T4,TResult>(IServiceNode node, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3,T4 para4) =>
            {
                return fromJSFunction<T1,T2,T3,T4,TResult>(node, value)(false,para1,para2,para3,para4);
            };
        }

        private static JavaScriptValue toJSCallbackFunction<T1,T2,T3,T4,TResult> (IServiceNode node, Func<T1,T2,T3,T4,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,T4,TResult>(node, (b,para1,para2,para3,para4) =>
              {
                  return callback(para1,para2,para3,para4);
              }
            );
        }



        private static Func<T1,T2,T3,T4,T5,TResult> fromJSCallbackFunction<T1,T2,T3,T4,T5,TResult>(IServiceNode node, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5) =>
            {
                return fromJSFunction<T1,T2,T3,T4,T5,TResult>(node, value)(false,para1,para2,para3,para4,para5);
            };
        }

        private static JavaScriptValue toJSCallbackFunction<T1,T2,T3,T4,T5,TResult> (IServiceNode node, Func<T1,T2,T3,T4,T5,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,T4,T5,TResult>(node, (b,para1,para2,para3,para4,para5) =>
              {
                  return callback(para1,para2,para3,para4,para5);
              }
            );
        }



        private static Func<T1,T2,T3,T4,T5,T6,TResult> fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult>(IServiceNode node, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6) =>
            {
                return fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>(node, value)(false,para1,para2,para3,para4,para5,para6);
            };
        }

        private static JavaScriptValue toJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult> (IServiceNode node, Func<T1,T2,T3,T4,T5,T6,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,T4,T5,T6,TResult>(node, (b,para1,para2,para3,para4,para5,para6) =>
              {
                  return callback(para1,para2,para3,para4,para5,para6);
              }
            );
        }



        private static Func<T1,T2,T3,T4,T5,T6,T7,TResult> fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(IServiceNode node, JavaScriptValue value)
        {
            return (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7) =>
            {
                return fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(node, value)(false,para1,para2,para3,para4,para5,para6,para7);
            };
        }

        private static JavaScriptValue toJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult> (IServiceNode node, Func<T1,T2,T3,T4,T5,T6,T7,TResult> callback)
        {
            return toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(node, (b,para1,para2,para3,para4,para5,para6,para7) =>
              {
                  return callback(para1,para2,para3,para4,para5,para6,para7);
              }
            );
        }




    }
}
