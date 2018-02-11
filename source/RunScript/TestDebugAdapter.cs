using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using ChakraCore.NET.Debug;

namespace RunScript
{
    class TestDebugAdapter : ChakraCore.NET.Debug.IDebugAdapter
    {
        public void AddScript(string name, string script)
        {
            var lines = script.Split(Environment.NewLine);
            Console.WriteLine($"script {name} loaded,line count={lines.Length},size ={script.Length}");
        }

        public JavaScriptDiagStepType OnDebugEvent(JavaScriptDiagDebugEvent eventType,string data, DebugEngine engine)
        {
            Console.WriteLine($"{eventType},{data}");
            switch (eventType)
            {
                case JavaScriptDiagDebugEvent.JsDiagDebugEventSourceCompile:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventCompileError:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventBreakpoint:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventStepComplete:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventDebuggerStatement:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventAsyncBreak:
                    engine.SetBreakpoint(3, 4, 0);
                    Console.WriteLine("break point set");
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventRuntimeException:
                    break;
                default:
                    break;
            }
            return JavaScriptDiagStepType.JsDiagStepTypeContinue;
        }

        public void ScriptReady(DebugEngine engine)
        {
            engine.RequestAsyncBreak();
            foreach (var item in engine.GetScripts())
            {
                Console.WriteLine($"GetScripts {item}");
            }
            
        }
    }
}
