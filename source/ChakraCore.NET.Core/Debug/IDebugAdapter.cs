using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET.Debug
{
    public interface IDebugAdapter
    {
        Task OnDebugEvent(JavaScriptDiagDebugEvent eventType,string data,DebugEngine engine);
        Task ScriptReady(DebugEngine engine);
        void AddScript(SourceCode sourceCode);
        Task OnBreakPoint(BreakPoint breakPoint, DebugEngine engine);
    }
}
