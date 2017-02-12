using Chakra.NET.API;
using System;
namespace Chakra.NET
{
    public partial class JSValueWithContext
    {
        public readonly JavaScriptValue target;
        public readonly ChakraContext context;
        public JSValueWithContext(JavaScriptValue target, ChakraContext context)
        {
            this.target = target;
            this.context = context;
        }

        public JSValueWithContext SetMethod(string name,Action a)
        {
            context.WriteProperty<Action>(target, name, a);
            return this;
        }

        public JSValueWithContext SetProperty<T>(string name,Action<T> set, Func<T> get)
        {
            if (set!=null)
            {
                SetMethod<T>("set" + name, set);
            }
            if (get!=null)
            {
                SetFunction<T>("get" + name, get);
            }
            return this;
        }


        public JSValueWithContext SetField<T>(string name,T value)
        {
            context.WriteProperty<T>(target,name,value);
            return this;
        }

        public T GetField<T>(string name)
        {
            return context.ReadProperty<T>(target, name);
        }

        public T GetValue<T>()
        {
            return context.ReadValue<T>(target);
        }

        public void CallMethod(string name)
        {
            using (context.With())
            {
                GetField<Action>(name).Invoke();
            }
            
        }

        //public void CallMethod<T>(string name, T para1)
        //{
        //    object tmpHolder = new object();//temp object to hold potencial call back during this call
        //    using (var w = context.With(TimeSpan.MaxValue, tmpHolder))
        //    {
        //        context.ValueConverter.RegisterMethodConverter<T>();
        //        GetField<Action<T>>(name).Invoke(para1);
        //        context.GCStackTrace.Release();//because there will be no callback to release the node, release it manually
        //    }
        //}

        //public T CallFunction<T>(string name)
        //{
        //    object tmpHolder = new object();//temp object to hold potencial call back during this call
        //    using (var w = context.With(TimeSpan.MaxValue, tmpHolder))
        //    {
        //        context.ValueConverter.RegisterMethodConverter<T>();
        //        return GetField<Func<T>>(name).Invoke();
        //    }
        //}

        //public JSValueWithHost SetMethod<T>(string name, Action<T> a)
        //{
        //    host.ValueConverter.RegisterMethodConverter<T>();
        //    host.WriteProperty<Action<T>>(target, name, a);
        //    return this;
        //}

        //public JSValueWithHost SetFunction<T>(string name, Func<T> a)
        //{
        //    host.ValueConverter.RegisterFunctionConverter<T>();
        //    host.WriteProperty<Func<T>>(target, name, a);
        //    return this;
        //}

    }
}
