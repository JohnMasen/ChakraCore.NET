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
        private Thread currentThread = new Thread(1, "Thread #1");
        IRuntimeDebuggingService debuggingService;
        ManualResetEvent configurationDoneEvent = new ManualResetEvent(false);
        ManualResetEvent engineReadyEvent = new ManualResetEvent(false);
        private Action postAsyncBreakAction = null;
        private Action postConfigurationDoneAction = null;
        private List<SourceCode> sourceCodeList = new List<SourceCode>();
        private Dictionary<string, string> sourceMap = new Dictionary<string, string>();
        private bool waitForLaunch = false;
        VSCodeDebugVariableReferenceManager handleManager = new VSCodeDebugVariableReferenceManager();
        private DebugAdapterStatusEnum currentStatus = DebugAdapterStatusEnum.WaitingForEngineReady;
        public event EventHandler<OnStatusChangArguments> OnStatusChang;
        public bool IsConnected { get; private set; } = false;
        public VSCodeDebugAdapter(bool waitForLaunch=true)
        {
            this.waitForLaunch = waitForLaunch;
        }

        public DebugAdapterStatusEnum DebugAdapterStatus {
            get
            {
                return currentStatus;
            } private set
            {
                switch (value)
                {
                    case DebugAdapterStatusEnum.WaitingForEngineReady:
                        break;
                    case DebugAdapterStatusEnum.Ready:
                        if (DebugAdapterStatus == DebugAdapterStatusEnum.WaitingForEngineReady)
                        {
                            engineReadyEvent.Set();
                        }
                        break;
                    case DebugAdapterStatusEnum.BreakPointHit:
                    case DebugAdapterStatusEnum.AsyncBreakHit:
                    case DebugAdapterStatusEnum.ExceptionOccured:
                        break;
                    default:
                        break;
                }
                OnStatusChang?.Invoke(this, new OnStatusChangArguments(currentStatus, value));
                currentStatus = value;
            }
        } 

        private void sendLoadedScriptInfo()
        {
            foreach (var item in sourceCodeList.Where(x=>!(string.IsNullOrEmpty(x.FileName) || x.FileName.StartsWith("<"))))
            {
                SendOutput($"Script {item.FileName} loaded");
            }
        }
        public override void Attach(Response response, dynamic arguments)
        {
            switch (DebugAdapterStatus)
            {
                case DebugAdapterStatusEnum.WaitingForEngineReady:
                    throw new InvalidOperationException("Attach failed, engine is waiting for Launch");
                
                case DebugAdapterStatusEnum.BreakPointHit:
                case DebugAdapterStatusEnum.StepComplete:
                case DebugAdapterStatusEnum.AsyncBreakHit:
                case DebugAdapterStatusEnum.ExceptionOccured:
                    throw new InvalidOperationException($"Attach failed, engine is busy. Status={DebugAdapterStatus}");
                case DebugAdapterStatusEnum.Ready:
                    ManualResetEventSlim mse = new ManualResetEventSlim(false);
                    postAsyncBreakAction = mse.Set;
                    postConfigurationDoneAction = () => //continue the script excution when VSCode configuration is done
                    {
                        debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeContinue);
                    };
                    debuggingService.RequestAsyncBreak();
                    SendOutput("AsyncBreak request sent, waiting for engine response");
                    mse.Wait();
                    SendOutput("AsyncBreak hit, continue Attach progress");
                    break;
                default:
                    break;
            }

            OnAdapterMessage?.Invoke(this, "[Attach]");
            sourceMap.Clear();
            if (arguments.sourceMap != null)
            {
                foreach (var item in arguments.sourceMap)
                {
                    sourceMap.Add((string)item.name, (string)item.value);
                }
            }
            SendResponse(response);
        }


        public override void Continue(Response response, dynamic arguments)
        {
            debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeContinue);
            SendResponse(response);
            DebugAdapterStatus = DebugAdapterStatusEnum.Ready;
        }

        public override void Disconnect(Response response, dynamic arguments)
        {
            switch (DebugAdapterStatus)
            {
                case DebugAdapterStatusEnum.BreakPointHit:
                case DebugAdapterStatusEnum.StepComplete:
                case DebugAdapterStatusEnum.AsyncBreakHit:
                case DebugAdapterStatusEnum.ExceptionOccured:
                    debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeContinue);
                    DebugAdapterStatus = DebugAdapterStatusEnum.Ready;
                    break;
                default:
                    break;
            }
            SendOutput("Debugger disconnected, continue script execution");
            IsConnected = false;
            OnAdapterMessage?.Invoke(this, "Disconnect");
        }

        public override void Evaluate(Response response, dynamic arguments)
        {
            var t= debuggingService.Evaluate((string)arguments.expression, (uint)arguments.frameId, false);
            var x = t.ToVSCodeVarible();
            if (t.ClassName == "Error")
            {
                SendResponse(response, new EvaluateResponseBody(x.value, x.type, 0));
            }
            else
            {
                SendResponse(response, new EvaluateResponseBody(x.value, x.type, x.variablesReference));
            }
            
            
        }

        public override void Initialize(Response response, dynamic args)
        {
            IsConnected = true;
            OnAdapterMessage?.Invoke(this, "Initialize called");
            SendResponse(response, new Capabilities()
            {
                exceptionBreakpointFilters = new dynamic[0],
                supportsConditionalBreakpoints = false,
                supportsEvaluateForHovers = false,
                supportsConfigurationDoneRequest = true,
                supportsFunctionBreakpoints = false,
                supportsLoadedSourcesRequest = true
            });

            OnAdapterMessage?.Invoke(this, "Waiting for script ready");
            engineReadyEvent.WaitOne();
            OnAdapterMessage?.Invoke(this, "InitializedEvent Sent");
            SendEvent(new InitializedEvent());
        }

        public override void Launch(Response response, dynamic arguments)
        {
            OnAdapterMessage?.Invoke(this, "[Launch]");
            if (DebugAdapterStatus!= DebugAdapterStatusEnum.WaitingForEngineReady)
            {
                throw new InvalidOperationException("Launch failed, engine already running");
            }
            sourceMap.Clear();
            if (arguments.sourceMap != null)
            {
                foreach (var item in arguments.sourceMap)
                {
                    sourceMap.Add((string)item.name, (string)item.value);
                }
            }
            bool pauseOnLaunch = arguments.pauseOnLaunch ?? false;
            if (pauseOnLaunch)
            {
                debuggingService.RequestAsyncBreak();
            }
            SendResponse(response);
        }

        

        public override void Next(Response response, dynamic arguments)
        {
            debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeStepOver);
            SendResponse(response);
            DebugAdapterStatus = DebugAdapterStatusEnum.Ready;
        }

        public override void Pause(Response response, dynamic arguments)
        {
            debuggingService.RequestAsyncBreak();
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
                clearBreakpointOnScript(sourceCode.ScriptId);
                for (int i = 0; i < arguments.breakpoints.Count; i++)
                {
                    uint line = (uint)ConvertDebuggerLineToClient((int)arguments.breakpoints[i].line);
                    BreakPoint bp = debuggingService.SetBreakpoint(sourceCode.ScriptId, line, 0);
                    bps.Add(new Breakpoint(true, ConvertClientLineToDebugger((int)bp.Line)));
                }
            }
            OnAdapterMessage?.Invoke(this, $"BreakPoints on {fileName} Set");
            SendResponse(response, new SetBreakpointsResponseBody(bps));

        }

        private bool findSourceCode(string name, out SourceCode result)
        {
            foreach (var item in sourceCodeList)
            {
                if (item.FileName == name || item.FileName + ".js" == name)
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
            var t = debuggingService.GetScriptSource(id);
            SendResponse(response, new SourceResponseBody(t.Source));
        }

        public override void StackTrace(Response response, dynamic arguments)
        {
            List<StackFrame> frames = new List<StackFrame>();
            var sts = debuggingService.GetStackTrace();

            foreach (var item in sts)
            {
                var t1 = debuggingService.GetObjectFromHandle(item.FunctionHandle);
                string functionName = t1.Name;
                SourceCode engineSource = sourceCodeList.First(x => x.ScriptId == item.ScriptId);
                Source source;
                if (engineSource.FileName != null && sourceMap.ContainsKey(engineSource.FileName))//eval() code does not have FileName
                {
                    source = new Source(engineSource.FileName, sourceMap[engineSource.FileName], 0, "normal");
                }
                else
                {
                    source = new Source(engineSource.FileName ?? functionName, string.Empty, (int)engineSource.ScriptId, "normal");
                }
                StackFrame frame = new StackFrame(
                    item.Index,
                    functionName,
                    source,
                    ConvertClientLineToDebugger(item.Line),
                    item.Column,
                    string.Empty);
                frames.Add(frame);
            }

            SendResponse(response, new StackTraceResponseBody(frames, frames.Count));
        }


        public override void StepIn(Response response, dynamic arguments)
        {
            debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeStepIn);
            SendResponse(response);
            DebugAdapterStatus = DebugAdapterStatusEnum.Ready;
        }

        public override void StepOut(Response response, dynamic arguments)
        {
            debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeStepOut);
            SendResponse(response);
            DebugAdapterStatus = DebugAdapterStatusEnum.Ready;
        }

        public override void Threads(Response response, dynamic arguments)
        {
            OnAdapterMessage?.Invoke(this, "[Threads]");
            handleManager.Reset();
            SendResponse(response, new ThreadsResponseBody(new List<Thread>() { currentThread }));
        }

        public override void Variables(Response response, dynamic arguments)
        {
            int variableReference = arguments.variablesReference;
            if (handleManager.TryGet(variableReference, out var handle))
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

        private VariablesResponseBody loadVariableProperties(uint varibleHandle,uint from=0, uint to=99)
        {
            var t = debuggingService.GetProperties(varibleHandle,from,to);
            var properties = from item in t.Properties
                             select item.ToVSCodeVarible();
            return new VariablesResponseBody(properties.ToList());
        }

        private VariablesResponseBody loadVariablesFromScope(VariableHandle handle)
        {
            var t = debuggingService.GetStackProperties(handle.FrameId);

            switch (handle.Scope)
            {
                case VariableScopeEnum.Local:
                    var variables = (from item in t.Locals
                                     select item.ToVSCodeVarible()).ToList();
                    variables.Insert(0, t.ThisObject.ToVSCodeVarible("[{0}]"));
                    if (t.Arguments.Handle != 0)
                    {
                        variables.Insert(1, t.Arguments.ToVSCodeVarible());
                    }
                    return new VariablesResponseBody(variables);
                case VariableScopeEnum.Globals:
                    return loadVariableProperties(t.Global.Handle);
                case VariableScopeEnum.Scopes:
                    if (t.Scopes.Length > 0)
                    {
                        List<Variable> scopes = new List<Variable>();
                        foreach (var item in t.Scopes)
                        {
                            scopes.Add(new Variable($"Scope #{item.Index}", string.Empty, string.Empty, (int)item.Handle));
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

            OnAdapterMessage?.Invoke(this, "ConfigurationDone");
            if (waitForLaunch)
            {
                configurationDoneEvent.Set();
            }
            if (postConfigurationDoneAction != null)
            {
                postConfigurationDoneAction();
                postConfigurationDoneAction = null;
            }
            sendLoadedScriptInfo();
            SendResponse(response);
            
        }

        

        public void Init(IRuntimeDebuggingService debuggingService)
        {
            this.debuggingService = debuggingService;
            debuggingService.StartDebug();
            debuggingService.OnAsyncBreak += DebuggingService_OnAsyncBreak;
            debuggingService.OnBreakPoint += DebuggingService_OnBreakPoint;
            debuggingService.OnDebugEvent += DebuggingService_OnDebugEvent;
            debuggingService.OnEngineReady += DebuggingService_OnEngineReady;
            debuggingService.OnException += DebuggingService_OnException;
            debuggingService.OnScriptLoad += DebuggingService_OnScriptLoad;
            debuggingService.OnStepComplete += DebuggingService_OnStepComplete;
        }

        private void DebuggingService_OnStepComplete(object sender, BreakPoint e)
        {
            OnAdapterMessage?.Invoke(this, $"Step complete at {e}");
            SendEvent(new StoppedEvent(currentThread.id, "step"));
            DebugAdapterStatus = DebugAdapterStatusEnum.StepComplete;
        }

        private void DebuggingService_OnScriptLoad(object sender, SourceCode e)
        {
            sourceCodeList.Add(e);
            OnAdapterMessage?.Invoke(this, $"Script {e.FileName} Loaded");
        }

        private void DebuggingService_OnException(object sender, RuntimeException e)
        {
            if (!IsConnected)
            {
                debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeContinue);
                return;
            }
            OnAdapterMessage?.Invoke(this, $"Exception occured at  {e}");
            SendEvent(new StoppedEvent(currentThread.id, "exception", e.ExceptionObject.Display));
            DebugAdapterStatus = DebugAdapterStatusEnum.ExceptionOccured;
        }

        private void DebuggingService_OnEngineReady(object sender, EventArgs e)
        {
            OnAdapterMessage?.Invoke(this, "Script ready, Waiting for debugger");
            engineReadyEvent.Set();
            if (waitForLaunch)
            {
                configurationDoneEvent.WaitOne();
            }
            OnAdapterMessage?.Invoke(this, "Script continue running");
            DebugAdapterStatus = DebugAdapterStatusEnum.Ready;
        }

        private void DebuggingService_OnDebugEvent(object sender, DebugEventArguments e)
        {
            OnAdapterMessage?.Invoke(this, $"[{e.EventType}],{e.EventData}");
        }

        private void DebuggingService_OnBreakPoint(object sender, BreakPoint e)
        {
            if (!IsConnected)
            {
                debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeContinue);
                return;
            }
            OnAdapterMessage?.Invoke(this, $"Breakpoint hit at {e}");
            SendEvent(new StoppedEvent(currentThread.id, "breakpoint", "breakpoint hit"));
            DebugAdapterStatus = DebugAdapterStatusEnum.BreakPointHit;
        }

        private void DebuggingService_OnAsyncBreak(object sender, BreakPoint e)
        {
            if (!IsConnected)
            {
                debuggingService.Step(JavaScriptDiagStepType.JsDiagStepTypeContinue);
                return;
            }
            OnAdapterMessage?.Invoke(this, $"Async break at  {e}");
            if (postAsyncBreakAction != null)//event is triggered by Attach command
            {
                postAsyncBreakAction();
                postAsyncBreakAction = null;
            }
            else
            {
                SendEvent(new StoppedEvent(currentThread.id, "async break"));
            }
            
            DebugAdapterStatus = DebugAdapterStatusEnum.AsyncBreakHit;
        }

        private void clearBreakpointOnScript(uint scriptId)
        {
            var bps = debuggingService.GetBreakpoints();
            foreach (var item in bps.Where(x => x.ScriptId == scriptId))
            {
                debuggingService.RemoveBreakpoint(item.BreakpointId);
            }
        }
    }
}
