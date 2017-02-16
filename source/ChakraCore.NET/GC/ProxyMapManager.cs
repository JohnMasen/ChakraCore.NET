using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace ChakraCore.NET.GC
{
    public class ProxyMapManager:LoggableObjectBase<ProxyMapManager>
    {
        private Dictionary<Type, object> mapList = new Dictionary<Type, object>();
        public JavaScriptValue ReigsterMap<T>(T obj, Func<IntPtr, JavaScriptObjectFinalizeCallback,JavaScriptValue>callback, out DelegateHandler delegateHandler) where T : class
        {
            MapItemList<T> currentTypeList;
            if (mapList.ContainsKey(typeof(T)))
            {
                currentTypeList = mapList[typeof(T)] as MapItemList<T>;
                if (currentTypeList==null)
                {
                    log.LogCritical("Internal proxy map list corrupted");
                    throw new InvalidOperationException("Internal proxy map list corrupted");
                }
            }
            else
            {
                currentTypeList = new MapItemList<T>();
                mapList.Add(typeof(T), currentTypeList);
            }

            JavaScriptObjectFinalizeCallback cb = (p) =>
            {
                GCHandle h = GCHandle.FromIntPtr(p);
                Guid id = (Guid)h.Target;
                currentTypeList.Release(id);
                h.Free();
            };

            delegateHandler = new DelegateHandler();
            Guid g = Guid.NewGuid();
            var handle = GCHandle.Alloc(g, GCHandleType.Pinned);
            JavaScriptValue proxy = callback((IntPtr)handle, cb);
            ProxyMap<T> map = new ProxyMap<T>(g, obj, proxy, delegateHandler);
            currentTypeList.Add(map);
            return proxy;
        }

        public T GetSource<T>(IntPtr p) where T:class
        {
            Guid id = (Guid)GCHandle.FromIntPtr(p).Target;
            return (mapList[typeof(T)] as MapItemList<T>).Get(id);
        }

        public JavaScriptValue? GetProxy<T>(T source) where T:class
        {
            if (!mapList.ContainsKey(typeof(T)))
            {
                return null;
            }
            var item = mapList[typeof(T)] as MapItemList<T>;
            return item.Get(source);
        }

        private class MapItemList<T> :LoggableObjectBase<MapItemList<T>> where T : class
        {
            private SortedDictionary<Guid, ProxyMap<T>> internalMap = new SortedDictionary<Guid, ProxyMap<T>>();
            private SortedDictionary<T, ProxyMap<T>> externalMap =new SortedDictionary<T, ProxyMap<T>>();

            public void Release(Guid value)
            {
                var item = internalMap[value];
                internalMap.Remove(value);
                externalMap.Remove(item.source);
            }

            public void Add(ProxyMap<T> item)
            {
                internalMap.Add(item.ItemID, item);
                externalMap.Add(item.source, item);
            }

            public JavaScriptValue? Get(T obj)
            {
                if (externalMap.ContainsKey(obj))
                {
                    return externalMap[obj].proxy;
                }
                else
                {
                    return null;
                }
            }

            public T Get(Guid id)
            {
                if (internalMap.ContainsKey(id))
                {
                    return internalMap[id].source;
                }
                else
                {
                    log.LogCritical("Internal proxy map list corrupted");
                    throw new InvalidOperationException("Internal proxy map list corrupted");
                }
            }
        }

    }
}
