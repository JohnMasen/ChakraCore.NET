using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.API
{
    public enum JavaScriptDiagDebugEvent
    {
        /// <summary>

        ///     Indicates a new script being compiled, this includes script, eval, new function.

        /// </summary>

        JsDiagDebugEventSourceCompile = 0,

        /// <summary>

        ///     Indicates compile error for a script.

        /// </summary>

        JsDiagDebugEventCompileError = 1,

        /// <summary>

        ///     Indicates a break due to a breakpoint.

        /// </summary>

        JsDiagDebugEventBreakpoint = 2,

        /// <summary>

        ///     Indicates a break after completion of step action.

        /// </summary>

        JsDiagDebugEventStepComplete = 3,

        /// <summary>

        ///     Indicates a break due to debugger statement.

        /// </summary>

        JsDiagDebugEventDebuggerStatement = 4,

        /// <summary>

        ///     Indicates a break due to async break.

        /// </summary>

        JsDiagDebugEventAsyncBreak = 5,

        /// <summary>

        ///     Indicates a break due to a runtime script exception.

        /// </summary>

        JsDiagDebugEventRuntimeException = 6
    }
}
