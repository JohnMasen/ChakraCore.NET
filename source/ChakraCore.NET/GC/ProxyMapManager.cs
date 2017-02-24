using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ChakraCore.NET.Helper;

namespace ChakraCore.NET.GC
{
    public class ProxyMapManager
    {
        private SortedDictionary<Type, object> mapList = new SortedDictionary<Type, object>(new TypeComparer());
        

        

        public JavaScriptValue ReigsterMap<T>(T obj, Func<IntPtr, JavaScriptObjectFinalizeCallback,JavaScriptValue>callback, out DelegateHandler delegateHandler) where T : class
        {
            MapItemList<T> currentTypeList;
            if (mapList.ContainsKey(typeof(T)))
            {
                currentTypeList = mapList[typeof(T)] as MapItemList<T>;
                if (currentTypeList==null)
                {
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
                System.Diagnostics.Debug.WriteLine("proxy object collected");
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

        private class MapItemList<T>  where T : class
        {
            private IDictionary<Guid, ProxyMap<T>> internalMap;
            private IDictionary<T, ProxyMap<T>> externalMap; 

            
            public void Release(Guid value)
            {
                var item = internalMap[value];
                internalMap.Remove(value);
                externalMap.Remove(item.source);
            }
            private void initCollection(ProxyMap<T> item)
            {
                if ((item.source as IComparer<T>)!=null)
                {
                    //item implemented IComparer interface
                    internalMap = new SortedDictionary<Guid, ProxyMap<T>>();
                    externalMap = new SortedDictionary<T, ProxyMap<T>>();
                }
                else
                {
                    internalMap=new Dictionary<Guid, ProxyMap<T>>();
                    externalMap = new Dictionary<T, ProxyMap<T>>();
                }
            }

            public void Add(ProxyMap<T> item)
            {
                if (internalMap==null)
                {
                    initCollection(item);
                }
                internalMap.Add(item.ItemID, item);
                externalMap.Add(item.source, item);
            }

            public JavaScriptValue? Get(T obj)
            {
                if (externalMap?.ContainsKey(obj)==true)
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
                    throw new InvalidOperationException("Internal proxy map list corrupted");
                }
            }
        }

    }
}
