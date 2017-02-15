using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public partial class JSValue : ContextObjectBase<JSValue>
    {
        public JSValue Parent { get; private set; }
        public JavaScriptValue Reference { get; private set; }
        private JSValueBinding binding;

        private ValueConvertContext convertContext;

        public readonly bool IsProxy;
        public readonly bool IsRoot;
        internal JSValue(ChakraContext context, JSValue parent, JavaScriptValue reference, bool isRoot) : base(context)
        {
            Parent = parent;
            Reference = reference;
            convertContext = new ValueConvertContext(context,reference);
            binding = new JSValueBinding(context,reference, convertContext);
            IsProxy = reference.HasExternalData;
            IsRoot = isRoot;
        }

        public static JSValue CreateRoot(ChakraContext context)
        {
            return new JSValue(context, null, context.GlobalObject, true);
        }

        public T ReadProperty<T>(JavaScriptPropertyId id)
        {
            JavaScriptValue tmp = RuntimeContext.With<JavaScriptValue>(() => { return Reference.GetProperty(id); });

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
            RuntimeContext.With(() => { Reference.SetProperty(id, tmp, true); });
        }

        public void WriteProperty<T>(string name, T value)
        {
            JavaScriptPropertyId id = RuntimeContext.With<JavaScriptPropertyId>(() => { return JavaScriptPropertyId.FromString(name); });
            WriteProperty<T>(id, value);
        }



        //public void CallMethod<T>(string name, T para1)
        //{
        //    RuntimeContext.ValueConverter.RegisterMethodConverter<T>();
        //    var a = ReadProperty<Action<T>>(name);
        //    a(para1);
        //}

        //public TResult CallFunction<T1,TResult>(string name, T1 para1, bool isConstructCall=false)
        //{
        //    RuntimeContext.ValueConverter.RegisterFunctionConverter<T1,TResult>();
        //    var a = ReadProperty<Func<bool,T1,TResult>>(name);
        //    return a(isConstructCall,para1);
        //}

    }
}
