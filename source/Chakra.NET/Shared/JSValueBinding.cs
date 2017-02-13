using Chakra.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chakra.NET
{
    public class JSValueBinding : ContextObjectBase
    {
        public readonly JavaScriptValue Value;
        ValueConvertContext convertContext;
        public JSValueBinding(ChakraContext context,JavaScriptValue reference,ValueConvertContext convertContext) : base(context)
        {
            this.Value = reference;
            this.convertContext = convertContext;
        }
    }
}
