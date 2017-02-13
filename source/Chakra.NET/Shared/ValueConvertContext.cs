using System;
using System.Collections.Generic;
using System.Text;
using Chakra.NET.GC;
using Chakra.NET.API;

namespace Chakra.NET
{
    public class ValueConvertContext : ContextObjectBase
    {
        public DelegateHandler Handler { get; private set; }
        public JavaScriptValue JSClass;
        public ValueConvertContext(ChakraContext context,DelegateHandler handler) : base(context)
        {
            Handler = handler;
        }

        public JavaScriptNativeFunction WrapFunctionCall(JavaScriptNativeFunction callback)
        {
            JavaScriptNativeFunction result =
                (callee, isConstructCall, arguments, argumentCount, callbackData) =>
                {
                    Context.Leave();//leave the context. [1]user method does not require javascript context  [2]user may switch thread in the code.
                    var r= callback(callee, isConstructCall, arguments, argumentCount, callbackData);
                    Context.Enter();//restore context
                    return r;
                };
            Handler.Hold(result);
            return result;
        }
    }
}
