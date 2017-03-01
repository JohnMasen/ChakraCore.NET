using System;
using ChakraCore.NET.API;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChakraCore.NET.Timer
{
    public class JSTimer :ServiceConsumerBase
    {
        SortedDictionary<Guid, Tuple<JavaScriptValue, CancellationTokenSource>> list = new SortedDictionary<Guid, Tuple<JavaScriptValue, CancellationTokenSource>>();
        public JSTimer(IServiceNode parentNode) : base(parentNode, "JSTimer")
        {
        }

        public Guid SetInterval(JavaScriptValue callback,int delay)
        {
            if (callback.ValueType!=JavaScriptValueType.Function)
            {
                throw new ArgumentException("SetInterval only accepts javascript function");
            }
            CancellationTokenSource source = new CancellationTokenSource();
            Guid id = addCallback(callback, source);
            Action a = ServiceNode.GetService<IJSValueConverterService>().FromJSValue<Action>(callback);
            Task.Factory.StartNew(async() =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(delay, source.Token);
                        a();
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                
            },source.Token);
            return id;
        }

        public void ClearInterval(Guid id)
        {
            releaseCallback(id);
        }

        public void SetTimeout(Action callback,int delay)
        {
            Task.Factory.StartNew(async() =>
            {
                await Task.Delay(delay);
                callback();
            });
        }

        private void releaseCallback(Guid id)
        {
            if (!list.ContainsKey(id))
            {
                return;//ignore the request if id does not exists
            }
            ServiceNode.WithContext(() =>
            {
                list[id].Item1.Release();//release the callback function reference
            });
            list[id].Item2.Cancel();//cancel the interval loop
            list.Remove(id);
        }

        private Guid addCallback(JavaScriptValue value, CancellationTokenSource source)
        {
            Guid result = Guid.NewGuid();
            ServiceNode.WithContext(() =>
            {
                value.AddRef();//hold the callback function
            });
            list.Add(result, new Tuple<JavaScriptValue, CancellationTokenSource>(value, source));
            return result;
        }


    }
}
