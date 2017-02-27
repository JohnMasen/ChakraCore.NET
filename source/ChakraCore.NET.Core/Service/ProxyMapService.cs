using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Core.API;

namespace ChakraCore.NET.Core
{
    public class ProxyMapService : ServiceBase, IProxyMapService
    {
        SortedDictionary<Type, object> TypeMapLists = new SortedDictionary<Type, object>(TypeComparer.Instance);
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

        public JavaScriptValue Map<T>(T obj,Action<JSValueBinding> createBinding) where T:class
        {
            JavaScriptValue output;
            var list = getMapList<T>(true);
            if (list.objList.ContainsKey(obj))
            {
                return list.objList[obj];//object already mapped, return cached object
            }
            output = serviceNode.GetService<IContextSwitchService>().With<JavaScriptValue>(() =>
            {
                return JavaScriptValue.CreateExternalObject(IntPtr.Zero, null);
            });
            JSValueBinding binding = new JSValueBinding(serviceNode, output);
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
                list.jsList.Remove(jsvalue);
                list.objList.Remove(obj);
            }

        }


        public void ReleaseAll()
        {
            TypeMapLists.Clear();
        }

        private class TypeMapList<T> where T : class
        {
            public SortedDictionary<JavaScriptValue, Tuple<T,JSValueBinding>> jsList = new SortedDictionary<JavaScriptValue, Tuple<T, JSValueBinding>>();
            public IDictionary<T, JavaScriptValue> objList = new Dictionary<T, JavaScriptValue>(new ObjectReferenceEqualityComparer<T>());

            public void Add(T obj, JavaScriptValue value, JSValueBinding binding)
            {
                jsList.Add(value, new Tuple<T, JSValueBinding>(obj,binding));
                objList.Add(obj, value);
            }
        }
    }
}
