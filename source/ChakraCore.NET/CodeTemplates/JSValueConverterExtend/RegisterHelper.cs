
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
            RegisterConverter<Action>(toJSMethod, fromJSMethod, false);
        }

        public void RegisterFunctionConverter<TResult>()
        {
            RegisterConverter<Func<bool,TResult>>(toJSFunction<TResult>, fromJSFunction<TResult>,false);
        }

        public void RegisterCallbackFunctionConverter<TResult>()
        {
            RegisterConverter<Func<TResult>>(toJSCallbackFunction<TResult>, fromJSCallbackFunction<TResult>,false);
        }



        public void RegisterMethodConverter<T1>()
        {
            RegisterConverter<Action<T1>>(toJSMethod<T1>, fromJSMethod<T1>, false);
        }

        public void RegisterFunctionConverter<T1,TResult>()
        {
            RegisterConverter<Func<bool,T1,TResult>>(toJSFunction<T1,TResult>, fromJSFunction<T1,TResult>,false);
        }

        public void RegisterCallbackFunctionConverter<T1,TResult>()
        {
            RegisterConverter<Func<T1,TResult>>(toJSCallbackFunction<T1,TResult>, fromJSCallbackFunction<T1,TResult>,false);
        }



        public void RegisterMethodConverter<T1,T2>()
        {
            RegisterConverter<Action<T1,T2>>(toJSMethod<T1,T2>, fromJSMethod<T1,T2>, false);
        }

        public void RegisterFunctionConverter<T1,T2,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,TResult>>(toJSFunction<T1,T2,TResult>, fromJSFunction<T1,T2,TResult>,false);
        }

        public void RegisterCallbackFunctionConverter<T1,T2,TResult>()
        {
            RegisterConverter<Func<T1,T2,TResult>>(toJSCallbackFunction<T1,T2,TResult>, fromJSCallbackFunction<T1,T2,TResult>,false);
        }



        public void RegisterMethodConverter<T1,T2,T3>()
        {
            RegisterConverter<Action<T1,T2,T3>>(toJSMethod<T1,T2,T3>, fromJSMethod<T1,T2,T3>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,TResult>>(toJSFunction<T1,T2,T3,TResult>, fromJSFunction<T1,T2,T3,TResult>,false);
        }

        public void RegisterCallbackFunctionConverter<T1,T2,T3,TResult>()
        {
            RegisterConverter<Func<T1,T2,T3,TResult>>(toJSCallbackFunction<T1,T2,T3,TResult>, fromJSCallbackFunction<T1,T2,T3,TResult>,false);
        }



        public void RegisterMethodConverter<T1,T2,T3,T4>()
        {
            RegisterConverter<Action<T1,T2,T3,T4>>(toJSMethod<T1,T2,T3,T4>, fromJSMethod<T1,T2,T3,T4>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,T4,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,T4,TResult>>(toJSFunction<T1,T2,T3,T4,TResult>, fromJSFunction<T1,T2,T3,T4,TResult>,false);
        }

        public void RegisterCallbackFunctionConverter<T1,T2,T3,T4,TResult>()
        {
            RegisterConverter<Func<T1,T2,T3,T4,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,TResult>,false);
        }



        public void RegisterMethodConverter<T1,T2,T3,T4,T5>()
        {
            RegisterConverter<Action<T1,T2,T3,T4,T5>>(toJSMethod<T1,T2,T3,T4,T5>, fromJSMethod<T1,T2,T3,T4,T5>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,T4,T5,TResult>>(toJSFunction<T1,T2,T3,T4,T5,TResult>, fromJSFunction<T1,T2,T3,T4,T5,TResult>,false);
        }

        public void RegisterCallbackFunctionConverter<T1,T2,T3,T4,T5,TResult>()
        {
            RegisterConverter<Func<T1,T2,T3,T4,T5,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,TResult>,false);
        }



        public void RegisterMethodConverter<T1,T2,T3,T4,T5,T6>()
        {
            RegisterConverter<Action<T1,T2,T3,T4,T5,T6>>(toJSMethod<T1,T2,T3,T4,T5,T6>, fromJSMethod<T1,T2,T3,T4,T5,T6>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,T4,T5,T6,TResult>>(toJSFunction<T1,T2,T3,T4,T5,T6,TResult>, fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>,false);
        }

        public void RegisterCallbackFunctionConverter<T1,T2,T3,T4,T5,T6,TResult>()
        {
            RegisterConverter<Func<T1,T2,T3,T4,T5,T6,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult>,false);
        }



        public void RegisterMethodConverter<T1,T2,T3,T4,T5,T6,T7>()
        {
            RegisterConverter<Action<T1,T2,T3,T4,T5,T6,T7>>(toJSMethod<T1,T2,T3,T4,T5,T6,T7>, fromJSMethod<T1,T2,T3,T4,T5,T6,T7>, false);
        }

        public void RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,T7,TResult>()
        {
            RegisterConverter<Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult>>(toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>, fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>,false);
        }

        public void RegisterCallbackFunctionConverter<T1,T2,T3,T4,T5,T6,T7,TResult>()
        {
            RegisterConverter<Func<T1,T2,T3,T4,T5,T6,T7,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult>,false);
        }



    }
}
