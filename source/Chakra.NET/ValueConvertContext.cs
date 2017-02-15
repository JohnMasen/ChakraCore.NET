using System;
using System.Collections.Generic;
using System.Text;
using Chakra.NET.GC;
using Chakra.NET.API;

namespace Chakra.NET
{
    public class ValueConvertContext : ContextObjectBase<ValueConvertContext>
    {
        public DelegateHandler Handler { get; private set; }
        public readonly JavaScriptValue JSClass;
        public ValueConvertContext(ChakraContext context,DelegateHandler handler,JavaScriptValue caller) : base(context)
        {
            Handler = handler;
            JSClass = caller;
        }
        public ValueConvertContext(ChakraContext context, JavaScriptValue caller) : base(context)
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
