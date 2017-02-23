
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using static ChakraCore.NET.API.JavaScriptContext;
namespace ChakraCore.NET
{
public partial class JSValueConverter
{

        public void RegisterMethodConverter()
        {
            if (CanConvert<Action>())
            {
                return;
            }
            RegisterConverter<Action>(toJSMethod, fromJSMethod, false);
        }
        

        //register direct call delegate and callback delegate
        public void RegisterFunctionConverter<TResult>()
        {
            if (!CanConvert<Func<bool,TResult>>())
            {
                RegisterConverter<Func<bool,TResult>>(toJSFunction<TResult>, fromJSFunction<TResult>,false);
            }
            if (!CanConvert<Func<TResult>>())
            {
                RegisterConverter<Func<TResult>>(toJSCallbackFunction<TResult>, fromJSCallbackFunction<TResult>,false);
            }
            
            
        }

        



        public void RegisterMethodConverter<T1>()
        {
            if (CanConvert<Action<T1>>())
            {
                return;
            }
            RegisterConverter<Action<T1>>(toJSMethod<T1>, fromJSMethod<T1>, false);
        }
        

        //register direct call delegate and callback delegate
        public void RegisterFunctionConverter<T1,TResult>()
        {
            if (!CanConvert<Func<bool,T1,TResult>>())
            {
                RegisterConverter<Func<bool,T1,TResult>>(toJSFunction<T1,TResult>, fromJSFunction<T1,TResult>,false);
            }
            if (!CanConvert<Func<T1,TResult>>())
            {
                RegisterConverter<Func<T1,TResult>>(toJSCallbackFunction<T1,TResult>, fromJSCallbackFunction<T1,TResult>,false);
            }
            
            
        }

        



        public void RegisterMethodConverter<T1,T2>()
        {
            if (CanConvert<Action<T1,T2>>())
            {
                return;
            }
            RegisterConverter<Action<T1,T2>>(toJSMethod<T1,T2>, fromJSMethod<T1,T2>, false);
        }
        

        //register direct call delegate and callback delegate
        public void RegisterFunctionConverter<T1,T2,TResult>()
        {
            if (!CanConvert<Func<bool,T1,T2,TResult>>())
            {
                RegisterConverter<Func<bool,T1,T2,TResult>>(toJSFunction<T1,T2,TResult>, fromJSFunction<T1,T2,TResult>,false);
            }
            if (!CanConvert<Func<T1,T2,TResult>>())
            {
                RegisterConverter<Func<T1,T2,TResult>>(toJSCallbackFunction<T1,T2,TResult>, fromJSCallbackFunction<T1,T2,TResult>,false);
            }
            
            
        }

        



        public void RegisterMethodConverter<T1,T2,T3>()
        {
            if (CanConvert<Action<T1,T2,T3>>())
            {
                return;
            }
            RegisterConverter<Action<T1,T2,T3>>(toJSMethod<T1,T2,T3>, fromJSMethod<T1,T2,T3>, false);
        }
        

        //register direct call delegate and callback delegate
        public void RegisterFunctionConverter<T1,T2,T3,TResult>()
        {
            if (!CanConvert<Func<bool,T1,T2,T3,TResult>>())
            {
                RegisterConverter<Func<bool,T1,T2,T3,TResult>>(toJSFunction<T1,T2,T3,TResult>, fromJSFunction<T1,T2,T3,TResult>,false);
            }
            if (!CanConvert<Func<T1,T2,T3,TResult>>())
            {
                RegisterConverter<Func<T1,T2,T3,TResult>>(toJSCallbackFunction<T1,T2,T3,TResult>, fromJSCallbackFunction<T1,T2,T3,TResult>,false);
            }
            
            
        }

        



        public void RegisterMethodConverter<T1,T2,T3,T4>()
        {
            if (CanConvert<Action<T1,T2,T3,T4>>())
            {
                return;
            }
            RegisterConverter<Action<T1,T2,T3,T4>>(toJSMethod<T1,T2,T3,T4>, fromJSMethod<T1,T2,T3,T4>, false);
        }
        

        //register direct call delegate and callback delegate
        public void RegisterFunctionConverter<T1,T2,T3,T4,TResult>()
        {
            if (!CanConvert<Func<bool,T1,T2,T3,T4,TResult>>())
            {
                RegisterConverter<Func<bool,T1,T2,T3,T4,TResult>>(toJSFunction<T1,T2,T3,T4,TResult>, fromJSFunction<T1,T2,T3,T4,TResult>,false);
            }
            if (!CanConvert<Func<T1,T2,T3,T4,TResult>>())
            {
                RegisterConverter<Func<T1,T2,T3,T4,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,TResult>,false);
            }
            
            
        }

        



        public void RegisterMethodConverter<T1,T2,T3,T4,T5>()
        {
            if (CanConvert<Action<T1,T2,T3,T4,T5>>())
            {
                return;
            }
            RegisterConverter<Action<T1,T2,T3,T4,T5>>(toJSMethod<T1,T2,T3,T4,T5>, fromJSMethod<T1,T2,T3,T4,T5>, false);
        }
        

        //register direct call delegate and callback delegate
        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,TResult>()
        {
            if (!CanConvert<Func<bool,T1,T2,T3,T4,T5,TResult>>())
            {
                RegisterConverter<Func<bool,T1,T2,T3,T4,T5,TResult>>(toJSFunction<T1,T2,T3,T4,T5,TResult>, fromJSFunction<T1,T2,T3,T4,T5,TResult>,false);
            }
            if (!CanConvert<Func<T1,T2,T3,T4,T5,TResult>>())
            {
                RegisterConverter<Func<T1,T2,T3,T4,T5,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,TResult>,false);
            }
            
            
        }

        



        public void RegisterMethodConverter<T1,T2,T3,T4,T5,T6>()
        {
            if (CanConvert<Action<T1,T2,T3,T4,T5,T6>>())
            {
                return;
            }
            RegisterConverter<Action<T1,T2,T3,T4,T5,T6>>(toJSMethod<T1,T2,T3,T4,T5,T6>, fromJSMethod<T1,T2,T3,T4,T5,T6>, false);
        }
        

        //register direct call delegate and callback delegate
        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,TResult>()
        {
            if (!CanConvert<Func<bool,T1,T2,T3,T4,T5,T6,TResult>>())
            {
                RegisterConverter<Func<bool,T1,T2,T3,T4,T5,T6,TResult>>(toJSFunction<T1,T2,T3,T4,T5,T6,TResult>, fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>,false);
            }
            if (!CanConvert<Func<T1,T2,T3,T4,T5,T6,TResult>>())
            {
                RegisterConverter<Func<T1,T2,T3,T4,T5,T6,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult>,false);
            }
            
            
        }

        



        public void RegisterMethodConverter<T1,T2,T3,T4,T5,T6,T7>()
        {
            if (CanConvert<Action<T1,T2,T3,T4,T5,T6,T7>>())
            {
                return;
            }
            RegisterConverter<Action<T1,T2,T3,T4,T5,T6,T7>>(toJSMethod<T1,T2,T3,T4,T5,T6,T7>, fromJSMethod<T1,T2,T3,T4,T5,T6,T7>, false);
        }
        

        //register direct call delegate and callback delegate
        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,T7,TResult>()
        {
            if (!CanConvert<Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult>>())
            {
                RegisterConverter<Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult>>(toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>, fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>,false);
            }
            if (!CanConvert<Func<T1,T2,T3,T4,T5,T6,T7,TResult>>())
            {
                RegisterConverter<Func<T1,T2,T3,T4,T5,T6,T7,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult>,false);
            }
            
            
        }

        



    }
}
