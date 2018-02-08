using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.API
{
    public enum JavaScriptDiagBreakOnExceptionAttributes
    {
        /// <summary>

        ///     Don't break on any exception.

        /// </summary>

        JsDiagBreakOnExceptionAttributeNone = 0x0,

        /// <summary>

        ///     Break on uncaught exception.

        /// </summary>

        JsDiagBreakOnExceptionAttributeUncaught = 0x1,

        /// <summary>

        ///     Break on first chance exception.

        /// </summary>

        JsDiagBreakOnExceptionAttributeFirstChance = 0x2
    }
}
