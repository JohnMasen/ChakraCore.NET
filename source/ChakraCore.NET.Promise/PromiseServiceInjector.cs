using ChakraCore.NET.API;
using ChakraCore.NET.Promise;
using System;
using System.Threading.Tasks;

namespace ChakraCore.NET
{
    public static class PromiseServiceInjector
    {

        public static void InjectTaskService(this IServiceNode target)
        {
            if (target.CanGetService<PromiseCallbackPairService>())
            {
                return;
            }
            PromiseCallbackPairService service = new PromiseCallbackPairService();
            target.PushService(service);
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
            target.RegisterMethodConverter<Action, Action<JavaScriptValue>>();


            target.RegisterConverter<Task>(
                (node, value) =>
                {
                    //convert resolve, reject
                    Action<Action, Action<JavaScriptValue>> promiseBody = async (resolve, reject) =>
                    {
                        try
                        {
                            await value;
                            resolve();
                        }
                        catch (PromiseRejectedException ex)
                        {
                            reject(ex.Error);
                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    };
                    var jsValueService = node.GetService<IJSValueService>();
                    var globalObject = jsValueService.JSGlobalObject;
                    var jsGlobalObject = new JSValue(node, globalObject);
                    return jsGlobalObject.CallFunction<Action<Action, Action<JavaScriptValue>>, JavaScriptValue>("Promise", promiseBody, true);
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
            target.RegisterMethodConverter<Action<TResult>, Action<JavaScriptValue>>();


            target.RegisterConverter<Task<TResult>>(
                (node, value) =>
                {
                    //convert resolve, reject
                    Action<Action<TResult>, Action<JavaScriptValue>> promiseBody = async (resolve, reject) =>
                       {
                           try
                           {
                               var result = await value;
                               resolve(result);
                           }
                           catch (PromiseRejectedException ex)
                           {
                               reject(ex.Error);
                           }
                           catch (Exception)
                           {
                               throw;
                           }
                           
                       };
                    var jsValueService = node.GetService<IJSValueService>();
                    var globalObject = jsValueService.JSGlobalObject;
                    var jsGlobalObject = new JSValue(node, globalObject);
                    return jsGlobalObject.CallFunction<Action<Action<TResult>, Action<JavaScriptValue>>, JavaScriptValue>("Promise", promiseBody, true);
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
            var result = new AsyncResult();
            Action fullfilledCallback = () =>
            {
                callback(result);
            };
            Action<JavaScriptValue> rejectedCallback = (s) =>
            {
                result.SetError(s);
                callback(result);
            };

            Func<JSValue> promiseCall = conveter.FromJSValue<Func<JSValue>>(func);// the target function which returns a promise object
            JSValue promiseObject = promiseCall();//call the function,get the Promise object
            var service = promiseObject.ServiceNode.GetService<PromiseCallbackPairService>();//get the delegate interceptor
            service.Begin();//enable the interceptor
            promiseObject.CallMethod("then", fullfilledCallback, rejectedCallback);
            System.Diagnostics.Debug.WriteLine("[Then] called");
            service.End();//disable the interceptor
            return result;
        }

        private static IAsyncResult BeginMethod<T>(JavaScriptValue func, IServiceNode node, AsyncCallback callback, object state)
        {
            var conveter = node.GetService<IJSValueConverterService>();
            var result = new AsyncResult<T>();
            Action<T> fullfilledCallback = (x) =>
            {
                result.SetResult(x);
                callback(result);
            };
            Action<JavaScriptValue> rejectedCallback = (s) =>
            {
                result.SetError(s);
                callback(result);
            };

            Func<JSValue> promiseCall = conveter.FromJSValue<Func<JSValue>>(func);// the target function which returns a promise object
            JSValue promiseObject = promiseCall();//call the function,get the Promise object
            var service=promiseObject.ServiceNode.GetService<PromiseCallbackPairService>();//get the delegate interceptor
            service.Begin();//enable the interceptor
            promiseObject.CallMethod("then", fullfilledCallback, rejectedCallback);
            System.Diagnostics.Debug.WriteLine("[Then] called");
            service.End();//disable the interceptor
            return result;
        }

        private static T EndMethod<T>(IAsyncResult result)
        {
            var asyncResult = result as AsyncResult<T> ?? throw new ArgumentException("Result is of wrong type.");
            if (asyncResult.Error.IsValid) throw new PromiseRejectedException(asyncResult.Error);
            return asyncResult.Result;
        }

        private static void EndMethod(IAsyncResult result)
        {
            var asyncResult = result as AsyncResult ?? throw new ArgumentException("Result is of wrong type.");
            if (asyncResult.Error.ValueType == JavaScriptValueType.Error) throw new PromiseRejectedException(asyncResult.Error);
        }
        
    }
}
