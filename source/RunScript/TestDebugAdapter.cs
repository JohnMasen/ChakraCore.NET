using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChakraCore.NET;
using ChakraCore.NET.API;
using ChakraCore.NET.Debug;

namespace RunScript
{
    class TestDebugAdapter : IDebugAdapter
    {
        

        public void AddScript(SourceCode sourceCode)
        {
            Console.WriteLine($"AddScript,{sourceCode}");
        }

        public void AddScriptSource(string name, string content)
        {
            throw new NotImplementedException();
        }

        public void Init(IRuntimeDebuggingService debuggingService)
        {
            throw new NotImplementedException();
        }

        public Task OnAsyncBreak(BreakPoint breakPoint, DebugEngine engine)
        {
            throw new NotImplementedException();
        }

        public async Task OnBreakPoint(BreakPoint breakPoint, DebugEngine engine)
        {
            Console.WriteLine($"BreakPoint Hit:{breakPoint}");
            Console.WriteLine($"Stack trace:{await engine.GetStackTraceAsync()}");
            Console.WriteLine($"Stack properties:{await engine.GetStackPropertiesAsync(0)}");
            Console.WriteLine($"Object 7:{await engine.GetObjectFromHandleAsync(7)}");
            Console.WriteLine($"Object Properties of 7:{await engine.GetObjectPropertiesAsync(7)}");
            string eval = "o.d";
            Console.WriteLine($"eval '{eval}':{await engine.EvaluateAsync(eval, 0, false)}");
        }

        public async Task OnDebugEvent(JavaScriptDiagDebugEvent eventType,string data, DebugEngine engine)
        {
            Console.WriteLine($"[{eventType}],{data}");
            switch (eventType)
            {
                case JavaScriptDiagDebugEvent.JsDiagDebugEventSourceCompile:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventCompileError:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventStepComplete:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventDebuggerStatement:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventAsyncBreak:
                    //Console.WriteLine($"Breakpoint set: {await engine.SetBreakpointAsync(3, 8, 0)}");
                    foreach (var item in await engine.GetBreakPointsAsync())
                    {
                        Console.WriteLine($"Breakpoints={item}");
                    }
                    
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventRuntimeException:
                    break;
                default:
                    break;
            }
        }

        public Task OnException(RuntimeException exception, DebugEngine engine)
        {
            throw new NotImplementedException();
        }

        public Task OnStep(BreakPoint breakPoint, DebugEngine engine)
        {
            throw new NotImplementedException();
        }

        public async Task ScriptReady(DebugEngine engine)
        {
            //engine.RequestAsyncBreak();
            Console.WriteLine($"GetScripts {await engine.GetScriptsAsync()}");
            Console.WriteLine($"Breakpoint set: {await engine.SetBreakpointAsync(3, 8, 0)}");
        }
    }
}
