using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.API
{
    public enum JavaScriptDiagStepType
    {
        /// <summary>

        ///     Perform a step operation to next statement.

        /// </summary>

        JsDiagStepTypeStepIn = 0,

        /// <summary>

        ///     Perform a step out from the current function.

        /// </summary>

        JsDiagStepTypeStepOut = 1,

        /// <summary>

        ///     Perform a single step over after a debug break if the next statement is a function call, else behaves as a stepin.

        /// </summary>

        JsDiagStepTypeStepOver = 2,

        /// <summary>

        ///     Perform a single step back to the previous statement (only applicable in TTD mode).

        /// </summary>

        JsDiagStepTypeStepBack = 3,

        /// <summary>

        ///     Perform a reverse continue operation (only applicable in TTD mode).

        /// </summary>

        JsDiagStepTypeReverseContinue = 4,

        /// <summary>

        ///     Perform a forward continue operation. Clears any existing step value.

        /// </summary>

        JsDiagStepTypeContinue = 5
    }
}
