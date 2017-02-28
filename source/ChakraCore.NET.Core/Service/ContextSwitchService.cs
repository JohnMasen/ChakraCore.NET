
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ChakraCore.NET
{
    public class ContextSwitchService :ServiceBase, IContextSwitchService,IDisposable
    {
        JavaScriptContext context;
        public EventWaitHandle Handle { get; private set; }
        public ContextSwitchService(JavaScriptContext context,EventWaitHandle handle)
        {
            Handle = handle;
            this.context = context;
            context.AddRef();
        }


        public bool IsCurrentContext => JavaScriptContext.Current==context;

        public bool Enter()
        {
            if (IsCurrentContext)
            {
                return false;
            }
            else
            {
                Handle.WaitOne();
                JavaScriptContext.Current = context;
                return true;
            }
        }

        public void Leave()
        {
            JavaScriptContext.Current = JavaScriptContext.Invalid;
            Handle.Set();
        }

        public void With(Action a)
        {
            if(Enter())
            {
                a();
                Leave();
            }
            else
            {
                a();
            }
        }

        public T With<T>(Func<T> f)
        {
            T result;
            if (Enter())
            {
                result=f();
                Leave();
            }
            else
            {
                result = f();
            }
            return result;
        }

        public void Dispose()
        {
            context.Release();
        }
    }
}
