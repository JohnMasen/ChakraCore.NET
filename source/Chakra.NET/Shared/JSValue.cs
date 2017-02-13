using Chakra.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chakra.NET
{
    public class JSValue : ContextObjectBase
    {
        public JSValue Parent { get; private set; }
        public JavaScriptValue Reference { get; private set; }
        private JSValueBinding binding;

        private ValueConvertContext convertContext;
        internal JSValue(ChakraContext context, JSValue parent,JavaScriptValue reference) : base(context)
        {
            Parent = parent;
            Reference = reference;
            convertContext = new ValueConvertContext(context);
            binding = new JSValueBinding(context, reference,convertContext);
        }
        
        public static JSValue CreateNew(ChakraContext context)
        {
            return new JSValue(context, null, context.GlobalObject);
        }

        public T ReadProperty<T>(JavaScriptPropertyId id)
        {
            using (RuntimeContext.With())
            {
                return RuntimeContext.ValueConverter.FromJSValue<T>(convertContext, Reference.GetProperty(id));
            }
            
        }

    }
}
