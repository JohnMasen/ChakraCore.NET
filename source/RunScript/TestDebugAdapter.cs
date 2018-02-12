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
            Console.WriteLine($"[{eventType}],{data}");
            switch (eventType)
            {
                case JavaScriptDiagDebugEvent.JsDiagDebugEventSourceCompile:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventCompileError:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventBreakpoint:
                    Console.WriteLine($"Stack trace:{engine.GetStackTrace()}");
                    Console.WriteLine($"Stack properties:{engine.GetStackProperties(0)}");
                    Console.WriteLine($"Object 7:{engine.GetObjectFromHandle(7)}");
                    Console.WriteLine($"Object Properties of 7:{engine.GetObjectProperties(7)}");
                    string eval = "o.d";
                    Console.WriteLine($"eval '{eval}':{engine.Evaluate(eval,0,false)}");
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventStepComplete:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventDebuggerStatement:
                    break;
                case JavaScriptDiagDebugEvent.JsDiagDebugEventAsyncBreak:
                    engine.SetBreakpoint(3, 6, 0);
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
                Console.WriteLine($"GetScripts {engine.GetScripts()}");
            
        }
    }
}
