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
            throw new NotImplementedException();
        }

        public override void Pause(Response response, dynamic arguments)
        {
            throw new NotImplementedException();
        }

        public override void Scopes(Response response, dynamic arguments)
        {
            List<Scope> scopes = new List<Scope>()
            {
                new Scope("Local",CreateVariableHandle(arguments.frameId,false)),
                new Scope("Global",CreateVariableHandle(arguments.frameId,true))

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
            throw new NotImplementedException();
        }

        public override void StepOut(Response response, dynamic arguments)
        {
            throw new NotImplementedException();
        }

        public override void Threads(Response response, dynamic arguments)
        {
            Console.WriteLine("[Threads]");
            SendResponse(response, new ThreadsResponseBody(new List<Thread>() { currentThread }));
        }

        public override void Variables(Response response, dynamic arguments)
        {
            VariableHandle handle = VariableHandles[arguments.variablesReference];
            var t=currentEngine.GetStackPropertiesAsync(handle.FrameId);
            t.Wait();
            foreach (var item in t.Result.Locals)
            {

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

        async Task IDebugAdapter.ScriptReady(DebugEngine engine)
        {
            currentEngine = engine;
            Console.WriteLine("Script ready");
            engineEventASE.Set();
            vscodeEventASE.WaitOne();
            //engine.RequestAsyncBreak();
            //await engine.SetBreakpointAsync(3, 8, 0);
            //await engine.SetBreakpointAsync(3, 9, 0);
            Console.WriteLine("Script continue running");
        }

        void IDebugAdapter.AddScript(SourceCode sourceCode)
        {
            sourceCodeList.Add(sourceCode);
            sourceMap.Add(sourceCode.FileName, string.Empty);
        }
    }
}
