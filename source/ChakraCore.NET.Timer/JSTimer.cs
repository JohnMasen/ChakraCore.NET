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
        SortedDictionary<Guid, Tuple<Action, CancellationTokenSource>> list = new SortedDictionary<Guid, Tuple<Action, CancellationTokenSource>>();
        public JSTimer(IServiceNode parentNode) : base(parentNode, "JSTimer")
        {
        }

        public Guid SetInterval(Action callback,int delay)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            Guid id = addCallback(callback, source);
            Task.Factory.StartNew(async() =>
            {
                while (true)
                {
                    try
                    {
                        await Task.Delay(delay, source.Token);
                        callback();
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
            list[id].Item2.Cancel();//cancel the interval loop
            list.Remove(id);
        }

        private Guid addCallback(Action value, CancellationTokenSource source)
        {
            Guid result = Guid.NewGuid();
            list.Add(result, new Tuple<Action, CancellationTokenSource>(value, source));
            return result;
        }
        public void ReleaseAll()
        {
            foreach (var item in list)
            {
                item.Value.Item2.Cancel();
            }
        }

    }
}
