using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChakraCore.NET.API;
using ChakraCore.NET.Debug;
using System.Linq;

namespace RunScript
{
    internal class VSCodeDebugAdapter : DebugSession, IDebugAdapter
    {
        private static int variableId=0;
        private struct VariableHandle
        {
            public int Id;
            public uint FrameId;
            public bool IsGlobal;
            public static VariableHandle Create(uint frameId,bool isGlobal)
            {
                return new VariableHandle()
                {
                    FrameId = frameId,
                    IsGlobal = isGlobal,
                    Id = variableId++
                };
            }
        }

        private Dictionary<int, VariableHandle> VariableHandles = new Dictionary<int, VariableHandle>();
        private Thread currentThread = new Thread(1, "Thread1");
        DebugEngine currentEngine;
        AutoResetEvent engineEventASE = new AutoResetEvent(false);
        AutoResetEvent vscodeEventASE = new AutoResetEvent(false);
        private Dictionary<string, string> sourceMap = new Dictionary<string, string>();
        private List<SourceCode> sourceCodeList = new List<SourceCode>();

        private int CreateVariableHandle(uint frameId,bool isGlobal)
        {
            VariableHandle handle = VariableHandle.Create(frameId, isGlobal);
            VariableHandles.Add(handle.Id, handle);
            return handle.Id;
        }
        public override void Attach(Response response, dynamic arguments)
        {
            throw new NotImplementedException();
        }
        private string mapSourceFromVSCode(string vscodeFileName)
        {
            return sourceMap.First(item => item.Value == vscodeFileName).Key;
        }

        private string mapSourceFromEngine(string engineFileName)
        {
            return sourceMap[engineFileName];
        }

        

        public override void Continue(Response response, dynamic arguments)
        {
            currentEngine.StepType = JavaScriptDiagStepType.JsDiagStepTypeContinue;
            vscodeEventASE.Set();
            SendResponse(response);
        }

        public override void Disconnect(Response response, dynamic arguments)
        {
            Console.WriteLine("Disconnect");
        }

        public override void Evaluate(Response response, dynamic arguments)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(Response response, dynamic args)
        {
            SendResponse(response, new Capabilities()
            {
                exceptionBreakpointFilters = new dynamic[0],
                supportsConditionalBreakpoints = false,
                supportsEvaluateForHovers = false,
                supportsConfigurationDoneRequest = true,
                supportsFunctionBreakpoints = false
            });
            Console.WriteLine("Initialize called");
            Console.WriteLine("Waiting for script ready");
            engineEventASE.WaitOne();
            Console.WriteLine("InitializedEvent Sent");
            SendEvent(new InitializedEvent());
            SendEvent(new OutputEvent(string.Empty, "InitializedEvent"));
        }

        public override void Launch(Response response, dynamic arguments)
        {
            Console.WriteLine("[Launch]");
            SendResponse(response);
            SendEvent(new OutputEvent(string.Empty, "Launch received"));
        }

        public override void Next(Response response, dynamic arguments)
        {
            currentEngine.StepType = JavaScriptDiagStepType.JsDiagStepTypeStepOver;
            SendResponse(response);
            vscodeEventASE.Set();
            
        }

        public override void Pause(Response response, dynamic arguments)
        {
            throw new NotImplementedException();
        }

        public override void Scopes(Response response, dynamic arguments)
        {
            uint frameid = (uint)arguments.frameId;
            List<Scope> scopes = new List<Scope>()
            {
                new Scope("Global",CreateVariableHandle(frameid,true)),
                new Scope("Local",CreateVariableHandle(frameid,false))
                

            };
            SendResponse(response, new ScopesResponseBody(scopes));
        }

        public override void SetBreakpoints(Response response, dynamic arguments)
        {
            //throw new NotImplementedException();
            string fileName = arguments.source.name;
            List<Breakpoint> bps = new List<Breakpoint>();
            if (findSourceCode(fileName, out var sourceCode))
            {
                sourceMap[sourceCode.FileName] = arguments.source.path;
                for (int i = 0; i < arguments.breakpoints.Count; i++)
                {
                    uint line = (uint)ConvertDebuggerLineToClient((int)arguments.breakpoints[i].line);
                    var t = currentEngine.SetBreakpointAsync(sourceCode.ScriptId, line, 0);
                    t.Wait();
                    BreakPoint bp = t.Result;
                    bps.Add(new Breakpoint(true, ConvertClientLineToDebugger( (int)bp.Line)));
                }
            }
            Console.WriteLine($"BreakPoint {fileName} Set");
            SendResponse(response, new SetBreakpointsResponseBody(bps));
            
        }

        private bool findSourceCode(string name, out SourceCode result)
        {
            foreach (var item in sourceCodeList)
            {
                if (item.FileName==name || item.FileName+".js"==name)
                {
                    result = item;
                    return true;
                }
            }
            result = new SourceCode();
            return false;
        }

        public override void Source(Response response, dynamic arguments)
        {
            throw new NotImplementedException();
        }

        public override void StackTrace(Response response, dynamic arguments)
        {
            List<StackFrame> frames = new List<StackFrame>();
            var t=currentEngine.GetStackTraceAsync();
            t.Wait();
            var sts = t.Result;
            foreach (var item in sts)
            {
                SourceCode engineSource = sourceCodeList.First(x => x.ScriptId == item.ScriptId);
                StackFrame frame = new StackFrame(
                    item.Index,
                    $"Frame{item.Index}",
                    new RunScript.Source(engineSource.FileName, mapSourceFromEngine(engineSource.FileName),0,"Normal"),
                    ConvertClientLineToDebugger( item.Line),
                    item.Column,
                    "Normal");
                frames.Add(frame);
            }

            SendResponse(response, new StackTraceResponseBody(frames, frames.Count));
        }

        public override void StepIn(Response response, dynamic arguments)
        {
            currentEngine.StepType = JavaScriptDiagStepType.JsDiagStepTypeStepIn;
            SendResponse(response);
            vscodeEventASE.Set();
        }

        public override void StepOut(Response response, dynamic arguments)
        {
            currentEngine.StepType = JavaScriptDiagStepType.JsDiagStepTypeStepOut;
            SendResponse(response);
            vscodeEventASE.Set();
        }

        public override void Threads(Response response, dynamic arguments)
        {
            Console.WriteLine("[Threads]");
            SendResponse(response, new ThreadsResponseBody(new List<Thread>() { currentThread }));
        }

        public override void Variables(Response response, dynamic arguments)
        {
            int variableReference = arguments.variablesReference;
            if (VariableHandles.ContainsKey(variableReference))
            {
                //variableReference is scope id
                SendResponse(response, loadVariablesFromScope(VariableHandles[variableReference]));
            }
            else
            {
                //variableReference is variable handle
                SendResponse(response, loadVariableProperties(variableReference));
            }
        }

        private VariablesResponseBody loadVariableProperties(int varibleHandle)
        {
            var t = currentEngine.GetObjectPropertiesAsync((uint)varibleHandle);
            t.Wait();
            var properties =from item in t.Result.Properties
                            select new Variable(item.Name, item.Display ?? item.Value, item.Type, (int)item.Handle);
            return new VariablesResponseBody(properties.ToList());
        }

        private VariablesResponseBody loadVariablesFromScope(VariableHandle handle)
        {
            var t = currentEngine.GetStackPropertiesAsync(handle.FrameId);
            t.Wait();
            if (handle.IsGlobal)
            {
                return new VariablesResponseBody(new List<Variable>());
            }
            else
            {
                var variables = from item in t.Result.Locals
                                select new Variable(item.Name, item.Display ?? item.Value, item.Type, (int)item.Handle);
                return new VariablesResponseBody(variables.ToList());
            }
        }

        internal override void ConfigurationDone(Response response, dynamic args)
        {
            Console.WriteLine("ConfigurationDone");
            vscodeEventASE.Set();
            SendResponse(response);
        }

        

        Task IDebugAdapter.OnBreakPoint(BreakPoint breakPoint, DebugEngine engine)
        {
            Console.WriteLine($"Breakpoint hit at {breakPoint}");
            SendEvent(new StoppedEvent(currentThread.id, "breakpoint","breakpoint hit"));
            currentEngine = engine;
            vscodeEventASE.WaitOne();
            return Task.CompletedTask;
        }

        Task IDebugAdapter.OnDebugEvent(JavaScriptDiagDebugEvent eventType, string data, DebugEngine engine)
        {
            Console.WriteLine($"[{eventType}],{data}");
            return Task.CompletedTask;
        }

        Task IDebugAdapter.ScriptReady(DebugEngine engine)
        {
            currentEngine = engine;
            Console.WriteLine("Script ready, Waiting for debugger");
            engineEventASE.Set();
            vscodeEventASE.WaitOne();
            Console.WriteLine("Script continue running");
            return Task.CompletedTask;
        }

        void IDebugAdapter.AddScript(SourceCode sourceCode)
        {
            sourceCodeList.Add(sourceCode);
            sourceMap.Add(sourceCode.FileName, string.Empty);
        }


        Task IDebugAdapter.OnStep(BreakPoint breakPoint, DebugEngine engine)
        {
            Console.WriteLine($"Step complete at {breakPoint}");
            SendEvent(new StoppedEvent(currentThread.id, "step"));
            currentEngine = engine;
            vscodeEventASE.WaitOne();
            return Task.CompletedTask;
        }
    }
}
