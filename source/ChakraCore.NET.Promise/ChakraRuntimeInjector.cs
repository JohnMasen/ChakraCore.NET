using ChakraCore.NET.API;
using ChakraCore.NET.Promise;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET
{
    public static class ChakraRuntimeInjector
    {

        public static void InjectTaskService(this ChakraRuntime target)
        {
            if (!target.ServiceNode.CanGetService< PromiseCallbackPairService>())
            {
                PromiseCallbackPairService service = new PromiseCallbackPairService();
                target.ServiceNode.PushService(service);
            }
        }

        public static void RegisterTask(this IJSValueConverterService target)
        {
            if (target.CanConvert<Task>())
            {
                return;
            }
            //register internal call back types
            target.RegisterMethodConverter<string>();
            target.RegisterFunctionConverter<JSValue>();
            target.RegisterMethodConverter<Action, Action<string>>();


            target.RegisterConverter<Task>(
                (node, value) =>
                {
                    //convert resolve, reject
                    Action<Action, Action<string>> promiseBody = async (resolve, reject) =>
                    {
                        try
                        {
                            await value;
                            resolve();
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
                    var jsValueService = node.GetService<IJSValueService>();
                    var globalObject = jsValueService.JSGlobalObject;
                    var jsGlobalObject = new JSValue(node, globalObject);
                    return jsGlobalObject.CallFunction<Action<Action, Action<string>>, JavaScriptValue>("Promise", promiseBody, true);
                },
                (node, value) =>
                {
                    //from a promise 
                    return Task.Factory.FromAsync(

                        (callback, state) =>
                        {
                            return BeginMethod(value, node, callback, state);
                        }
                        , EndMethod, null
                        );


                }, false
                );
        }

        public static void RegisterTask<TResult>(this IJSValueConverterService target)
        {
            if (target.CanConvert<Task<TResult>>())
            {
                return;
            }
            //register internal call back types
            target.RegisterFunctionConverter<TResult>();
            target.RegisterMethodConverter<TResult>();
            target.RegisterMethodConverter<string>();
            target.RegisterFunctionConverter<JSValue>();
            target.RegisterMethodConverter<Action<TResult>, Action<string>>();


            target.RegisterConverter<Task<TResult>>(
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
                    var jsValueService = node.GetService<IJSValueService>();
                    var globalObject = jsValueService.JSGlobalObject;
                    var jsGlobalObject = new JSValue(node, globalObject);
                    return jsGlobalObject.CallFunction<Action<Action<TResult>, Action<string>>, JavaScriptValue>("Promise", promiseBody, true);
                },
                (node, value) =>
                {
                    //from a promise 
                    return Task.Factory.FromAsync(

                        (callback, state) =>
                        {
                            return BeginMethod<TResult>(value, node, callback, state);
                        }
                        , EndMethod<TResult>, null
                        );


                }, false
                );
        }

        private static IAsyncResult BeginMethod(JavaScriptValue func, IServiceNode node, AsyncCallback callback, object state)
        {
            var conveter = node.GetService<IJSValueConverterService>();
            var jsValueService = node.GetService<IJSValueService>();
            AsyncResult result = new AsyncResult();
            Action fullfilledCallback = () =>
            {
                callback(result);
            };
            Action<string> rejectedCallback = (s) =>
            {
                throw new PromiseRejectedException(s);
            };

            Func<JSValue> promiseCall = conveter.FromJSValue<Func<JSValue>>(func);// the target function which returns a promise object
            JSValue promiseObject = promiseCall();//call the function,get the Promise object
            var service = promiseObject.ServiceNode.GetService<PromiseCallbackPairService>();//get the delegate interceptor
            service.Begin();//enable the interceptor
            promiseObject.CallMethod<Action, Action<String>>("then", fullfilledCallback, rejectedCallback);
            System.Diagnostics.Debug.WriteLine("[Then] called");
            service.End();//disable the interceptor
            return result;
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

        private static void EndMethod(IAsyncResult result)
        {
            //do nothing
        }
        
    }
}
