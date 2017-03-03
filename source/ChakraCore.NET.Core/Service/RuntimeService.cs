
using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using System.Threading;

namespace ChakraCore.NET
{
    public class RuntimeService : ServiceBase, IRuntimeService,IDisposable
    {
        JavaScriptRuntime runtime;
        JavaScriptContext context;
        public IContextSwitchService InternalContextSwitchService { get; private set; }
        public RuntimeService(JavaScriptRuntime runtime)
        {
            this.runtime = runtime;
            context = runtime.CreateContext();
            InternalContextSwitchService = new RuntimeInternalContextSwitcher(context);
        }

        public void CollectGarbage()
        {
            runtime.CollectGarbage();
        }

        public void Dispose()
        {
            InternalContextSwitchService.Dispose();
        }

        public void TerminateRuningScript()
        {
            runtime.Disabled = true;
            runtime.Disabled = false;
        }
    }
}
