using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public class RuntimeInternalContextSwitcher : ServiceBase, IContextSwitchService
    {
        private JavaScriptContext context;
        private JavaScriptContext previousContext;
        public bool IsCurrentContext => JavaScriptContext.Current == context;
        public RuntimeInternalContextSwitcher(JavaScriptContext internalContext)
        {
            context = internalContext;
            context.AddRef();
        }

        public void Dispose()
        {
            context.Release();
        }

        public bool Enter()
        {
            previousContext = JavaScriptContext.Current;//force context switch to internal context
            JavaScriptContext.Current = context;
            return true;
        }

        public void Leave()
        {
            JavaScriptContext.Current = previousContext;
        }

        public void With(Action a)
        {
            Enter();
            a();
            Leave();
        }

        public T With<T>(Func<T> f)
        {
            Enter();
            T result = f();
            Leave();
            return result;
        }
    }
}
