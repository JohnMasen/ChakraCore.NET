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
        /// <summary>
        /// returns current caller, JSValueConvertContext and JSValueConverter both have their current caller(JSClass)
        /// JSValueConverter has higher priority
        /// </summary>
        public JavaScriptValue JSClass
        {
            get
            {
                JavaScriptValue result = RuntimeContext.ValueConverter.JSCaller;
                if (result.IsValid)
                {
                    return result;
                }
                else
                {
                    return contextJSCalss;
                }
            }

        }
        public readonly JavaScriptValue contextJSCalss;
        public JSValueConvertContext(ChakraContext context, DelegateHandler handler, JavaScriptValue caller) : base(context)
        {
            Handler = handler;
            contextJSCalss = caller;
        }
        public JSValueConvertContext(ChakraContext context, JavaScriptValue caller) : base(context)
        {
            Handler = new DelegateHandler();
            contextJSCalss = caller;
        }

    }
}
