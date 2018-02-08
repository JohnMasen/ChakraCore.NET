using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.API
{
    public delegate void JsDiagDebugEventCallback(JavaScriptDiagDebugEvent debugEvent, JavaScriptValue eventData, IntPtr callbackState);
}
