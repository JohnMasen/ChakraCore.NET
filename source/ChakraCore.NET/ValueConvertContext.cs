using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.GC;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class JSValueConvertContext : ContextObjectBase<JSValueConvertContext>
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

        public JavaScriptNativeFunction WrapFunctionCall(JavaScriptNativeFunction callback)
        {
            JavaScriptNativeFunction result =
                (callee, isConstructCall, arguments, argumentCount, callbackData) =>
                {
                    RuntimeContext.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.
                    var r= callback(callee, isConstructCall, arguments, argumentCount, callbackData);
                    RuntimeContext.Enter();//restore context
                    return r;
                };
            Handler.Hold(result);
            return result;
        }
    }
}
