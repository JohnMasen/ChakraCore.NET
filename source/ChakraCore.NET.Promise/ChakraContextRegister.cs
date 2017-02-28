using ChakraCore.NET.API;
using ChakraCore.NET.Promise;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET
{
    public static class ChakraContextRegister
    {

        public static void RegisterTask<TResult>(this ChakraContext target)
        {
            PromiseCallbackPairService service = new PromiseCallbackPairService();
            target.ServiceNode.PushService(service);

            var converter = target.ServiceNode.GetService<IJSValueConverterService>();
            if (converter.CanConvert<Task<TResult>>())
            {
                return;
            }
            //register internal call back types
            converter.RegisterFunctionConverter<TResult>();
            converter.RegisterMethodConverter<TResult>();
            converter.RegisterMethodConverter<string>();
            converter.RegisterFunctionConverter<JSValue>();
            converter.RegisterMethodConverter<Action<TResult>, Action<string>>();


            converter.RegisterConverter<Task<TResult>>(
                (node, value) =>
                {
                    //convert resolve, reject
                    Action<Action<TResult>, Action<string>> promiseBody = async (resolve, reject) =>
                       {
                           try
                           {
                               var result = await value;
                               resolve(result);
                           }
                           catch (PromiseRejectedException ex)
                           {
                               reject(ex.ToString());
                           }
                           catch (Exception)
                           {
                               throw;
                           }
                           
                       };
                    return target.GlobalObject.CallFunction<Action<Action<TResult>, Action<string>>, JavaScriptValue>("Promise", promiseBody, true);
                },
                (node, value) =>
                {
                    //from a promise 
                    return Task.Factory.FromAsync<TResult>(

                        (callback, state) =>
                        {
                            return BeginMethod<TResult>(value, node, callback, state);
                        }
                        , EndMethod<TResult>, null
                        );


                }, false
                );
        }

        private static IAsyncResult BeginMethod<T>(JavaScriptValue func, IServiceNode node, AsyncCallback callback, object state)
        {
            var conveter = node.GetService<IJSValueConverterService>();
            var jsValueService = node.GetService<IJSValueService>();
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

            Func<JSValue> promiseCall = conveter.FromJSValue<Func<JSValue>>(func);// the target function which returns a promise object
            JSValue promiseObject = promiseCall();//call the function,get the Promise object
            var service=promiseObject.ServiceNode.GetService<PromiseCallbackPairService>();//get the delegate interceptor
            service.Begin();//enable the interceptor
            promiseObject.CallMethod<Action<T>, Action<String>>("then", fullfilledCallback, rejectedCallback);
            System.Diagnostics.Debug.WriteLine("[Then] called");
            service.End();//disable the interceptor
            return result;
        }


        private static T EndMethod<T>(IAsyncResult result)
        {
            return (result as AsyncResult<T>).Result;
        }

        
    }
}
