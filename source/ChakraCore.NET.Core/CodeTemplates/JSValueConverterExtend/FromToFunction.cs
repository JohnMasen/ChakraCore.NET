
using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
namespace ChakraCore.NET.Core
{
public  static partial class JSValueConverterHelper
{

        private static JavaScriptValue toJSFunction<TResult> (IServiceNode node, Func<bool,TResult> callback)
        {
            var converter = node.GetService<IJSValueConverterService>();
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 1)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                
                

                TResult result=callback(isConstructCall);
                
                return converter.ToJSValue<TResult>(result);
            };

            return node.GetService<IContextService>().CreateFunction(f, IntPtr.Zero);
        }

        

        private static Func<bool,TResult> fromJSFunction<TResult>(IServiceNode node, JavaScriptValue value)
        {
            var converter = node.GetService<IJSValueConverterService>();
            var context = node.GetService<IContextService>();
            var callContext = node.GetService<ICallContextService>();
            Func<bool,TResult> result = (bool isConstruct) =>
            {
                

                JavaScriptValue r=node.WithContext<JavaScriptValue>(()=>
                {
                
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= context.ConstructObject(value,context.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= context.CallFunction(value,callContext.Caller);
                    }
                
                    return resultValue;
                });
                return converter.FromJSValue<TResult>(r);
            };
            return result;
        }



        private static JavaScriptValue toJSFunction<T1,TResult> (IServiceNode node, Func<bool,T1,TResult> callback)
        {
            var converter = node.GetService<IJSValueConverterService>();
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 2)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                T1 para1 = converter.FromJSValue<T1>(arguments[1]);
                arguments[1].AddRef();

                TResult result=callback(isConstructCall,para1);
                arguments[1].Release();
                return converter.ToJSValue<TResult>(result);
            };

            return node.GetService<IContextService>().CreateFunction(f, IntPtr.Zero);
        }

        

        private static Func<bool,T1,TResult> fromJSFunction<T1,TResult>(IServiceNode node, JavaScriptValue value)
        {
            var converter = node.GetService<IJSValueConverterService>();
            var context = node.GetService<IContextService>();
            var callContext = node.GetService<ICallContextService>();
            Func<bool,T1,TResult> result = (bool isConstruct,T1 para1) =>
            {
                JavaScriptValue p1 = converter.ToJSValue<T1>(para1);

                JavaScriptValue r=node.WithContext<JavaScriptValue>(()=>
                {
                p1.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= context.ConstructObject(value,context.JSValue_Undefined,p1);
                    }
                    else
                    {
                        resultValue= context.CallFunction(value,callContext.Caller,p1);
                    }
                p1.Release();
                    return resultValue;
                });
                return converter.FromJSValue<TResult>(r);
            };
            return result;
        }



        private static JavaScriptValue toJSFunction<T1,T2,TResult> (IServiceNode node, Func<bool,T1,T2,TResult> callback)
        {
            var converter = node.GetService<IJSValueConverterService>();
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 3)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                T1 para1 = converter.FromJSValue<T1>(arguments[1]);
T2 para2 = converter.FromJSValue<T2>(arguments[2]);
                arguments[1].AddRef();
arguments[2].AddRef();

                TResult result=callback(isConstructCall,para1,para2);
                arguments[1].Release();
arguments[2].Release();
                return converter.ToJSValue<TResult>(result);
            };

            return node.GetService<IContextService>().CreateFunction(f, IntPtr.Zero);
        }

        

        private static Func<bool,T1,T2,TResult> fromJSFunction<T1,T2,TResult>(IServiceNode node, JavaScriptValue value)
        {
            var converter = node.GetService<IJSValueConverterService>();
            var context = node.GetService<IContextService>();
            var callContext = node.GetService<ICallContextService>();
            Func<bool,T1,T2,TResult> result = (bool isConstruct,T1 para1,T2 para2) =>
            {
                JavaScriptValue p1 = converter.ToJSValue<T1>(para1);
JavaScriptValue p2 = converter.ToJSValue<T2>(para2);

                JavaScriptValue r=node.WithContext<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= context.ConstructObject(value,context.JSValue_Undefined,p1,p2);
                    }
                    else
                    {
                        resultValue= context.CallFunction(value,callContext.Caller,p1,p2);
                    }
                p1.Release();
p2.Release();
                    return resultValue;
                });
                return converter.FromJSValue<TResult>(r);
            };
            return result;
        }



        private static JavaScriptValue toJSFunction<T1,T2,T3,TResult> (IServiceNode node, Func<bool,T1,T2,T3,TResult> callback)
        {
            var converter = node.GetService<IJSValueConverterService>();
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 4)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                T1 para1 = converter.FromJSValue<T1>(arguments[1]);
T2 para2 = converter.FromJSValue<T2>(arguments[2]);
T3 para3 = converter.FromJSValue<T3>(arguments[3]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();

                TResult result=callback(isConstructCall,para1,para2,para3);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
                return converter.ToJSValue<TResult>(result);
            };

            return node.GetService<IContextService>().CreateFunction(f, IntPtr.Zero);
        }

        

        private static Func<bool,T1,T2,T3,TResult> fromJSFunction<T1,T2,T3,TResult>(IServiceNode node, JavaScriptValue value)
        {
            var converter = node.GetService<IJSValueConverterService>();
            var context = node.GetService<IContextService>();
            var callContext = node.GetService<ICallContextService>();
            Func<bool,T1,T2,T3,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3) =>
            {
                JavaScriptValue p1 = converter.ToJSValue<T1>(para1);
JavaScriptValue p2 = converter.ToJSValue<T2>(para2);
JavaScriptValue p3 = converter.ToJSValue<T3>(para3);

                JavaScriptValue r=node.WithContext<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
p3.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= context.ConstructObject(value,context.JSValue_Undefined,p1,p2,p3);
                    }
                    else
                    {
                        resultValue= context.CallFunction(value,callContext.Caller,p1,p2,p3);
                    }
                p1.Release();
p2.Release();
p3.Release();
                    return resultValue;
                });
                return converter.FromJSValue<TResult>(r);
            };
            return result;
        }



        private static JavaScriptValue toJSFunction<T1,T2,T3,T4,TResult> (IServiceNode node, Func<bool,T1,T2,T3,T4,TResult> callback)
        {
            var converter = node.GetService<IJSValueConverterService>();
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 5)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                T1 para1 = converter.FromJSValue<T1>(arguments[1]);
T2 para2 = converter.FromJSValue<T2>(arguments[2]);
T3 para3 = converter.FromJSValue<T3>(arguments[3]);
T4 para4 = converter.FromJSValue<T4>(arguments[4]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
arguments[4].AddRef();

                TResult result=callback(isConstructCall,para1,para2,para3,para4);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
arguments[4].Release();
                return converter.ToJSValue<TResult>(result);
            };

            return node.GetService<IContextService>().CreateFunction(f, IntPtr.Zero);
        }

        

        private static Func<bool,T1,T2,T3,T4,TResult> fromJSFunction<T1,T2,T3,T4,TResult>(IServiceNode node, JavaScriptValue value)
        {
            var converter = node.GetService<IJSValueConverterService>();
            var context = node.GetService<IContextService>();
            var callContext = node.GetService<ICallContextService>();
            Func<bool,T1,T2,T3,T4,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4) =>
            {
                JavaScriptValue p1 = converter.ToJSValue<T1>(para1);
JavaScriptValue p2 = converter.ToJSValue<T2>(para2);
JavaScriptValue p3 = converter.ToJSValue<T3>(para3);
JavaScriptValue p4 = converter.ToJSValue<T4>(para4);

                JavaScriptValue r=node.WithContext<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
p3.AddRef();
p4.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= context.ConstructObject(value,context.JSValue_Undefined,p1,p2,p3,p4);
                    }
                    else
                    {
                        resultValue= context.CallFunction(value,callContext.Caller,p1,p2,p3,p4);
                    }
                p1.Release();
p2.Release();
p3.Release();
p4.Release();
                    return resultValue;
                });
                return converter.FromJSValue<TResult>(r);
            };
            return result;
        }



        private static JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,TResult> (IServiceNode node, Func<bool,T1,T2,T3,T4,T5,TResult> callback)
        {
            var converter = node.GetService<IJSValueConverterService>();
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 6)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                T1 para1 = converter.FromJSValue<T1>(arguments[1]);
T2 para2 = converter.FromJSValue<T2>(arguments[2]);
T3 para3 = converter.FromJSValue<T3>(arguments[3]);
T4 para4 = converter.FromJSValue<T4>(arguments[4]);
T5 para5 = converter.FromJSValue<T5>(arguments[5]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
arguments[4].AddRef();
arguments[5].AddRef();

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
arguments[4].Release();
arguments[5].Release();
                return converter.ToJSValue<TResult>(result);
            };

            return node.GetService<IContextService>().CreateFunction(f, IntPtr.Zero);
        }

        

        private static Func<bool,T1,T2,T3,T4,T5,TResult> fromJSFunction<T1,T2,T3,T4,T5,TResult>(IServiceNode node, JavaScriptValue value)
        {
            var converter = node.GetService<IJSValueConverterService>();
            var context = node.GetService<IContextService>();
            var callContext = node.GetService<ICallContextService>();
            Func<bool,T1,T2,T3,T4,T5,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5) =>
            {
                JavaScriptValue p1 = converter.ToJSValue<T1>(para1);
JavaScriptValue p2 = converter.ToJSValue<T2>(para2);
JavaScriptValue p3 = converter.ToJSValue<T3>(para3);
JavaScriptValue p4 = converter.ToJSValue<T4>(para4);
JavaScriptValue p5 = converter.ToJSValue<T5>(para5);

                JavaScriptValue r=node.WithContext<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
p3.AddRef();
p4.AddRef();
p5.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= context.ConstructObject(value,context.JSValue_Undefined,p1,p2,p3,p4,p5);
                    }
                    else
                    {
                        resultValue= context.CallFunction(value,callContext.Caller,p1,p2,p3,p4,p5);
                    }
                p1.Release();
p2.Release();
p3.Release();
p4.Release();
p5.Release();
                    return resultValue;
                });
                return converter.FromJSValue<TResult>(r);
            };
            return result;
        }



        private static JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,T6,TResult> (IServiceNode node, Func<bool,T1,T2,T3,T4,T5,T6,TResult> callback)
        {
            var converter = node.GetService<IJSValueConverterService>();
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 7)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                T1 para1 = converter.FromJSValue<T1>(arguments[1]);
T2 para2 = converter.FromJSValue<T2>(arguments[2]);
T3 para3 = converter.FromJSValue<T3>(arguments[3]);
T4 para4 = converter.FromJSValue<T4>(arguments[4]);
T5 para5 = converter.FromJSValue<T5>(arguments[5]);
T6 para6 = converter.FromJSValue<T6>(arguments[6]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
arguments[4].AddRef();
arguments[5].AddRef();
arguments[6].AddRef();

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5,para6);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
arguments[4].Release();
arguments[5].Release();
arguments[6].Release();
                return converter.ToJSValue<TResult>(result);
            };

            return node.GetService<IContextService>().CreateFunction(f, IntPtr.Zero);
        }

        

        private static Func<bool,T1,T2,T3,T4,T5,T6,TResult> fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>(IServiceNode node, JavaScriptValue value)
        {
            var converter = node.GetService<IJSValueConverterService>();
            var context = node.GetService<IContextService>();
            var callContext = node.GetService<ICallContextService>();
            Func<bool,T1,T2,T3,T4,T5,T6,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6) =>
            {
                JavaScriptValue p1 = converter.ToJSValue<T1>(para1);
JavaScriptValue p2 = converter.ToJSValue<T2>(para2);
JavaScriptValue p3 = converter.ToJSValue<T3>(para3);
JavaScriptValue p4 = converter.ToJSValue<T4>(para4);
JavaScriptValue p5 = converter.ToJSValue<T5>(para5);
JavaScriptValue p6 = converter.ToJSValue<T6>(para6);

                JavaScriptValue r=node.WithContext<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
p3.AddRef();
p4.AddRef();
p5.AddRef();
p6.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= context.ConstructObject(value,context.JSValue_Undefined,p1,p2,p3,p4,p5,p6);
                    }
                    else
                    {
                        resultValue= context.CallFunction(value,callContext.Caller,p1,p2,p3,p4,p5,p6);
                    }
                p1.Release();
p2.Release();
p3.Release();
p4.Release();
p5.Release();
p6.Release();
                    return resultValue;
                });
                return converter.FromJSValue<TResult>(r);
            };
            return result;
        }



        private static JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult> (IServiceNode node, Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult> callback)
        {
            var converter = node.GetService<IJSValueConverterService>();
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 8)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                T1 para1 = converter.FromJSValue<T1>(arguments[1]);
T2 para2 = converter.FromJSValue<T2>(arguments[2]);
T3 para3 = converter.FromJSValue<T3>(arguments[3]);
T4 para4 = converter.FromJSValue<T4>(arguments[4]);
T5 para5 = converter.FromJSValue<T5>(arguments[5]);
T6 para6 = converter.FromJSValue<T6>(arguments[6]);
T7 para7 = converter.FromJSValue<T7>(arguments[7]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
arguments[4].AddRef();
arguments[5].AddRef();
arguments[6].AddRef();
arguments[7].AddRef();

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5,para6,para7);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
arguments[4].Release();
arguments[5].Release();
arguments[6].Release();
arguments[7].Release();
                return converter.ToJSValue<TResult>(result);
            };

            return node.GetService<IContextService>().CreateFunction(f, IntPtr.Zero);
        }

        

        private static Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult> fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(IServiceNode node, JavaScriptValue value)
        {
            var converter = node.GetService<IJSValueConverterService>();
            var context = node.GetService<IContextService>();
            var callContext = node.GetService<ICallContextService>();
            Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7) =>
            {
                JavaScriptValue p1 = converter.ToJSValue<T1>(para1);
JavaScriptValue p2 = converter.ToJSValue<T2>(para2);
JavaScriptValue p3 = converter.ToJSValue<T3>(para3);
JavaScriptValue p4 = converter.ToJSValue<T4>(para4);
JavaScriptValue p5 = converter.ToJSValue<T5>(para5);
JavaScriptValue p6 = converter.ToJSValue<T6>(para6);
JavaScriptValue p7 = converter.ToJSValue<T7>(para7);

                JavaScriptValue r=node.WithContext<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
p3.AddRef();
p4.AddRef();
p5.AddRef();
p6.AddRef();
p7.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= context.ConstructObject(value,context.JSValue_Undefined,p1,p2,p3,p4,p5,p6,p7);
                    }
                    else
                    {
                        resultValue= context.CallFunction(value,callContext.Caller,p1,p2,p3,p4,p5,p6,p7);
                    }
                p1.Release();
p2.Release();
p3.Release();
p4.Release();
p5.Release();
p6.Release();
p7.Release();
                    return resultValue;
                });
                return converter.FromJSValue<TResult>(r);
            };
            return result;
        }



    }
}
