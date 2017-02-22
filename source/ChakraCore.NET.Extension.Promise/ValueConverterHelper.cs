using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET.Extension.Promise
{
    public static class ValueConverterHelper
    {
        public static void RegisterTask<TResult>(this JSValueConverter converter)
        {
            //register internal call back types
            converter.RegisterFunctionConverter<TResult>();
            converter.RegisterMethodConverter<TResult>();
            converter.RegisterMethodConverter<string>();
            converter.RegisterFunctionConverter<JavaScriptValue>();
            converter.RegisterMethodConverter<Action<TResult>, Action<string>>();

            converter.RegisterConverter<Task<TResult>>(
                (context, value) =>
                {
                    throw new NotImplementedException();
                },
                (context, value) =>
                {
                    //from a promise 
                    return Task.Factory.FromAsync<TResult>(

                        (callback, state) =>
                        {
                            return BeginMethod<TResult>(value, context, callback, state);
                        }
                        , EndMethod<TResult>, null
                        );


                }, false
                );
        }

        private static IAsyncResult BeginMethod<T>(JavaScriptValue func, JSValueConvertContext context, AsyncCallback callback, object state)
        {
            AsyncResult<T> result = new AsyncResult<T>();
            Action<T> fullfilledCallback = (x) =>
            {
                result.SetResult(x);
                callback(result);
            };
            Action<string> rejectedCallback = (s) =>
            {
                result.SetResult(default(T));
                throw new PromiseRejectedException(s);
            };

            Func<JavaScriptValue> promiseCall = context.RuntimeContext.ValueConverter.FromJSValue<Func<JavaScriptValue>>(context, func);// the target function which returns a promise object
            //Task.Factory.StartNew(() =>
            //{
                JavaScriptValue promiseObject = promiseCall();//call the function
                JavaScriptValue thenCall = context.RuntimeContext.With<JavaScriptValue>(
                    () =>
                    {
                        return promiseObject.GetProperty(JavaScriptPropertyId.FromString("then"));
                    }
                    );
                JavaScriptValue p1 = context.RuntimeContext.ValueConverter.ToJSValue<Action<T>>(context, fullfilledCallback);
                JavaScriptValue p2 = context.RuntimeContext.ValueConverter.ToJSValue<Action<string>>(context, rejectedCallback);

                context.RuntimeContext.With(
                    () =>
                    {
                        System.Diagnostics.Debug.WriteLine("[Then] called");
                        thenCall.CallFunction(promiseObject, p1, p2);
                    }

                    );
            //});
            
            return result;
        }


        private static T EndMethod<T>(IAsyncResult result)
        {
            return (result as AsyncResult<T>).Result;
        }


    }
}
