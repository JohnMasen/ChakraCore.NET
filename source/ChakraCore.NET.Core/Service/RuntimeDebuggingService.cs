using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;
using ChakraCore.NET.Debug;

namespace ChakraCore.NET
{
    public class RuntimeDebuggingService : ServiceBase, IRuntimeDebuggingService
    {
        JavaScriptRuntime runtime;
        IDebugAdapter currentAdapter;
        public bool IsDebugging { get; private set; }
        DebugEngine engine;
        JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();
        public RuntimeDebuggingService(JavaScriptRuntime runtime)
        {
            this.runtime = runtime;
            engine = new DebugEngine(this);
        }

        private void onDebugEvent(JavaScriptDiagDebugEvent debugEvent, JavaScriptValue eventData, IntPtr callbackState)
        {
            if (IsDebugging)
            {
                WithInternalContext(() =>
                {
                    string data=null;
                    if (eventData.IsValid)
                    {
                        data = eventData.ToJsonString();
                    }
                    var stepType=currentAdapter.OnDebugEvent(debugEvent, data,engine);
                    switch (debugEvent)
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
                            break;
                        case JavaScriptDiagDebugEvent.JsDiagDebugEventRuntimeException:
                            break;
                        default:
                            break;
                    }
                    engine.IsBreakpointMode = true;
                    
                });
                
            }
        }

        public void AddScriptSource(string name, string content)
        {
            currentAdapter?.AddScript(name, content);
        }

        public void AttachAdapter(IDebugAdapter adapter)
        {
            if (IsDebugging)
            {
                throw new InvalidOperationException("Debugging is already in progress, detach current adapter first");
            }
            WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagStartDebugging(runtime, onDebugEvent, IntPtr.Zero));
            });
            IsDebugging = true;
            currentAdapter = adapter;
        }

        public void DetachAdapter()
        {
            if (currentAdapter!=null)
            {
                WithInternalContext(() =>
                {
                    Native.ThrowIfError(Native.JsDiagStopDebugging(runtime, out IntPtr callbackState));
                });
            }
            IsDebugging = false;
            currentAdapter = null;
        }

        public JavaScriptValue Evaluate(string expression, uint stackFrameIndex,  bool forceSetValueProp)
        {
                JavaScriptValue exp = JavaScriptValue.FromString(expression);

                Native.ThrowIfError(Native.JsDiagEvaluate(exp, stackFrameIndex, JavaScriptParseScriptAttributes.JsParseScriptAttributeNone, forceSetValueProp, out JavaScriptValue result));
                return result;
            
        }

        public string GetBreakpoints()
        {
            return WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetBreakpoints(out JavaScriptValue result));
                return result.ToString();
            });
        }

        public JavaScriptValue GetObjectFromHandle(uint objectHandle)
        {
            return WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetObjectFromHandle(objectHandle, out JavaScriptValue result));
                return result;
            });
        }

        public string GetProperties(uint objectHandle, uint from, uint to)
        {
            return WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetProperties(objectHandle, from, to, out JavaScriptValue result));
                return result.ToJsonString();
            });
        }

        public string GetScriptSource(uint scriptId)
        {
            return WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetSource(scriptId, out JavaScriptValue source));
                return source.ToString();
            });
        }

        public string GetScripts()
        {
            IJSValueConverterService converter = CurrentNode.GetService<IJSValueConverterService>();
            return WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetScripts(out JavaScriptValue result));
            return result.ToJsonString();
            });

        }

        public string GetStackProperties(uint stackFrameIndex)
        {
            return WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetStackProperties(stackFrameIndex, out JavaScriptValue result));
                return result.ToJsonString();
            });
        }

        public string GetStackTrace()
        {
            return WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetStackTrace(out JavaScriptValue result));
                return result.ToJsonString();
            });
        }

        public void RemoveBreakpoint(uint breakpointId)
        {
            Native.ThrowIfError(Native.JsDiagRemoveBreakpoint(breakpointId));
        }

        public void ScriptReady()
        {
            engine.IsBreakpointMode = false;
            currentAdapter?.ScriptReady(engine);
        }

        public string SetBreakpoint(uint scriptId, uint line, uint column)
        {
            return WithInternalContext(() =>
            {
                Native.ThrowIfError(Native.JsDiagSetBreakpoint(scriptId, line, column, out JavaScriptValue breakpoint));
                return breakpoint.ToJsonString();
            });
        }

        public void SetBreakpointOnException(JavaScriptDiagBreakOnExceptionAttributes attributes)
        {
            Native.ThrowIfError(Native.JsDiagSetBreakOnException(runtime, attributes));
        }

        public void SetStepType(JavaScriptDiagStepType stepType)
        {
            Native.ThrowIfError(Native.JsDiagSetStepType(stepType));
        }

        private void WithInternalContext(Action a)
        {
            CurrentNode.GetService<IRuntimeService>().InternalContextSwitchService.With(a);
        }

        private T WithInternalContext<T>(Func<T> func)
        {
            return CurrentNode.GetService<IRuntimeService>().InternalContextSwitchService.With(func);
        }

        public void RequestAsyncBreak()
        {
            Native.ThrowIfError(Native.JsDiagRequestAsyncBreak(runtime));
        }
        //private string getJsonString(JavaScriptValue value)
        //{
        //    var valueService=CurrentNode.GetService<IJSValueService>();
        //    var valueConverter = CurrentNode.GetService<IJSValueConverterService>();
        //    JavaScriptPropertyId jsonId = JavaScriptPropertyId.FromString("JSON");
        //    JavaScriptPropertyId stringifyId = JavaScriptPropertyId.FromString("stringify");
        //    var json = JavaScriptValue.GlobalObject.GetProperty(jsonId);
        //    var stringify = json.GetProperty(stringifyId);
        //    var result=stringify.CallFunction(json, value);
        //    return result.ToString();
        //}

        public JavaScriptSourceContext GetScriptContext(string name, string script)
        {
            sourceContext++;
            return sourceContext;
        }
    }
}
