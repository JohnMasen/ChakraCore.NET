using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public interface IJSValueService:IService
    {
        T ReadProperty<T>(JavaScriptValue target, JavaScriptPropertyId id);
        void WriteProperty<T>(JavaScriptValue target,JavaScriptPropertyId id,T value);

        T ReadProperty<T>(JavaScriptValue target, string id);
        void WriteProperty<T>(JavaScriptValue target, string id, T value);
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
