using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChakraCore.NET.API;
using ChakraCore.NET.Debug;

namespace RunScript
{
    internal class VSCodeDebugAdapter : DebugSession, IDebugAdapter
    {
        private Thread currentThread = new Thread(1, "Thread1");
        DebugEngine currentEngine;
        AutoResetEvent engineEventASE = new AutoResetEvent(false);
        AutoResetEvent vscodeEventASE = new AutoResetEvent(false);
        

        private List<SourceCode> sourceCodeList = new List<SourceCode>();
        public override void Attach(Response response, dynamic arguments)
        {
            throw new NotImplementedException();
        }

        public override void Continue(Response response, dynamic arguments)
        {
            currentEngine.StepType = JavaScriptDiagStepType.JsDiagStepTypeContinue;
            vscodeEventASE.Set();
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
                supportsEvaluateForHovers = true,
                supportsConfigurationDoneRequest = true,
                supportsFunctionBreakpoints = false
            });
            Console.WriteLine("Initialize called");
            Console.WriteLine("Waiting for script ready");
            engineEventASE.WaitOne();
            Console.WriteLine("InitializedEvent Sent");
            SendEvent(new InitializedEvent());
        }

        public override void Launch(Response response, dynamic arguments)
        {
            Console.WriteLine("[Launch]");
            SendEvent(new OutputEvent(string.Empty, "Launch received"));
            SendResponse(response);
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
            throw new NotImplementedException();
        }

        public override void SetBreakpoints(Response response, dynamic arguments)
        {
            //throw new NotImplementedException();
            string fileName = arguments.source.name;
            List<Breakpoint> bps = new List<Breakpoint>();
            if (findSourceCode(fileName, out var sourceCode))
            {
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            SendEvent(new StoppedEvent(1, "breakpoint","breakpoint hit"));
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
        }
    }
}
