using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Debug;
namespace ChakraCore.NET.ServiceInterface
{
    public interface IRuntimeDebuggingService
    {
        void AttachAdapter(IDebugAdapter adapter);
        void DetachAdapter();
        void AddScriptSource(string name, string content);
        void ScriptReady();
    }
}
