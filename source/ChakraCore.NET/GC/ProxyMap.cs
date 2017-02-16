using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.GC
{
    public struct ProxyMap<T> where T:class
    {
        public Guid ItemID;
        public T source;
        public JavaScriptValue proxy;
        public DelegateHandler DelegateHandler;
        public ProxyMap(Guid id,T source,JavaScriptValue value,DelegateHandler handler)
        {
            ItemID = id;
            this.source = source;
            proxy = value;
            DelegateHandler = handler;
        }
    }
}
