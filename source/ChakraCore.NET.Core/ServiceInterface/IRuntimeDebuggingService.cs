using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Debug;
using ChakraCore.NET.API;
using System.Threading.Tasks;

namespace ChakraCore.NET
{
    public struct DebugEventArguments
    {
        public JavaScriptDiagDebugEvent EventType;
        public string EventData;
    }

    public interface IRuntimeDebuggingService:IService
    {
        event EventHandler<BreakPoint> OnBreakPoint;
        event EventHandler<BreakPoint> OnStepComplete;
        event EventHandler<RuntimeException> OnException;
        event EventHandler<BreakPoint> OnAsyncBreak;
        event EventHandler<DebugEventArguments> OnDebugEvent;
        event EventHandler OnEngineReady;
        event EventHandler<SourceCode> OnScriptLoad;

        void Step(JavaScriptDiagStepType stepType);
        void StartDebug();
        void StopDebug();
        void NotifyScriptReady();
        JavaScriptSourceContext GetScriptContext(string name, string script);
        #region Chakracore Debug Features
        BreakPoint SetBreakpoint(uint scriptId, uint line, uint column);
        BreakPoint[] GetBreakpoints();
        void RemoveBreakpoint(uint breakpointId);
        void SetBreakpointOnException(JavaScriptDiagBreakOnExceptionAttributes attributes);
        SourceCode[] GetScripts();
        SourceCode GetScriptSource(uint scriptId);
        StackTrace[] GetStackTrace();
        StackProperties GetStackProperties(uint stackFrameIndex);
        VariableProperties GetProperties(uint objectHandle, uint from, uint to);
        void RequestAsyncBreak();
        Variable GetObjectFromHandle(uint objectHandle);
        Variable Evaluate(string expression, uint stackFrameIndex,  bool forceSetValueProp);
        #endregion

    }
}
