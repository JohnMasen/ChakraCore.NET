using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;


namespace ChakraCore.NET
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
