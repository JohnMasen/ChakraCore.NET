using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChakraCore.NET.API;
using ChakraCore.NET.Debug;
using Newtonsoft.Json;

namespace ChakraCore.NET
{
    public class RuntimeDebuggingService : ServiceBase, IRuntimeDebuggingService
    {
        JavaScriptRuntime runtime;
        //IDebugAdapter currentAdapter;
        public bool IsDebugging { get; private set; }
        JavaScriptSourceContext sourceContext = new JavaScriptSourceContext();
        private JsDiagDebugEventCallback debugCallback;

        public event EventHandler<BreakPoint> OnBreakPoint;
        public event EventHandler<BreakPoint> OnStepComplete;
        public event EventHandler<RuntimeException> OnException;
        public event EventHandler<BreakPoint> OnAsyncBreak;
        public event EventHandler<DebugEventArguments> OnDebugEvent;
        public event EventHandler OnEngineReady;
        public event EventHandler<SourceCode> OnScriptLoad;
        //private JavaScriptDiagStepType StepType = JavaScriptDiagStepType.JsDiagStepTypeContinue;
        TaskQueueRunner runner = new TaskQueueRunner();
        private AutoResetEvent stepASE = new AutoResetEvent(false);
        

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
                //DebugEngine engine;
                //runner.With(() =>
                //{
                    OnDebugEvent?.Invoke(this, new DebugEventArguments() { EventType = debugEvent, EventData = data });
                //});
                switch (debugEvent)
                {
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventSourceCompile:

                        OnScriptLoad?.Invoke(this,JsonConvert.DeserializeObject<SourceCode>(data));
                        break;
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventRuntimeException:
                        runner.With(() =>
                        {
                            OnException?.Invoke(this,JsonConvert.DeserializeObject<RuntimeException>(data));
                            stepASE.WaitOne();
                        });
                        
                        break;
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventStepComplete:
                        runner.With(() =>
                        {
                            OnStepComplete?.Invoke(this,JsonConvert.DeserializeObject<BreakPoint>(data));
                            stepASE.WaitOne();
                        });
                        
                        break;
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventBreakpoint:
                        runner.With(() =>
                        {
                            OnBreakPoint?.Invoke(this,JsonConvert.DeserializeObject<BreakPoint>(data));
                            stepASE.WaitOne();
                        });
                        
                        break;
                    case JavaScriptDiagDebugEvent.JsDiagDebugEventAsyncBreak:
                        runner.With(() =>
                        {
                            OnAsyncBreak?.Invoke(this,JsonConvert.DeserializeObject<BreakPoint>(data));
                            stepASE.WaitOne();
                        });
                        
                        break;
                    default:
                        break;
                }

            }
        }

        public void StartDebug()
        {
            if (IsDebugging)
            {
                throw new InvalidOperationException("Debugging is already in progress, detach current adapter first");
            }
            Native.ThrowIfError(Native.JsDiagStartDebugging(runtime, debugCallback, IntPtr.Zero));
            IsDebugging = true;
        }

        public void StopDebug()
        {
            if (IsDebugging)
            {
                Native.ThrowIfError(Native.JsDiagStopDebugging(runtime, out IntPtr callbackState));
            }
            IsDebugging = false;
        }

        public void NotifyScriptReady()
        {
            runner.With(() => { OnEngineReady?.Invoke(this, null); });
            
        }


        public Variable Evaluate(string expression, uint stackFrameIndex, bool forceSetValueProp)
        {
            return runner.RunTask(() =>
            {
                JavaScriptValue exp = JavaScriptValue.FromString(expression);
                Native.ThrowIfError(Native.JsDiagEvaluate(exp, stackFrameIndex, JavaScriptParseScriptAttributes.JsParseScriptAttributeNone, forceSetValueProp, out JavaScriptValue result), true);
                return JsonConvert.DeserializeObject<Variable>(result.ToJsonString());
            });
            

        }

        public BreakPoint[] GetBreakpoints()
        {
            return runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetBreakpoints(out JavaScriptValue result));
                var json = result.ToJsonString();
                return JsonConvert.DeserializeObject<BreakPoint[]>(json);
            });
            
        }

        public Variable GetObjectFromHandle(uint objectHandle)
        {
            return runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetObjectFromHandle(objectHandle, out JavaScriptValue result));
                return JsonConvert.DeserializeObject<Variable>(result.ToJsonString());
            });
        }

        public VariableProperties GetProperties(uint objectHandle, uint from, uint to)
        {
            return runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetProperties(objectHandle, from, to, out JavaScriptValue result));
                return JsonConvert.DeserializeObject<VariableProperties>(result.ToJsonString());
            });
        }

        public SourceCode GetScriptSource(uint scriptId)
        {
            return runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetSource(scriptId, out JavaScriptValue source));
                return JsonConvert.DeserializeObject<SourceCode>(source.ToJsonString());
            });
            
        }

        public SourceCode[] GetScripts()
        {
            return runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetScripts(out JavaScriptValue result));
                return JsonConvert.DeserializeObject<SourceCode[]>(result.ToJsonString());
            });
            


        }

        public StackProperties GetStackProperties(uint stackFrameIndex)
        {
            return runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetStackProperties(stackFrameIndex, out JavaScriptValue result));
                return JsonConvert.DeserializeObject<StackProperties>(result.ToJsonString());
            });
            
        }

        public StackTrace[] GetStackTrace()
        {
            return runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagGetStackTrace(out JavaScriptValue result));
                return JsonConvert.DeserializeObject<StackTrace[]>(result.ToJsonString());
            });
            
        }

        public void RemoveBreakpoint(uint breakpointId)
        {
            runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagRemoveBreakpoint(breakpointId));
            });
            
        }


        public BreakPoint SetBreakpoint(uint scriptId, uint line, uint column)
        {
            return runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagSetBreakpoint(scriptId, line, column, out JavaScriptValue breakpoint));
                string json = breakpoint.ToJsonString();
                return JsonConvert.DeserializeObject<BreakPoint>(json);
            });
            
        }

        public void SetBreakpointOnException(JavaScriptDiagBreakOnExceptionAttributes attributes)
        {
            runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagSetBreakOnException(runtime, attributes));
            });
            stepASE.Set();
        }

        


        public void RequestAsyncBreak()
        {
            Native.ThrowIfError(Native.JsDiagRequestAsyncBreak(runtime));
        }


        public JavaScriptSourceContext GetScriptContext(string name, string script)
        {
            sourceContext++;
            return sourceContext;
        }

        public void Step(JavaScriptDiagStepType stepType)
        {
            runner.RunTask(() =>
            {
                Native.ThrowIfError(Native.JsDiagSetStepType(stepType));
            });
            stepASE.Set();
        }

        private class TaskQueueRunner
        {
            private BlockingCollection<Action> taskQueue = null;
            private AutoResetEvent processASE = new AutoResetEvent(false);
            public void StartProcess(BlockingCollection<Action> queue)
            {
                processASE.Set();
                foreach (var item in queue.GetConsumingEnumerable())
                {
                    item();
                }
            }

            public void RunTask(Action action)
            {
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                taskQueue.Add(()=>
                {
                    action();
                    tcs.SetResult(null);
                });
                tcs.Task.Wait();
            }

            public T RunTask<T>(Func<T> func)
            {
                TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
                taskQueue.Add(()=>
                {
                    tcs.SetResult(func());
                });
                tcs.Task.Wait();
                return tcs.Task.Result;
            }

            public void StopProcess()
            {
                processASE.WaitOne();
                taskQueue.CompleteAdding();
                taskQueue = null;
            }

            public void With(Action action)
            {
                taskQueue = new BlockingCollection<Action>();
                Task.Factory.StartNew(() =>
                {
                    action();//run the action on new thread
                    
                    StopProcess(); 
                });
                StartProcess(taskQueue);//start process at current thread
            }

        }


    }
}
