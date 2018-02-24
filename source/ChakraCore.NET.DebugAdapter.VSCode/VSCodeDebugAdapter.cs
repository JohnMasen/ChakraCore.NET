using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChakraCore.NET.API;
using ChakraCore.NET.Debug;
using System.Linq;

namespace ChakraCore.NET.DebugAdapter.VSCode
{
    public class VSCodeDebugAdapter : DebugSession, IDebugAdapter
    {
        public event EventHandler<string> OnAdapterMessage;
        private Thread currentThread = new Thread(1, "Thread1");
        DebugEngine currentEngine;
        ManualResetEvent scriptReadyMSE = new ManualResetEvent(false);
        AutoResetEvent engineEventASE = new AutoResetEvent(false);
        AutoResetEvent vscodeEventASE = new AutoResetEvent(false);
        private List<SourceCode> sourceCodeList = new List<SourceCode>();
        private Dictionary<string, string> sourceMap = new Dictionary<string, string>();

        VSCodeDebugVariableReferenceManager handleManager = new VSCodeDebugVariableReferenceManager();
        public override void Attach(Response response, dynamic arguments)
        {
            OnAdapterMessage?.Invoke(this,"[Attach]");
            sourceMap.Clear();
            if (arguments.sourceMap != null)
            {
                foreach (var item in arguments.sourceMap)
                {
                    sourceMap.Add((string)item.name,(string)item.value);
                }
            }
            SendResponse(response);
            SendEvent(new OutputEvent(string.Empty, "Attach received"));
        }


        public override void Continue(Response response, dynamic arguments)
        {
            currentEngine.StepType = JavaScriptDiagStepType.JsDiagStepTypeContinue;
            vscodeEventASE.Set();
            SendResponse(response);
        }

        public override void Disconnect(Response response, dynamic arguments)
        {
            OnAdapterMessage?.Invoke(this,"Disconnect");
            vscodeEventASE.Set();
        }

        public override void Evaluate(Response response, dynamic arguments)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(Response response, dynamic args)
        {
            OnAdapterMessage?.Invoke(this,"Initialize called");
            SendResponse(response, new Capabilities()
            {
                exceptionBreakpointFilters = new dynamic[0],
                supportsConditionalBreakpoints = false,
                supportsEvaluateForHovers = false,
                supportsConfigurationDoneRequest = true,
                supportsFunctionBreakpoints = false,
                supportsLoadedSourcesRequest = true
            });
            
            OnAdapterMessage?.Invoke(this,"Waiting for script ready");
            scriptReadyMSE.WaitOne();
            OnAdapterMessage?.Invoke(this,"InitializedEvent Sent");
            SendEvent(new InitializedEvent());
            //SendEvent(new OutputEvent(string.Empty, "InitializedEvent"));
        }

        public override void Launch(Response response, dynamic arguments)
        {
            throw new NotSupportedException("Launch is not supported");
        }

        private void sendLoadedScripts()
        {
            foreach (var item in sourceCodeList)
            {
                SendEvent(new LoadedSourceEvent("new", new Source(item.FileName,item.FileName,(int)item.ScriptId,"source hint")));
            }
            OnAdapterMessage?.Invoke(this,"Source code sent to VSCode");
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
                new Scope("Local",handleManager.Create(frameid,VariableScopeEnum.Local).Id),//first item will be expanded by default
                new Scope("Globals",handleManager.Create(frameid,VariableScopeEnum.Globals).Id),
                new Scope("Scopes",handleManager.Create(frameid,VariableScopeEnum.Scopes).Id)
            };
            SendResponse(response, new ScopesResponseBody(scopes));
        }

        public override void SetBreakpoints(Response response, dynamic arguments)
        {
            string fileName = arguments.source.name;
            List<Breakpoint> bps = new List<Breakpoint>();
            if (findSourceCode(fileName, out var sourceCode))
            {
                currentEngine.ClearBreakPointOnScript(sourceCode.ScriptId).Wait();
                for (int i = 0; i < arguments.breakpoints.Count; i++)
                {
                    uint line = (uint)ConvertDebuggerLineToClient((int)arguments.breakpoints[i].line);
                    var t = currentEngine.SetBreakpointAsync(sourceCode.ScriptId, line, 0);
                    t.Wait();
                    BreakPoint bp = t.Result;
                    bps.Add(new Breakpoint(true, ConvertClientLineToDebugger( (int)bp.Line)));
                }
            }
            OnAdapterMessage?.Invoke(this,$"BreakPoints on {fileName} Set");
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
            uint id = arguments.source.sourceReference;
            var t=currentEngine.GetScriptSourceAsync(id);
            t.Wait();
            SendResponse(response, new SourceResponseBody(t.Result.Source));
        }

        public override void StackTrace(Response response, dynamic arguments)
        {
            List<StackFrame> frames = new List<StackFrame>();
            var t=currentEngine.GetStackTraceAsync();
            t.Wait();
            var sts = t.Result;
            
            foreach (var item in sts)
            {
                var t1= currentEngine.GetObjectFromHandleAsync(item.FunctionHandle);
                t1.Wait();
                string functionName = t1.Result.Name;
                SourceCode engineSource = sourceCodeList.First(x => x.ScriptId == item.ScriptId);
                Source source;
                if (sourceMap.ContainsKey(engineSource.FileName))
                {
                    source = new Source(engineSource.FileName, sourceMap[engineSource.FileName], 0, "file");
                }
                else
                {
                    source = new Source(engineSource.FileName, string.Empty,(int)engineSource.ScriptId, "dynamic load");
                }
                StackFrame frame = new StackFrame(
                    item.Index,
                    functionName,
                    source,
                    ConvertClientLineToDebugger( item.Line),
                    item.Column,
                    string.Empty);
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
            OnAdapterMessage?.Invoke(this,"[Threads]");
            handleManager.Reset();
            SendResponse(response, new ThreadsResponseBody(new List<Thread>() { currentThread }));
        }

        public override void Variables(Response response, dynamic arguments)
        {
            int variableReference = arguments.variablesReference;
            if (handleManager.TryGet(variableReference,out var handle))
            {
                //variableReference is scope
                SendResponse(response, loadVariablesFromScope(handle));
            }
            else
            {
                //variableReference is variable handle
                SendResponse(response, loadVariableProperties(variableReference));
            }

            
        }

        private VariablesResponseBody loadVariableProperties(int varibleHandle)
        {
            return loadVariableProperties((uint)varibleHandle);
        }

        private VariablesResponseBody loadVariableProperties(uint varibleHandle)
        {
            if (varibleHandle==0)//if Eval function in script, the argument handle will be 0
            {
                return null;
            }
            var t = currentEngine.GetObjectPropertiesAsync(varibleHandle);
            t.Wait();
            var properties = from item in t.Result.Properties
                             select item.ToVSCodeVarible();
            return new VariablesResponseBody(properties.ToList());
        }

        private VariablesResponseBody loadVariablesFromScope(VariableHandle handle)
        {
            var t = currentEngine.GetStackPropertiesAsync(handle.FrameId);
            t.Wait();

            switch (handle.Scope)
            {
                case VariableScopeEnum.Local:
                    var variables = (from item in t.Result.Locals
                                    select item.ToVSCodeVarible()).ToList();
                    variables.Insert(0,t.Result.ThisObject.ToVSCodeVarible("[{0}]"));
                    if (t.Result.Arguments.Handle!=0)
                    {
                        variables.Insert(1,t.Result.Arguments.ToVSCodeVarible());
                    }
                    return new VariablesResponseBody(variables);
                case VariableScopeEnum.Globals:
                    return loadVariableProperties(t.Result.Global.Handle);
                case VariableScopeEnum.Scopes:
                    if (t.Result.Scopes.Length>0)
                    {
                        List<Variable> scopes = new List<Variable>();
                        foreach (var item in t.Result.Scopes)
                        {
                            scopes.Add(new Variable($"Scope #{item.Index}", string.Empty, string.Empty,(int)item.Handle));
                        }
                        return new VariablesResponseBody(scopes);
                    }
                    else
                    {
                        return null;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(handle), "Invalid scope type");
            }

        }

        internal override void ConfigurationDone(Response response, dynamic args)
        {
            
            OnAdapterMessage?.Invoke(this,"ConfigurationDone");
            vscodeEventASE.Set();
            SendResponse(response);
        }

        

        Task IDebugAdapter.OnBreakPoint(BreakPoint breakPoint, DebugEngine engine)
        {
            OnAdapterMessage?.Invoke(this,$"Breakpoint hit at {breakPoint}");
            currentEngine = engine;
            SendEvent(new StoppedEvent(currentThread.id, "breakpoint","breakpoint hit"));
            vscodeEventASE.WaitOne();
            return Task.CompletedTask;
        }

        Task IDebugAdapter.OnDebugEvent(JavaScriptDiagDebugEvent eventType, string data, DebugEngine engine)
        {
            OnAdapterMessage?.Invoke(this,$"[{eventType}],{data}");
            return Task.CompletedTask;
        }

        Task IDebugAdapter.ScriptReady(DebugEngine engine)
        {
            currentEngine = engine;
            OnAdapterMessage?.Invoke(this,"Script ready, Waiting for debugger");
            scriptReadyMSE.Set();
            vscodeEventASE.WaitOne();
            OnAdapterMessage?.Invoke(this,"Script continue running");
            return Task.CompletedTask;
        }

        void IDebugAdapter.AddScript(SourceCode sourceCode)
        {
            sourceCodeList.Add(sourceCode);
            OnAdapterMessage?.Invoke(this,$"Script {sourceCode.FileName} Loaded");
        }


        Task IDebugAdapter.OnStep(BreakPoint breakPoint, DebugEngine engine)
        {
            OnAdapterMessage?.Invoke(this,$"Step complete at {breakPoint}");
            currentEngine = engine;
            SendEvent(new StoppedEvent(currentThread.id, "step"));
            vscodeEventASE.WaitOne();
            return Task.CompletedTask;
        }

        Task IDebugAdapter.OnException(RuntimeException exception, DebugEngine engine)
        {
            OnAdapterMessage?.Invoke(this,$"Exception occured at  {exception}");
            currentEngine = engine;
            SendEvent(new StoppedEvent(currentThread.id, "exception",exception.ExceptionObject.Display));
            vscodeEventASE.WaitOne();
            return Task.CompletedTask;
        }

    }
}
