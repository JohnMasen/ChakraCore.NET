using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSModuleTest.API
{
    public delegate JavaScriptErrorCode NotifyModuleReadyCallbackDelegate(JavaScriptModuleRecord module,JavaScriptValue value);
    public delegate JavaScriptErrorCode FetchImportedModuleDelegate(JavaScriptModuleRecord reference, JavaScriptValue name, out JavaScriptModuleRecord result);
    public delegate JavaScriptErrorCode FetchImportedModuleFromScriptDelegate(JavaScriptSourceContext sourceContext, JavaScriptValue source, out JavaScriptModuleRecord result);
    public struct JavaScriptModuleRecord
    {
        //public static JavaScriptModuleRecord NULLRecord=new JavaScriptModuleRecord(IntPtr.Zero);
        public readonly IntPtr reference;
        public JavaScriptModuleRecord(IntPtr reference)
        {
            this.reference = reference;
        }

        public static JavaScriptModuleRecord NULLModuleRecord = new JavaScriptModuleRecord(IntPtr.Zero);

        public static JavaScriptModuleRecord Create(JavaScriptModuleRecord? parent, string name)
        {
            JavaScriptValue moduleName;
            if (string.IsNullOrEmpty(name))
            {
                moduleName = JavaScriptValue.Invalid;
            }
            else
            {
                moduleName = JavaScriptValue.FromString(name);
            }
            JavaScriptModuleRecord result;
            if (parent.HasValue)
            {
                Native.ThrowIfError(Native.JsInitializeModuleRecord(parent.Value, moduleName, out result));
            }
            else
            {
                Native.ThrowIfError(Native.JsInitializeModuleRecord(NULLModuleRecord, moduleName, out result));
            }
            
            return result;
        }
        public static void ParseScript(JavaScriptModuleRecord module,string script)
        {
            var buffer=Encoding.UTF8.GetBytes(script);
            uint length = (uint)buffer.Length;
            Native.ThrowIfError( Native.JsParseModuleSource(module, JavaScriptSourceContext.None, buffer, length, JavaScriptParseModuleSourceFlags.JsParseModuleSourceFlags_DataIsUTF8, out JavaScriptValue parseException));
            if (parseException.IsValid)
            {
                string ex = parseException.ToString();
                throw new InvalidOperationException($"Parse script failed with error={ex}");
            }
        }

        public static JavaScriptValue RunModule(JavaScriptModuleRecord module)
        {
            Native.ThrowIfError( Native.JsModuleEvaluation(module, out JavaScriptValue result));
            return result;
        }

        public static void SetHostInfo(JavaScriptModuleRecord module,JavascriptModuleHostInfoKind kind,object value)
        {
            Native.ThrowIfError(Native.JsSetModuleHostInfo(module, kind, value));
        }

        public static void SetNotifyReady(JavaScriptModuleRecord module,NotifyModuleReadyCallbackDelegate callback)
        {
            Native.ThrowIfError(Native.JsSetModuleNotifyModuleReadyCallback(module, JavascriptModuleHostInfoKind.JsModuleHostInfo_NotifyModuleReadyCallback, callback));
        }

        public static void SetFetchModuleCallback(JavaScriptModuleRecord module,FetchImportedModuleDelegate callback)
        {
            Native.ThrowIfError(Native.JsSetFetchImportedModuleCallback(module, JavascriptModuleHostInfoKind.JsModuleHostInfo_FetchImportedModuleCallback, callback));
        }

        public static void SetFetchModuleScriptCallback(JavaScriptModuleRecord module,FetchImportedModuleFromScriptDelegate callback)
        {
            Native.ThrowIfError(Native.JsSetFetchImportedModuleFromScriptyCallback(module, JavascriptModuleHostInfoKind.JsModuleHostInfo_FetchImportedModuleFromScriptCallback, callback));
        }

        public static void SetModuleName(JavaScriptModuleRecord module,string name)
        {
            JavaScriptValue value = JavaScriptValue.FromString(name);
            Native.ThrowIfError(Native.JsSetModuleHostName(module,JavascriptModuleHostInfoKind.JsModuleHostInfo_HostDefined, value));
        }
    }
}
