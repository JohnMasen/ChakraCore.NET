using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public interface IContextService:IService
    {
        string RunScript(string script);
        JavaScriptValue ParseScript(string script);
        JavaScriptValue JSValue_Undefined { get; }
        JavaScriptValue JSValue_Null { get; }
        JavaScriptValue JSValue_True { get; }
        JavaScriptValue JSValue_False { get; }
        JavaScriptValue GlobalObject { get; }

        JavaScriptValue CreateFunction(JavaScriptNativeFunction function, IntPtr callbackData);
        JavaScriptValue CreateObject();
        JavaScriptValue CreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback);

        JavaScriptValue CallFunction(JavaScriptValue target, params JavaScriptValue[] para);
        JavaScriptValue ConstructObject(JavaScriptValue target, params JavaScriptValue[] para);
        
    }
}
