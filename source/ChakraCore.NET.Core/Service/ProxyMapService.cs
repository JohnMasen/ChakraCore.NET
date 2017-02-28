using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using System.Linq;

namespace ChakraCore.NET
{
    public class ProxyMapService : ServiceBase, IProxyMapService
    {
        private IRuntimeService runtimeService => serviceNode.GetService<IRuntimeService>();
        SortedDictionary<Type, IEnumerable<JavaScriptValue>> TypeMapLists = new SortedDictionary<Type, IEnumerable<JavaScriptValue>>(TypeComparer.Instance);
        public T Get<T>(JavaScriptValue value) where T : class
        {
            var list = getMapList<T>(false);
            if (list==null)
            {
                throw new ArgumentException("proxy object type not registered");
            }
            if (list.jsList.ContainsKey(value))
            {
                return list.jsList[value].Item1;
            }
            throw new ArgumentException("proxy object not mapped");
        }

        private TypeMapList<T> getMapList<T>(bool createIfNotExists=true) where T : class
        {
            Type tt = typeof(T);
            if (TypeMapLists.ContainsKey(tt))
            {
                return TypeMapLists[tt] as TypeMapList<T>;
            }
            else if(createIfNotExists)
            {
                TypeMapList<T> item = new TypeMapList<T>();
                TypeMapLists.Add(tt, item);
                return item;
            }
            return null;
        }

        public JavaScriptValue Map<T>(T obj, Action<JSValueBinding, T, IServiceNode> createBinding) where T:class
        {
            JavaScriptValue output;
            var list = getMapList<T>(true);
            if (list.objList.ContainsKey(obj))
            {
                return list.objList[obj];//object already mapped, return cached object
            }
            output = runtimeService.InternalContextSwitchService.With<JavaScriptValue>(() =>
            {
                var result= JavaScriptValue.CreateExternalObject(IntPtr.Zero, null);//do not handle the external GC event as the value will be hold until user dispose current service
                result.AddRef();
                return result;
            });
            JSValueBinding binding = new JSValueBinding(serviceNode, output);
            createBinding?.Invoke(binding,obj,serviceNode);
            list.Add(obj, output,binding);
            return output;
        }

        public void Release<T>(T obj) where T:class
        {
            var list = getMapList<T>(false);
            if (list==null)
            {
                return;
            }
            if (list.objList.ContainsKey(obj))
            {
                var jsvalue = list.objList[obj];
                runtimeService.InternalContextSwitchService.With(()=> { jsvalue.Release(); });//use runtime internal context to ensure value can be safely released
                list.Remove(obj);
            }

        }

        public void Release<T>(JavaScriptValue value) where T : class
        {
            var list = getMapList<T>(false);
            if (list!=null)
            {
                if(list.jsList.TryGetValue(value,out Tuple<T, JSValueBinding> output))
                {
                    runtimeService.InternalContextSwitchService.With(() => { value.Release(); });
                    list.Remove(value);
                }
            }
        }
        public void ReleaseAll()
        {
            runtimeService.InternalContextSwitchService.With(() =>
            {
                foreach (var list in TypeMapLists)
                {
                    foreach (var item in list.Value)
                    {
                        item.Release();
                    }
                }
            });
            
            TypeMapLists.Clear();
        }

        public void Dispose()
        {
            
        }

        private class TypeMapList<T> :IEnumerable<JavaScriptValue> where T : class
        {
            public SortedDictionary<JavaScriptValue, Tuple<T,JSValueBinding>> jsList = new SortedDictionary<JavaScriptValue, Tuple<T, JSValueBinding>>(JavaScriptValueComparer.Instance);
            public IDictionary<T, JavaScriptValue> objList = new Dictionary<T, JavaScriptValue>(new ObjectReferenceEqualityComparer<T>());

            public void Add(T obj, JavaScriptValue value, JSValueBinding binding)
            {
                jsList.Add(value, new Tuple<T, JSValueBinding>(obj,binding));
                objList.Add(obj, value);
            }

            public void Remove(T obj)
            {
                if (objList.ContainsKey(obj))
                {
                    JavaScriptValue v = objList[obj];
                    objList.Remove(obj);
                    jsList.Remove(v);
                }
            }

            public void Remove(JavaScriptValue value)
            {
                if (jsList.ContainsKey(value))
                {
                    T obj = jsList[value].Item1;
                    objList.Remove(obj);
                    jsList.Remove(value);
                }
            }

            public IEnumerator<JavaScriptValue> GetEnumerator()
            {
                return jsList.Keys.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return jsList.Keys.GetEnumerator();
            }
        }
    }
}
