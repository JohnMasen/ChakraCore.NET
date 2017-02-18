
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using static ChakraCore.NET.API.JavaScriptContext;
namespace ChakraCore.NET
{
public partial class JSValueConverter
{

private JavaScriptValue toJSFunction<TResult> (JSValueConvertContext context, Func<bool,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 1)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                
                
                //context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall);
                
                context.RuntimeContext.Enter();//restore context
                return ToJSValue<TResult>(context,result);;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
            {
                return JavaScriptValue.CreateFunction(f);
            }
            );
        }

        

        private Func<bool,TResult> fromJSFunction<TResult>(JSValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,TResult> result = (bool isConstruct) =>
            {
                

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= value.CallFunction(context.JSClass);
                    }
                
                    return resultValue;
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



private JavaScriptValue toJSFunction<T1,TResult> (JSValueConvertContext context, Func<bool,T1,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 2)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);
                arguments[1].AddRef();
                //context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1);
                arguments[1].Release();
                context.RuntimeContext.Enter();//restore context
                return ToJSValue<TResult>(context,result);;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
            {
                return JavaScriptValue.CreateFunction(f);
            }
            );
        }

        

        private Func<bool,T1,TResult> fromJSFunction<T1,TResult>(JSValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,TResult> result = (bool isConstruct,T1 para1) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                p1.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= value.CallFunction(context.JSClass,p1);
                    }
                p1.Release();
                    return resultValue;
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



private JavaScriptValue toJSFunction<T1,T2,TResult> (JSValueConvertContext context, Func<bool,T1,T2,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 3)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);
T2 para2 = FromJSValue<T2>(context, arguments[2]);
                arguments[1].AddRef();
arguments[2].AddRef();
                //context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2);
                arguments[1].Release();
arguments[2].Release();
                context.RuntimeContext.Enter();//restore context
                return ToJSValue<TResult>(context,result);;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
            {
                return JavaScriptValue.CreateFunction(f);
            }
            );
        }

        

        private Func<bool,T1,T2,TResult> fromJSFunction<T1,T2,TResult>(JSValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,TResult> result = (bool isConstruct,T1 para1,T2 para2) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= value.CallFunction(context.JSClass,p1,p2);
                    }
                p1.Release();
p2.Release();
                    return resultValue;
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



private JavaScriptValue toJSFunction<T1,T2,T3,TResult> (JSValueConvertContext context, Func<bool,T1,T2,T3,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 4)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);
T2 para2 = FromJSValue<T2>(context, arguments[2]);
T3 para3 = FromJSValue<T3>(context, arguments[3]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
                //context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
                context.RuntimeContext.Enter();//restore context
                return ToJSValue<TResult>(context,result);;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
            {
                return JavaScriptValue.CreateFunction(f);
            }
            );
        }

        

        private Func<bool,T1,T2,T3,TResult> fromJSFunction<T1,T2,T3,TResult>(JSValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,T3,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
p3.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= value.CallFunction(context.JSClass,p1,p2,p3);
                    }
                p1.Release();
p2.Release();
p3.Release();
                    return resultValue;
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



private JavaScriptValue toJSFunction<T1,T2,T3,T4,TResult> (JSValueConvertContext context, Func<bool,T1,T2,T3,T4,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 5)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);
T2 para2 = FromJSValue<T2>(context, arguments[2]);
T3 para3 = FromJSValue<T3>(context, arguments[3]);
T4 para4 = FromJSValue<T4>(context, arguments[4]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
arguments[4].AddRef();
                //context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3,para4);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
arguments[4].Release();
                context.RuntimeContext.Enter();//restore context
                return ToJSValue<TResult>(context,result);;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
            {
                return JavaScriptValue.CreateFunction(f);
            }
            );
        }

        

        private Func<bool,T1,T2,T3,T4,TResult> fromJSFunction<T1,T2,T3,T4,TResult>(JSValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,T3,T4,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
p3.AddRef();
p4.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= value.CallFunction(context.JSClass,p1,p2,p3,p4);
                    }
                p1.Release();
p2.Release();
p3.Release();
p4.Release();
                    return resultValue;
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



private JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,TResult> (JSValueConvertContext context, Func<bool,T1,T2,T3,T4,T5,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 6)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);
T2 para2 = FromJSValue<T2>(context, arguments[2]);
T3 para3 = FromJSValue<T3>(context, arguments[3]);
T4 para4 = FromJSValue<T4>(context, arguments[4]);
T5 para5 = FromJSValue<T5>(context, arguments[5]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
arguments[4].AddRef();
arguments[5].AddRef();
                //context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
arguments[4].Release();
arguments[5].Release();
                context.RuntimeContext.Enter();//restore context
                return ToJSValue<TResult>(context,result);;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
            {
                return JavaScriptValue.CreateFunction(f);
            }
            );
        }

        

        private Func<bool,T1,T2,T3,T4,T5,TResult> fromJSFunction<T1,T2,T3,T4,T5,TResult>(JSValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,T3,T4,T5,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);
JavaScriptValue p5 = ToJSValue<T5>(context,para5);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                p1.AddRef();
p2.AddRef();
p3.AddRef();
p4.AddRef();
p5.AddRef();
                JavaScriptValue resultValue;
                    if (isConstruct)
                    {
                        resultValue= value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= value.CallFunction(context.JSClass,p1,p2,p3,p4,p5);
                    }
                p1.Release();
p2.Release();
p3.Release();
p4.Release();
p5.Release();
                    return resultValue;
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



private JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,T6,TResult> (JSValueConvertContext context, Func<bool,T1,T2,T3,T4,T5,T6,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 7)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);
T2 para2 = FromJSValue<T2>(context, arguments[2]);
T3 para3 = FromJSValue<T3>(context, arguments[3]);
T4 para4 = FromJSValue<T4>(context, arguments[4]);
T5 para5 = FromJSValue<T5>(context, arguments[5]);
T6 para6 = FromJSValue<T6>(context, arguments[6]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
arguments[4].AddRef();
arguments[5].AddRef();
arguments[6].AddRef();
                //context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5,para6);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
arguments[4].Release();
arguments[5].Release();
arguments[6].Release();
                context.RuntimeContext.Enter();//restore context
                return ToJSValue<TResult>(context,result);;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
            {
                return JavaScriptValue.CreateFunction(f);
            }
            );
        }

        

        private Func<bool,T1,T2,T3,T4,T5,T6,TResult> fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>(JSValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,T3,T4,T5,T6,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);
JavaScriptValue p5 = ToJSValue<T5>(context,para5);
JavaScriptValue p6 = ToJSValue<T6>(context,para6);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
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
                        resultValue= value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= value.CallFunction(context.JSClass,p1,p2,p3,p4,p5,p6);
                    }
                p1.Release();
p2.Release();
p3.Release();
p4.Release();
p5.Release();
p6.Release();
                    return resultValue;
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



private JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult> (JSValueConvertContext context, Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 8)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);
T2 para2 = FromJSValue<T2>(context, arguments[2]);
T3 para3 = FromJSValue<T3>(context, arguments[3]);
T4 para4 = FromJSValue<T4>(context, arguments[4]);
T5 para5 = FromJSValue<T5>(context, arguments[5]);
T6 para6 = FromJSValue<T6>(context, arguments[6]);
T7 para7 = FromJSValue<T7>(context, arguments[7]);
                arguments[1].AddRef();
arguments[2].AddRef();
arguments[3].AddRef();
arguments[4].AddRef();
arguments[5].AddRef();
arguments[6].AddRef();
arguments[7].AddRef();
                //context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5,para6,para7);
                arguments[1].Release();
arguments[2].Release();
arguments[3].Release();
arguments[4].Release();
arguments[5].Release();
arguments[6].Release();
arguments[7].Release();
                context.RuntimeContext.Enter();//restore context
                return ToJSValue<TResult>(context,result);;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
            {
                return JavaScriptValue.CreateFunction(f);
            }
            );
        }

        

        private Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult> fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(JSValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);
JavaScriptValue p5 = ToJSValue<T5>(context,para5);
JavaScriptValue p6 = ToJSValue<T6>(context,para6);
JavaScriptValue p7 = ToJSValue<T7>(context,para7);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
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
                        resultValue= value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        resultValue= value.CallFunction(context.JSClass,p1,p2,p3,p4,p5,p6,p7);
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
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



    }
}
