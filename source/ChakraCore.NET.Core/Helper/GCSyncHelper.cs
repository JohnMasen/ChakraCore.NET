using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Service
{
    public static class GCSyncHelper
    {
        public class ObjectWrapper<T>
        {
            private IDisposable sourceHolder;
            public ObjectWrapper(IDisposable disposable,T obj)
            {
                sourceHolder = disposable;
                Reference = obj;
            }
            public T Reference { get; private set; }
        }

        public static ObjectWrapper<T> CreateLinkedJsGCWrapper<T>(this IGCSyncService service, JavaScriptValue jsValue,T obj)
        {
            var holder = service.CreateJsGCWrapper(jsValue);
            return new ObjectWrapper<T>(holder, obj);
        }
    }
}
