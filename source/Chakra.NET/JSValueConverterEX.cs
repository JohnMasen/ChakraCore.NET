
using Chakra.NET.API;
using System;
using System.Collections.Generic;
using static Chakra.NET.API.JavaScriptContext;
namespace Chakra.NET
{
public partial class JSValueConverter
{
            private JavaScriptValue toJSMethod (ValueConvertContext context, Action a)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 1)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                a();

                context.RuntimeContext.Enter();//restore context
                return context.RuntimeContext.JSValue_Undefined;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    return JavaScriptValue.CreateFunction(f);
                }
            );
        }
        
        private JavaScriptValue toJSFunction<TResult> (ValueConvertContext context, Func<bool,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 1)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

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

        private Action fromJSMethod(ValueConvertContext context,JavaScriptValue value)
        {
            Action result = () =>
              {

                  
                  

                  context.RuntimeContext.With(()=>
                  {
                      value.CallFunction(context.JSClass);
                  });
              };
            return result;
        }

        private Func<bool,TResult> fromJSFunction<TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,TResult> result = (bool isConstruct) =>
            {
                

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    if (isConstruct)
                    {
                        return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        return value.CallFunction(context.JSClass);
                    }
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



        public void RegisterMethodConverter()
        {
            RegisterConverter<Action>(toJSMethod, fromJSMethod, false);
        }

        public void RegisterFunctionConverter<TResult>()
        {
            RegisterConverter<Func<bool,TResult>>(toJSFunction<TResult>, fromJSFunction<TResult>,false);
        }





        
              private JavaScriptValue toJSMethod<T1> (ValueConvertContext context, Action<T1> a)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 2)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                a(para1);

                context.RuntimeContext.Enter();//restore context
                return context.RuntimeContext.JSValue_Undefined;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    return JavaScriptValue.CreateFunction(f);
                }
            );
        }
        
        private JavaScriptValue toJSFunction<T1,TResult> (ValueConvertContext context, Func<bool,T1,TResult> callback)
        {
            JavaScriptNativeFunction f = (callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                if (argumentCount != 2)
                {
                    throw new InvalidOperationException("call from javascript did not pass enough parameters");
                }
                //context.JSClass = arguments[0];//put the caller object to context
                T1 para1 = FromJSValue<T1>(context, arguments[1]);

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1);

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

        private Action<T1> fromJSMethod<T1>(ValueConvertContext context,JavaScriptValue value)
        {
            Action<T1> result = (T1 para1) =>
              {

                  JavaScriptValue p1 = ToJSValue<T1>(context,para1);
                  

                  context.RuntimeContext.With(()=>
                  {
                      value.CallFunction(context.JSClass,p1);
                  });
              };
            return result;
        }

        private Func<bool,T1,TResult> fromJSFunction<T1,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,TResult> result = (bool isConstruct,T1 para1) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    if (isConstruct)
                    {
                        return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        return value.CallFunction(context.JSClass,p1);
                    }
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



        public void RegisterMethodConverter<T1>()
        {
            RegisterConverter<Action<T1>>(toJSMethod<T1>, fromJSMethod<T1>, false);
        }

        public void RegisterFunctionConverter<T1,TResult>()
        {
            RegisterConverter<Func<bool,T1,TResult>>(toJSFunction<T1,TResult>, fromJSFunction<T1,TResult>,false);
        }





        
              private JavaScriptValue toJSMethod<T1,T2> (ValueConvertContext context, Action<T1,T2> a)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                a(para1,para2);

                context.RuntimeContext.Enter();//restore context
                return context.RuntimeContext.JSValue_Undefined;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    return JavaScriptValue.CreateFunction(f);
                }
            );
        }
        
        private JavaScriptValue toJSFunction<T1,T2,TResult> (ValueConvertContext context, Func<bool,T1,T2,TResult> callback)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2);

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

        private Action<T1,T2> fromJSMethod<T1,T2>(ValueConvertContext context,JavaScriptValue value)
        {
            Action<T1,T2> result = (T1 para1,T2 para2) =>
              {

                  JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
                  

                  context.RuntimeContext.With(()=>
                  {
                      value.CallFunction(context.JSClass,p1,p2);
                  });
              };
            return result;
        }

        private Func<bool,T1,T2,TResult> fromJSFunction<T1,T2,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,TResult> result = (bool isConstruct,T1 para1,T2 para2) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    if (isConstruct)
                    {
                        return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        return value.CallFunction(context.JSClass,p1,p2);
                    }
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



        public void RegisterMethodConverter<T1,T2>()
        {
            RegisterConverter<Action<T1,T2>>(toJSMethod<T1,T2>, fromJSMethod<T1,T2>, false);
        }

        public void RegisterFunctionConverter<T1,T2,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,TResult>>(toJSFunction<T1,T2,TResult>, fromJSFunction<T1,T2,TResult>,false);
        }





        
              private JavaScriptValue toJSMethod<T1,T2,T3> (ValueConvertContext context, Action<T1,T2,T3> a)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                a(para1,para2,para3);

                context.RuntimeContext.Enter();//restore context
                return context.RuntimeContext.JSValue_Undefined;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    return JavaScriptValue.CreateFunction(f);
                }
            );
        }
        
        private JavaScriptValue toJSFunction<T1,T2,T3,TResult> (ValueConvertContext context, Func<bool,T1,T2,T3,TResult> callback)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3);

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

        private Action<T1,T2,T3> fromJSMethod<T1,T2,T3>(ValueConvertContext context,JavaScriptValue value)
        {
            Action<T1,T2,T3> result = (T1 para1,T2 para2,T3 para3) =>
              {

                  JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
                  

                  context.RuntimeContext.With(()=>
                  {
                      value.CallFunction(context.JSClass,p1,p2,p3);
                  });
              };
            return result;
        }

        private Func<bool,T1,T2,T3,TResult> fromJSFunction<T1,T2,T3,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,T3,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    if (isConstruct)
                    {
                        return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        return value.CallFunction(context.JSClass,p1,p2,p3);
                    }
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



        public void RegisterMethodConverter<T1,T2,T3>()
        {
            RegisterConverter<Action<T1,T2,T3>>(toJSMethod<T1,T2,T3>, fromJSMethod<T1,T2,T3>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,TResult>>(toJSFunction<T1,T2,T3,TResult>, fromJSFunction<T1,T2,T3,TResult>,false);
        }





        
              private JavaScriptValue toJSMethod<T1,T2,T3,T4> (ValueConvertContext context, Action<T1,T2,T3,T4> a)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                a(para1,para2,para3,para4);

                context.RuntimeContext.Enter();//restore context
                return context.RuntimeContext.JSValue_Undefined;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    return JavaScriptValue.CreateFunction(f);
                }
            );
        }
        
        private JavaScriptValue toJSFunction<T1,T2,T3,T4,TResult> (ValueConvertContext context, Func<bool,T1,T2,T3,T4,TResult> callback)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3,para4);

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

        private Action<T1,T2,T3,T4> fromJSMethod<T1,T2,T3,T4>(ValueConvertContext context,JavaScriptValue value)
        {
            Action<T1,T2,T3,T4> result = (T1 para1,T2 para2,T3 para3,T4 para4) =>
              {

                  JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);
                  

                  context.RuntimeContext.With(()=>
                  {
                      value.CallFunction(context.JSClass,p1,p2,p3,p4);
                  });
              };
            return result;
        }

        private Func<bool,T1,T2,T3,T4,TResult> fromJSFunction<T1,T2,T3,T4,TResult>(ValueConvertContext context, JavaScriptValue value)
        {
            Func<bool,T1,T2,T3,T4,TResult> result = (bool isConstruct,T1 para1,T2 para2,T3 para3,T4 para4) =>
            {
                JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);

                JavaScriptValue r=context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    if (isConstruct)
                    {
                        return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        return value.CallFunction(context.JSClass,p1,p2,p3,p4);
                    }
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



        public void RegisterMethodConverter<T1,T2,T3,T4>()
        {
            RegisterConverter<Action<T1,T2,T3,T4>>(toJSMethod<T1,T2,T3,T4>, fromJSMethod<T1,T2,T3,T4>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,T4,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,T4,TResult>>(toJSFunction<T1,T2,T3,T4,TResult>, fromJSFunction<T1,T2,T3,T4,TResult>,false);
        }





        
              private JavaScriptValue toJSMethod<T1,T2,T3,T4,T5> (ValueConvertContext context, Action<T1,T2,T3,T4,T5> a)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                a(para1,para2,para3,para4,para5);

                context.RuntimeContext.Enter();//restore context
                return context.RuntimeContext.JSValue_Undefined;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    return JavaScriptValue.CreateFunction(f);
                }
            );
        }
        
        private JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,TResult> (ValueConvertContext context, Func<bool,T1,T2,T3,T4,T5,TResult> callback)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5);

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

        private Action<T1,T2,T3,T4,T5> fromJSMethod<T1,T2,T3,T4,T5>(ValueConvertContext context,JavaScriptValue value)
        {
            Action<T1,T2,T3,T4,T5> result = (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5) =>
              {

                  JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);
JavaScriptValue p5 = ToJSValue<T5>(context,para5);
                  

                  context.RuntimeContext.With(()=>
                  {
                      value.CallFunction(context.JSClass,p1,p2,p3,p4,p5);
                  });
              };
            return result;
        }

        private Func<bool,T1,T2,T3,T4,T5,TResult> fromJSFunction<T1,T2,T3,T4,T5,TResult>(ValueConvertContext context, JavaScriptValue value)
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
                    if (isConstruct)
                    {
                        return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        return value.CallFunction(context.JSClass,p1,p2,p3,p4,p5);
                    }
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



        public void RegisterMethodConverter<T1,T2,T3,T4,T5>()
        {
            RegisterConverter<Action<T1,T2,T3,T4,T5>>(toJSMethod<T1,T2,T3,T4,T5>, fromJSMethod<T1,T2,T3,T4,T5>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,T4,T5,TResult>>(toJSFunction<T1,T2,T3,T4,T5,TResult>, fromJSFunction<T1,T2,T3,T4,T5,TResult>,false);
        }





        
              private JavaScriptValue toJSMethod<T1,T2,T3,T4,T5,T6> (ValueConvertContext context, Action<T1,T2,T3,T4,T5,T6> a)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                a(para1,para2,para3,para4,para5,para6);

                context.RuntimeContext.Enter();//restore context
                return context.RuntimeContext.JSValue_Undefined;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    return JavaScriptValue.CreateFunction(f);
                }
            );
        }
        
        private JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,T6,TResult> (ValueConvertContext context, Func<bool,T1,T2,T3,T4,T5,T6,TResult> callback)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5,para6);

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

        private Action<T1,T2,T3,T4,T5,T6> fromJSMethod<T1,T2,T3,T4,T5,T6>(ValueConvertContext context,JavaScriptValue value)
        {
            Action<T1,T2,T3,T4,T5,T6> result = (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6) =>
              {

                  JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);
JavaScriptValue p5 = ToJSValue<T5>(context,para5);
JavaScriptValue p6 = ToJSValue<T6>(context,para6);
                  

                  context.RuntimeContext.With(()=>
                  {
                      value.CallFunction(context.JSClass,p1,p2,p3,p4,p5,p6);
                  });
              };
            return result;
        }

        private Func<bool,T1,T2,T3,T4,T5,T6,TResult> fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>(ValueConvertContext context, JavaScriptValue value)
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
                    if (isConstruct)
                    {
                        return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        return value.CallFunction(context.JSClass,p1,p2,p3,p4,p5,p6);
                    }
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



        public void RegisterMethodConverter<T1,T2,T3,T4,T5,T6>()
        {
            RegisterConverter<Action<T1,T2,T3,T4,T5,T6>>(toJSMethod<T1,T2,T3,T4,T5,T6>, fromJSMethod<T1,T2,T3,T4,T5,T6>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,T4,T5,T6,TResult>>(toJSFunction<T1,T2,T3,T4,T5,T6,TResult>, fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>,false);
        }





        
              private JavaScriptValue toJSMethod<T1,T2,T3,T4,T5,T6,T7> (ValueConvertContext context, Action<T1,T2,T3,T4,T5,T6,T7> a)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                a(para1,para2,para3,para4,para5,para6,para7);

                context.RuntimeContext.Enter();//restore context
                return context.RuntimeContext.JSValue_Undefined;
            };
            context.Handler.Hold(f);

            return context.RuntimeContext.With<JavaScriptValue>(()=>
                {
                    return JavaScriptValue.CreateFunction(f);
                }
            );
        }
        
        private JavaScriptValue toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult> (ValueConvertContext context, Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult> callback)
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

                context.RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.

                TResult result=callback(isConstructCall,para1,para2,para3,para4,para5,para6,para7);

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

        private Action<T1,T2,T3,T4,T5,T6,T7> fromJSMethod<T1,T2,T3,T4,T5,T6,T7>(ValueConvertContext context,JavaScriptValue value)
        {
            Action<T1,T2,T3,T4,T5,T6,T7> result = (T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7) =>
              {

                  JavaScriptValue p1 = ToJSValue<T1>(context,para1);
JavaScriptValue p2 = ToJSValue<T2>(context,para2);
JavaScriptValue p3 = ToJSValue<T3>(context,para3);
JavaScriptValue p4 = ToJSValue<T4>(context,para4);
JavaScriptValue p5 = ToJSValue<T5>(context,para5);
JavaScriptValue p6 = ToJSValue<T6>(context,para6);
JavaScriptValue p7 = ToJSValue<T7>(context,para7);
                  

                  context.RuntimeContext.With(()=>
                  {
                      value.CallFunction(context.JSClass,p1,p2,p3,p4,p5,p6,p7);
                  });
              };
            return result;
        }

        private Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult> fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>(ValueConvertContext context, JavaScriptValue value)
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
                    if (isConstruct)
                    {
                        return value.ConstructObject(context.RuntimeContext.JSValue_Undefined);
                    }
                    else
                    {
                        return value.CallFunction(context.JSClass,p1,p2,p3,p4,p5,p6,p7);
                    }
                });
                return FromJSValue<TResult>(context,r);
            };
            return result;
        }



        public void RegisterMethodConverter<T1,T2,T3,T4,T5,T6,T7>()
        {
            RegisterConverter<Action<T1,T2,T3,T4,T5,T6,T7>>(toJSMethod<T1,T2,T3,T4,T5,T6,T7>, fromJSMethod<T1,T2,T3,T4,T5,T6,T7>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,T7,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult>>(toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>, fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>,false);
        }





        
          
}
}
