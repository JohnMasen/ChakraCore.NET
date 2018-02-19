using ChakraCore.NET.API;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET.Debug
{
    public class DebugEngine
    {
        private IRuntimeDebuggingService service;
        private BlockingCollection<Action> commandQueue;
        public JavaScriptDiagStepType StepType { get; set; } = JavaScriptDiagStepType.JsDiagStepTypeContinue;
        public DebugEngine(IRuntimeDebuggingService debuggingService)
        {
            service = debuggingService;
            commandQueue = new BlockingCollection<Action>();
        }

        internal void StartProcessing()
        {
            foreach (var item in commandQueue.GetConsumingEnumerable())
            {
                item();
            }
        }

        internal void StopProcessing()
        {
            commandQueue.CompleteAdding();
        }

        public Task<BreakPoint> SetBreakpointAsync(uint scriptId, uint line, uint column)
        {
            return addCommand(() =>
            {
                return service.SetBreakpoint(scriptId, line, column);
            });
        }

        public void RemoveBreakpoint(uint breakpointId)
        {
            service.RemoveBreakpoint(breakpointId);
        }

        public void RequestAsyncBreak()
        {
            commandQueue.Add(service.RequestAsyncBreak);
        }

        public Task<String> GetScriptsAsync()
        {
            return addCommand(() =>
            {
                return service.GetScripts();
            });
        }

        public Task<BreakPoint[]> GetBreakPointsAsync()
        {
            return addCommand(() =>
            {
                return service.GetBreakpoints();
            });
        }

        public Task<string> GetStackTraceAsync()
        {
            return addCommand(() =>
            {
                return service.GetStackTrace();
            });
        }

        public Task<string> GetStackPropertiesAsync(uint stackFrameIndex)
        {
            return addCommand(() =>
            {
                return service.GetStackProperties(stackFrameIndex);
            });
        }

        public Task<string> GetObjectFromHandleAsync(uint objectHandle)
        {
            return addCommand(() =>
            {
                return service.GetObjectFromHandle(objectHandle).ToJsonString();
            });
        }

        public Task<string> GetObjectPropertiesAsync(uint objectHandle, uint from = 0, uint to = 99)
        {
            return addCommand(() =>
            {
                return service.GetProperties(objectHandle, from, to);
            });
        }

        public Task<string> EvaluateAsync(string expression, uint stackFrameIndex, bool forceSetValueProp)
        {
            return addCommand(() =>
            {
                return service.Evaluate(expression, stackFrameIndex, forceSetValueProp).ToJsonString();
            });
        }

        private Task<T> addCommand<T>(Func<T> func)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            commandQueue.Add(() =>
            {
                tcs.SetResult(func());
            });
            return tcs.Task;
        }

    }
}
