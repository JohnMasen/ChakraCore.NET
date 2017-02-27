using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Core.API;

namespace ChakraCore.NET.Core
{
    public class CallContextService : ServiceBase, ICallContextService
    {
        public JavaScriptValue Caller { get; private set; }
        public CallContextService(JavaScriptValue caller)
        {
            Caller = caller;
        }
        
    }
}
