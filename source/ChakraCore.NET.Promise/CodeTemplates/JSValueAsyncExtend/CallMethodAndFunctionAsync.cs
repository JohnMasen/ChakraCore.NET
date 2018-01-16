

using System;
using System.Collections.Generic;
using ChakraCore.NET.API;
using System.Threading.Tasks;
namespace ChakraCore.NET
{
public static class JSValueAsyncExtend
{
        public static Task CallMethodAsync(this JSValue value, string name )
        {
            return value.CallFunction<Task>(name);
        }

        public static Task<TResult> CallFunctionAsync<TResult>(this JSValue value,string name )
        {
			value.ServiceNode.GetService<IJSValueConverterService>().RegisterTask<TResult>();
            return value.CallFunction<Task<TResult>>(name);
        }



        public static Task CallMethodAsync<T1>(this JSValue value, string name ,T1 para1)
        {
            return value.CallFunction<T1,Task>(name,para1);
        }

        public static Task<TResult> CallFunctionAsync<T1,TResult>(this JSValue value,string name ,T1 para1)
        {
			value.ServiceNode.GetService<IJSValueConverterService>().RegisterTask<TResult>();
            return value.CallFunction<T1,Task<TResult>>(name,para1);
        }



        public static Task CallMethodAsync<T1,T2>(this JSValue value, string name ,T1 para1,T2 para2)
        {
            return value.CallFunction<T1,T2,Task>(name,para1,para2);
        }

        public static Task<TResult> CallFunctionAsync<T1,T2,TResult>(this JSValue value,string name ,T1 para1,T2 para2)
        {
			value.ServiceNode.GetService<IJSValueConverterService>().RegisterTask<TResult>();
            return value.CallFunction<T1,T2,Task<TResult>>(name,para1,para2);
        }



        public static Task CallMethodAsync<T1,T2,T3>(this JSValue value, string name ,T1 para1,T2 para2,T3 para3)
        {
            return value.CallFunction<T1,T2,T3,Task>(name,para1,para2,para3);
        }

        public static Task<TResult> CallFunctionAsync<T1,T2,T3,TResult>(this JSValue value,string name ,T1 para1,T2 para2,T3 para3)
        {
			value.ServiceNode.GetService<IJSValueConverterService>().RegisterTask<TResult>();
            return value.CallFunction<T1,T2,T3,Task<TResult>>(name,para1,para2,para3);
        }



        public static Task CallMethodAsync<T1,T2,T3,T4>(this JSValue value, string name ,T1 para1,T2 para2,T3 para3,T4 para4)
        {
            return value.CallFunction<T1,T2,T3,T4,Task>(name,para1,para2,para3,para4);
        }

        public static Task<TResult> CallFunctionAsync<T1,T2,T3,T4,TResult>(this JSValue value,string name ,T1 para1,T2 para2,T3 para3,T4 para4)
        {
			value.ServiceNode.GetService<IJSValueConverterService>().RegisterTask<TResult>();
            return value.CallFunction<T1,T2,T3,T4,Task<TResult>>(name,para1,para2,para3,para4);
        }



        public static Task CallMethodAsync<T1,T2,T3,T4,T5>(this JSValue value, string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5)
        {
            return value.CallFunction<T1,T2,T3,T4,T5,Task>(name,para1,para2,para3,para4,para5);
        }

        public static Task<TResult> CallFunctionAsync<T1,T2,T3,T4,T5,TResult>(this JSValue value,string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5)
        {
			value.ServiceNode.GetService<IJSValueConverterService>().RegisterTask<TResult>();
            return value.CallFunction<T1,T2,T3,T4,T5,Task<TResult>>(name,para1,para2,para3,para4,para5);
        }



        public static Task CallMethodAsync<T1,T2,T3,T4,T5,T6>(this JSValue value, string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6)
        {
            return value.CallFunction<T1,T2,T3,T4,T5,T6,Task>(name,para1,para2,para3,para4,para5,para6);
        }

        public static Task<TResult> CallFunctionAsync<T1,T2,T3,T4,T5,T6,TResult>(this JSValue value,string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6)
        {
			value.ServiceNode.GetService<IJSValueConverterService>().RegisterTask<TResult>();
            return value.CallFunction<T1,T2,T3,T4,T5,T6,Task<TResult>>(name,para1,para2,para3,para4,para5,para6);
        }



        public static Task CallMethodAsync<T1,T2,T3,T4,T5,T6,T7>(this JSValue value, string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7)
        {
            return value.CallFunction<T1,T2,T3,T4,T5,T6,T7,Task>(name,para1,para2,para3,para4,para5,para6,para7);
        }

        public static Task<TResult> CallFunctionAsync<T1,T2,T3,T4,T5,T6,T7,TResult>(this JSValue value,string name ,T1 para1,T2 para2,T3 para3,T4 para4,T5 para5,T6 para6,T7 para7)
        {
			value.ServiceNode.GetService<IJSValueConverterService>().RegisterTask<TResult>();
            return value.CallFunction<T1,T2,T3,T4,T5,T6,T7,Task<TResult>>(name,para1,para2,para3,para4,para5,para6,para7);
        }




    }
}
