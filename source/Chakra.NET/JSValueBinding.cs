using Chakra.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chakra.NET
{
    public class JSValueBinding :ContextObjectBase<JSValueBinding>
    {
        private readonly JavaScriptValue source;
        ValueConvertContext convertContext;
        public JSValueBinding(ChakraContext context, JavaScriptValue source, ValueConvertContext convertContext) :base(context)
        {
            this.source = source;
            this.convertContext = convertContext;
        }

        public void SetMethod<T>(string name,Action<T> a)
        {
            RuntimeContext.ValueConverter.RegisterMethodConverter<T>();
            WriteProperty<Action<T>>(name, a);
        }

        public void SetFunction<T,TResult>(string name,Func<T,TResult> callback, Func<T, TResult> constructCallback)
        {
            RuntimeContext.ValueConverter.RegisterFunctionConverter<T,TResult>();
            if (callback==null &&constructCallback==null)
            {
                throw new ArgumentException("callback and constructCallback cannot both be null");
            }
            Func<bool, T, TResult> tmp = (isConstruct, para1) =>
               {
                   TResult result;
                   if (isConstruct)
                   {
                       if (constructCallback==null)
                       {
                           throw new NotImplementedException(@"function {name} does not support construct call");
                       }
                       else
                       {
                           result = constructCallback(para1);
                       }
                   }
                   else
                   {
                       if (callback==null)
                       {
                           throw new NotImplementedException(@"function {name} does not support direct call");
                       }
                       else
                       {
                           result = callback(para1);
                       }
                   }
                   return result;
               };
            WriteProperty<Func<bool, T, TResult>>(name, tmp);
        }


        public T ReadProperty<T>(JavaScriptPropertyId id)
        {
            JavaScriptValue tmp = RuntimeContext.With<JavaScriptValue>(() => { return source.GetProperty(id); });

            return RuntimeContext.ValueConverter.FromJSValue<T>(convertContext, tmp);
        }

        public T ReadProperty<T>(string name)
        {
            JavaScriptPropertyId id = RuntimeContext.With<JavaScriptPropertyId>(() => { return JavaScriptPropertyId.FromString(name); });

            return ReadProperty<T>(id);
        }


        public void WriteProperty<T>(JavaScriptPropertyId id, T value)
        {
            var tmp = RuntimeContext.ValueConverter.ToJSValue<T>(convertContext, value);
            RuntimeContext.With(() => { source.SetProperty(id, tmp, true); });
        }

        public void WriteProperty<T>(string name, T value)
        {
            JavaScriptPropertyId id = RuntimeContext.With<JavaScriptPropertyId>(() => { return JavaScriptPropertyId.FromString(name); });
            WriteProperty<T>(id, value);
        }
    }
}
