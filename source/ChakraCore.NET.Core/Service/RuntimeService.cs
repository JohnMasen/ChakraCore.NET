
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
        IContextSwitchService switchService;
        public RuntimeService(JavaScriptRuntime runtime,EventWaitHandle handle)
        {
            this.runtime = runtime;
            context = runtime.CreateContext();
            switchService = new ContextSwitchService(context, handle);
        }
        public IContextSwitchService InternalContextSwitchService => serviceNode.GetService<IContextSwitchService>(serviceNode);//force the service search path from current node

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
