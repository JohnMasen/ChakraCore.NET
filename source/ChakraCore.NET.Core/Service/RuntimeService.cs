using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public class RuntimeService : ServiceBase, IRuntimeService,IDisposable
    {
        JavaScriptRuntime runtime;
        JavaScriptContext context;
        IContextSwitchService switchService;
        public RuntimeService(JavaScriptRuntime runtime)
        {
            this.runtime = runtime;
            context = runtime.CreateContext();
            switchService = new ContextSwitchService(context);
        }
        public IContextSwitchService InternalContextSwitchService => switchService;

        public void CollectGarbage()
        {
            runtime.CollectGarbage();
        }

        public void Dispose()
        {
            switchService.Dispose();
        }

        public void TerminateRuningScript()
        {
            runtime.Disabled = true;
            runtime.Disabled = false;
        }
    }
}
