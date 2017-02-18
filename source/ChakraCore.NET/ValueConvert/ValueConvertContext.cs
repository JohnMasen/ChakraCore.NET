using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.GC;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class JSValueConvertContext : ContextObjectBase
    {
        public DelegateHandler Handler { get; private set; }
        public readonly JavaScriptValue JSClass;
        public JSValueConvertContext(ChakraContext context,DelegateHandler handler,JavaScriptValue caller) : base(context)
        {
            Handler = handler;
            JSClass = caller;
        }
        public JSValueConvertContext(ChakraContext context, JavaScriptValue caller) : base(context)
        {
            Handler = new DelegateHandler();
            JSClass = caller;
        }

    }
}
