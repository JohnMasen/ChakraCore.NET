using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Debug;
using ChakraCore.NET.API;
namespace ChakraCore.NET
{
    public interface IRuntimeDebuggingService:IService
    {
        void AttachAdapter(IDebugAdapter adapter);
        void DetachAdapter();
        //void AddScriptSource(string name, string content);
        void ScriptReady();
        JavaScriptSourceContext GetScriptContext(string name, string script);
        #region Chakracore Debug Features
        BreakPoint SetBreakpoint(uint scriptId, uint line, uint column);
        BreakPoint[] GetBreakpoints();
        void RemoveBreakpoint(uint breakpointId);
        void SetBreakpointOnException(JavaScriptDiagBreakOnExceptionAttributes attributes);
        void SetStepType(JavaScriptDiagStepType stepType);
        string GetScripts();
        string GetScriptSource(uint scriptId);
        StackTrace[] GetStackTrace();
        StackProperties GetStackProperties(uint stackFrameIndex);
        VariableProperties GetProperties(uint objectHandle, uint from, uint to);
        void RequestAsyncBreak();
        Variable GetObjectFromHandle(uint objectHandle);
        JavaScriptValue Evaluate(string expression, uint stackFrameIndex,  bool forceSetValueProp);
        #endregion

    }
}
