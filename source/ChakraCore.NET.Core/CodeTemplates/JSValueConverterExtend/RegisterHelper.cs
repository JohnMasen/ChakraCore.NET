
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
namespace ChakraCore.NET
{
public static partial class JSValueConverterHelper
{

        public static void RegisterMethodConverter(this IJSValueConverterService service)
        {
            if (service.CanConvert<Action>())
            {
                return;
            }
            service.RegisterConverter<Action>(toJSMethod, fromJSMethod, false);
        }
        

        //register direct call delegate and callback delegate
        public static void RegisterFunctionConverter<TResult>(this IJSValueConverterService service)
        {
            if (!service.CanConvert<Func<bool,TResult>>())
            {
                service.RegisterConverter<Func<bool,TResult>>(toJSFunction<TResult>, fromJSFunction<TResult>,false);
            }
            if (!service.CanConvert<Func<TResult>>())
            {
                service.RegisterConverter<Func<TResult>>(toJSCallbackFunction<TResult>, fromJSCallbackFunction<TResult>,false);
            }
            
            
        }

        



        public static void RegisterMethodConverter<T1>(this IJSValueConverterService service)
        {
            if (service.CanConvert<Action<T1>>())
            {
                return;
            }
            service.RegisterConverter<Action<T1>>(toJSMethod<T1>, fromJSMethod<T1>, false);
        }
        

        //register direct call delegate and callback delegate
        public static void RegisterFunctionConverter<T1,TResult>(this IJSValueConverterService service)
        {
            if (!service.CanConvert<Func<bool,T1,TResult>>())
            {
                service.RegisterConverter<Func<bool,T1,TResult>>(toJSFunction<T1,TResult>, fromJSFunction<T1,TResult>,false);
            }
            if (!service.CanConvert<Func<T1,TResult>>())
            {
                service.RegisterConverter<Func<T1,TResult>>(toJSCallbackFunction<T1,TResult>, fromJSCallbackFunction<T1,TResult>,false);
            }
            
            
        }

        



        public static void RegisterMethodConverter<T1,T2>(this IJSValueConverterService service)
        {
            if (service.CanConvert<Action<T1,T2>>())
            {
                return;
            }
            service.RegisterConverter<Action<T1,T2>>(toJSMethod<T1,T2>, fromJSMethod<T1,T2>, false);
        }
        

        //register direct call delegate and callback delegate
        public static void RegisterFunctionConverter<T1,T2,TResult>(this IJSValueConverterService service)
        {
            if (!service.CanConvert<Func<bool,T1,T2,TResult>>())
            {
                service.RegisterConverter<Func<bool,T1,T2,TResult>>(toJSFunction<T1,T2,TResult>, fromJSFunction<T1,T2,TResult>,false);
            }
            if (!service.CanConvert<Func<T1,T2,TResult>>())
            {
                service.RegisterConverter<Func<T1,T2,TResult>>(toJSCallbackFunction<T1,T2,TResult>, fromJSCallbackFunction<T1,T2,TResult>,false);
            }
            
            
        }

        



        public static void RegisterMethodConverter<T1,T2,T3>(this IJSValueConverterService service)
        {
            if (service.CanConvert<Action<T1,T2,T3>>())
            {
                return;
            }
            service.RegisterConverter<Action<T1,T2,T3>>(toJSMethod<T1,T2,T3>, fromJSMethod<T1,T2,T3>, false);
        }
        

        //register direct call delegate and callback delegate
        public static void RegisterFunctionConverter<T1,T2,T3,TResult>(this IJSValueConverterService service)
        {
            if (!service.CanConvert<Func<bool,T1,T2,T3,TResult>>())
            {
                service.RegisterConverter<Func<bool,T1,T2,T3,TResult>>(toJSFunction<T1,T2,T3,TResult>, fromJSFunction<T1,T2,T3,TResult>,false);
            }
            if (!service.CanConvert<Func<T1,T2,T3,TResult>>())
            {
                service.RegisterConverter<Func<T1,T2,T3,TResult>>(toJSCallbackFunction<T1,T2,T3,TResult>, fromJSCallbackFunction<T1,T2,T3,TResult>,false);
            }
            
            
        }

        



        public static void RegisterMethodConverter<T1,T2,T3,T4>(this IJSValueConverterService service)
        {
            if (service.CanConvert<Action<T1,T2,T3,T4>>())
            {
                return;
            }
            service.RegisterConverter<Action<T1,T2,T3,T4>>(toJSMethod<T1,T2,T3,T4>, fromJSMethod<T1,T2,T3,T4>, false);
        }
        

        //register direct call delegate and callback delegate
        public static void RegisterFunctionConverter<T1,T2,T3,T4,TResult>(this IJSValueConverterService service)
        {
            if (!service.CanConvert<Func<bool,T1,T2,T3,T4,TResult>>())
            {
                service.RegisterConverter<Func<bool,T1,T2,T3,T4,TResult>>(toJSFunction<T1,T2,T3,T4,TResult>, fromJSFunction<T1,T2,T3,T4,TResult>,false);
            }
            if (!service.CanConvert<Func<T1,T2,T3,T4,TResult>>())
            {
                service.RegisterConverter<Func<T1,T2,T3,T4,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,TResult>,false);
            }
            
            
        }

        



        public static void RegisterMethodConverter<T1,T2,T3,T4,T5>(this IJSValueConverterService service)
        {
            if (service.CanConvert<Action<T1,T2,T3,T4,T5>>())
            {
                return;
            }
            service.RegisterConverter<Action<T1,T2,T3,T4,T5>>(toJSMethod<T1,T2,T3,T4,T5>, fromJSMethod<T1,T2,T3,T4,T5>, false);
        }
        

        //register direct call delegate and callback delegate
        public static void RegisterFunctionConverter<T1,T2,T3,T4,T5,TResult>(this IJSValueConverterService service)
        {
            if (!service.CanConvert<Func<bool,T1,T2,T3,T4,T5,TResult>>())
            {
                service.RegisterConverter<Func<bool,T1,T2,T3,T4,T5,TResult>>(toJSFunction<T1,T2,T3,T4,T5,TResult>, fromJSFunction<T1,T2,T3,T4,T5,TResult>,false);
            }
            if (!service.CanConvert<Func<T1,T2,T3,T4,T5,TResult>>())
            {
                service.RegisterConverter<Func<T1,T2,T3,T4,T5,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,TResult>,false);
            }
            
            
        }

        



        public static void RegisterMethodConverter<T1,T2,T3,T4,T5,T6>(this IJSValueConverterService service)
        {
            if (service.CanConvert<Action<T1,T2,T3,T4,T5,T6>>())
            {
                return;
            }
            service.RegisterConverter<Action<T1,T2,T3,T4,T5,T6>>(toJSMethod<T1,T2,T3,T4,T5,T6>, fromJSMethod<T1,T2,T3,T4,T5,T6>, false);
        }
        

        //register direct call delegate and callback delegate
        public static void RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,TResult>(this IJSValueConverterService service)
        {
            if (!service.CanConvert<Func<bool,T1,T2,T3,T4,T5,T6,TResult>>())
            {
                service.RegisterConverter<Func<bool,T1,T2,T3,T4,T5,T6,TResult>>(toJSFunction<T1,T2,T3,T4,T5,T6,TResult>, fromJSFunction<T1,T2,T3,T4,T5,T6,TResult>,false);
            }
            if (!service.CanConvert<Func<T1,T2,T3,T4,T5,T6,TResult>>())
            {
                service.RegisterConverter<Func<T1,T2,T3,T4,T5,T6,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,TResult>,false);
            }
            
            
        }

        



        public static void RegisterMethodConverter<T1,T2,T3,T4,T5,T6,T7>(this IJSValueConverterService service)
        {
            if (service.CanConvert<Action<T1,T2,T3,T4,T5,T6,T7>>())
            {
                return;
            }
            service.RegisterConverter<Action<T1,T2,T3,T4,T5,T6,T7>>(toJSMethod<T1,T2,T3,T4,T5,T6,T7>, fromJSMethod<T1,T2,T3,T4,T5,T6,T7>, false);
        }
        

        //register direct call delegate and callback delegate
        public static void RegisterFunctionConverter<T1,T2,T3,T4,T5,T6,T7,TResult>(this IJSValueConverterService service)
        {
            if (!service.CanConvert<Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult>>())
            {
                service.RegisterConverter<Func<bool,T1,T2,T3,T4,T5,T6,T7,TResult>>(toJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>, fromJSFunction<T1,T2,T3,T4,T5,T6,T7,TResult>,false);
            }
            if (!service.CanConvert<Func<T1,T2,T3,T4,T5,T6,T7,TResult>>())
            {
                service.RegisterConverter<Func<T1,T2,T3,T4,T5,T6,T7,TResult>>(toJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult>, fromJSCallbackFunction<T1,T2,T3,T4,T5,T6,T7,TResult>,false);
            }
            
            
        }

        



    }
}
