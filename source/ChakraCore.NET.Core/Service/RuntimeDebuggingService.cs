using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChakraCore.NET.API;
using ChakraCore.NET.Debug;
using Newtonsoft.Json;

namespace ChakraCore.NET
{
    public class RuntimeDebuggingService : ServiceBase, IRuntimeDebuggingService
    {
        JavaScriptRuntime runtime;
        IDebugAdapter currentAdapter;
        public bool IsDebugging { get; private set; }
        JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();
        private JsDiagDebugEventCallback debugCallback;
        public RuntimeDebuggingService(JavaScriptRuntime runtime)
        {
            this.runtime = runtime;
            debugCallback = onDebugEvent;
        }

        private void onDebugEvent(JavaScriptDiagDebugEvent debugEvent, JavaScriptValue eventData, IntPtr callbackState)
        {
            if (IsDebugging)
            {
                string data = null;
                if (eventData.IsValid)
                {
                    data = eventData.ToJsonString();
                }
                DebugEngine engine;
                switch (debugEvent)
                {
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventSourceCompile:
                        currentAdapter.AddScript(JsonConvert.DeserializeObject<SourceCode>(data));
                        break;
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventCompileError:
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventDebuggerStatement:
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventStepComplete:
                    
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventRuntimeException:
                        callWithEngine((e) =>
                        {
                            return currentAdapter.OnDebugEvent(debugEvent, data, e);
                        });
                        break;
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventBreakpoint:
                        engine = callWithEngine((e) =>
                        {
                            return currentAdapter.OnBreakPoint(JsonConvert.DeserializeObject<BreakPoint>(data),e);
                        });
                        SetStepType(engine.StepType);
                        break;
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventAsyncBreak:
                        engine = callWithEngine((e) =>
                        {
                            return currentAdapter.OnDebugEvent(debugEvent, data, e);
                        });
                        SetStepType(engine.StepType);
                        break;
                    default:
                        break;
                }
                
            }
        }
        private DebugEngine callWithEngine(Func<DebugEngine,Task> func)
        {
            DebugEngine engine = new DebugEngine(this);
            Task.Factory.StartNew(() => {
                var t = func(engine);
                t.ContinueWith(_ => { engine.StopProcessing(); });
            });
            
            engine.StartProcessing();
            return engine;
        }

        //public void AddScriptSource(string name, string content)
        //{
        //    currentAdapter?.AddScript(name, content);
        //}

        public void AttachAdapter(IDebugAdapter adapter)
        {
            if (IsDebugging)
            {
                throw new InvalidOperationException("Debugging is already in progress, detach current adapter first");
            }
            Native.ThrowIfError(Native.JsDiagStartDebugging(runtime, debugCallback, IntPtr.Zero));
            IsDebugging = true;
            currentAdapter = adapter;
        }

        public void DetachAdapter()
        {
            if (currentAdapter != null)
            {
                Native.ThrowIfError(Native.JsDiagStopDebugging(runtime, out IntPtr callbackState));
            }
            IsDebugging = false;
            currentAdapter = null;
        }

        public JavaScriptValue Evaluate(string expression, uint stackFrameIndex, bool forceSetValueProp)
        {
            JavaScriptValue exp = JavaScriptValue.FromString(expression);

            Native.ThrowIfError(Native.JsDiagEvaluate(exp, stackFrameIndex, JavaScriptParseScriptAttributes.JsParseScriptAttributeNone, forceSetValueProp, out JavaScriptValue result));
            return result;

        }

        public BreakPoint[] GetBreakpoints()
        {
            Native.ThrowIfError(Native.JsDiagGetBreakpoints(out JavaScriptValue result));
            var json = result.ToJsonString();
            return JsonConvert.DeserializeObject<BreakPoint[]>(json);
        }

        public JavaScriptValue GetObjectFromHandle(uint objectHandle)
        {
            Native.ThrowIfError(Native.JsDiagGetObjectFromHandle(objectHandle, out JavaScriptValue result));
            return result;
        }

        public string GetProperties(uint objectHandle, uint from, uint to)
        {
            Native.ThrowIfError(Native.JsDiagGetProperties(objectHandle, from, to, out JavaScriptValue result));
            return result.ToJsonString();
        }

        public string GetScriptSource(uint scriptId)
        {

            Native.ThrowIfError(Native.JsDiagGetSource(scriptId, out JavaScriptValue source));
            return source.ToString();
        }

        public string GetScripts()
        {
            IJSValueConverterService converter = CurrentNode.GetService<IJSValueConverterService>();

            Native.ThrowIfError(Native.JsDiagGetScripts(out JavaScriptValue result));
            return result.ToJsonString();


        }

        public string GetStackProperties(uint stackFrameIndex)
        {
            Native.ThrowIfError(Native.JsDiagGetStackProperties(stackFrameIndex, out JavaScriptValue result));
            return result.ToJsonString();
        }

        public string GetStackTrace()
        {
            Native.ThrowIfError(Native.JsDiagGetStackTrace(out JavaScriptValue result));
            return result.ToJsonString();
        }

        public void RemoveBreakpoint(uint breakpointId)
        {
            Native.ThrowIfError(Native.JsDiagRemoveBreakpoint(breakpointId));
        }

        public void ScriptReady()
        {

            if (IsDebugging)
            {
                CurrentNode.WithContext(() =>
                {
                    callWithEngine(e =>
                    {
                        return currentAdapter.ScriptReady(e);
                    });
                });
            }
        }

        public BreakPoint SetBreakpoint(uint scriptId, uint line, uint column)
        {

            Native.ThrowIfError(Native.JsDiagSetBreakpoint(scriptId, line, column, out JavaScriptValue breakpoint));
            string json= breakpoint.ToJsonString();
            return JsonConvert.DeserializeObject<BreakPoint>(json);
        }

        public void SetBreakpointOnException(JavaScriptDiagBreakOnExceptionAttributes attributes)
        {
            Native.ThrowIfError(Native.JsDiagSetBreakOnException(runtime, attributes));
        }

        public void SetStepType(JavaScriptDiagStepType stepType)
        {
            Native.ThrowIfError(Native.JsDiagSetStepType(stepType));
        }

        //private void WithInternalContext(Action a)
        //{
        //    CurrentNode.GetService<IRuntimeService>().InternalContextSwitchService.With(a);
        //}

        //private T WithInternalContext<T>(Func<T> func)
        //{
        //    return CurrentNode.GetService<IRuntimeService>().InternalContextSwitchService.With(func);
        //}

        public void RequestAsyncBreak()
        {
            Native.ThrowIfError(Native.JsDiagRequestAsyncBreak(runtime));
        }


        public JavaScriptSourceContext GetScriptContext(string name, string script)
        {
            sourceContext++;
            return sourceContext;
        }
    }
}
