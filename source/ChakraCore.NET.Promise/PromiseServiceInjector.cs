using ChakraCore.NET.API;
using ChakraCore.NET.Promise;
using System;
using System.Threading.Tasks;

namespace ChakraCore.NET
{
    public static class PromiseServiceInjector
    {

        
        public static void RegisterTask(this IJSValueConverterService target)
        {
            if (target.CanConvert<Task>())
            {
                return;
            }
            //register internal call back types
            target.RegisterMethodConverter<string>();
            target.RegisterFunctionConverter<JSValue>();
            target.RegisterMethodConverter<JavaScriptValue>();


            target.RegisterConverter<Task>(
                (node, value) =>
                {
                    var jsValueService = node.GetService<IJSValueService>();
                    var globalObject = jsValueService.JSGlobalObject;
                    var converter = node.GetService<IJSValueConverterService>();
                    var tmp = node.WithContext(() =>
                      {
                          JavaScriptValue.CreatePromise(out var result, out var resolve, out var reject);
                          return Tuple.Create(result, resolve, reject);
                      });
                    //start the task on new thread
                    Task.Factory.StartNew(async() =>
                    {
                        try
                        {
                            await value;
                            jsValueService.CallFunction(tmp.Item2, globalObject);
                        }
                        catch (PromiseRejectedException ex)
                        {
                            var message=converter.ToJSValue(ex.ToString());
                            jsValueService.CallFunction(tmp.Item3, globalObject, message);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    });
                    //return the promise without wait task complete
                    return tmp.Item1;
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
            target.RegisterMethodConverter<JavaScriptValue>();


            target.RegisterConverter<Task<TResult>>(
                (node, value) =>
                {
                    var jsValueService = node.GetService<IJSValueService>();
                    var globalObject = jsValueService.JSGlobalObject;
                    var converter = node.GetService<IJSValueConverterService>();
                    var tmp = node.WithContext(() =>
                    {
                        JavaScriptValue.CreatePromise(out var result, out var resolve, out var reject);
                        return Tuple.Create(result, resolve, reject);
                    });
                    //start the task on new thread
                    Task.Factory.StartNew(async () =>
                    {
                        try
                        {
                            var r=await value;
                            jsValueService.CallFunction(tmp.Item2, globalObject, converter.ToJSValue(r));
                        }
                        catch (PromiseRejectedException ex)
                        {
                            var message = converter.ToJSValue(ex.ToString());
                            jsValueService.CallFunction(tmp.Item3, globalObject, message);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    });

                    //return the promise without wait task complete
                    return tmp.Item1;
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
                result.SetError(s.ToString());
                callback(result);
            };

            Func<JSValue> promiseCall = conveter.FromJSValue<Func<JSValue>>(func);// the target function which returns a promise object
            JSValue promiseObject = promiseCall();
            //call the function,get the Promise object
            promiseObject.CallMethod("then", fullfilledCallback, rejectedCallback);
            System.Diagnostics.Debug.WriteLine("[Then] called");
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
                result.SetError(s.ToString());
                callback(result);
            };

            Func<JSValue> promiseCall = conveter.FromJSValue<Func<JSValue>>(func);// the target function which returns a promise object
            JSValue promiseObject = promiseCall();//call the function,get the Promise object
            promiseObject.CallMethod("then", fullfilledCallback, rejectedCallback);
            System.Diagnostics.Debug.WriteLine("[Then] called");
            return result;
        }

        private static T EndMethod<T>(IAsyncResult result)
        {
            var asyncResult = result as AsyncResult<T> ?? throw new ArgumentException("Result is of wrong type.");
            if (asyncResult.HasError)
            {
                throw new PromiseRejectedException(asyncResult.Error.ToString());
            }
            return asyncResult.Result;
        }

        private static void EndMethod(IAsyncResult result)
        {
            var asyncResult = result as AsyncResult ?? throw new ArgumentException("Result is of wrong type.");
            if (asyncResult.HasError)
            {
                throw new PromiseRejectedException(asyncResult.Error.ToString());
            }
        }

    }
}
