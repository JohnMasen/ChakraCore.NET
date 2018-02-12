using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.API
{
    /// <summary>
    ///     User implemented callback to get notification when the module is ready.
    /// </summary>
    /// <remarks>
    /// Notify the host after ModuleDeclarationInstantiation step (15.2.1.1.6.4) is finished. If there was error in the process, exceptionVar
    /// holds the exception. Otherwise the referencingModule is ready and the host should schedule execution afterwards.
    /// </remarks>
    /// <param name="referencingModule">The referencing module that have finished running ModuleDeclarationInstantiation step.</param>
    /// <param name="exceptionVar">If nullptr, the module is successfully initialized and host should queue the execution job
    ///                           otherwise it's the exception object.</param>
    /// <returns>
    ///     JavaScriptErrorCode.NoError if the operation succeeded, throw exception if failed.
    /// </returns>
    public delegate JavaScriptErrorCode NotifyModuleReadyCallbackDelegate(JavaScriptModuleRecord module,JavaScriptValue value);

    /// <summary>
    ///     User implemented callback to fetch additional imported modules.
    /// </summary>
    /// <remarks>
    /// Notify the host to fetch the dependent module. This is the "import" part before HostResolveImportedModule in ES6 spec.
    /// This notifies the host that the referencing module has the specified module dependency, and the host need to retrieve the module back.
    /// </remarks>
    /// <param name="reference">The referencing module that is requesting the dependency modules.</param>
    /// <param name="name">The specifier coming from the module source code.</param>
    /// <param name="result">The ModuleRecord of the dependent module. If the module was requested before from other source, return the
    ///                           existing ModuleRecord, otherwise return a newly created ModuleRecord.</param>
    /// <returns>
    ///     JavaScriptErrorCode.NoError if the operation succeeded, throw exception if failed.
    /// </returns>
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
            if (string.IsNullOrEmpty(name))
            {
                name = Guid.NewGuid().ToString();//root module has no name, give it a unique name
            }
            JavaScriptValue moduleName = JavaScriptValue.FromString(name);
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
        public static void ParseScript(JavaScriptModuleRecord module,string script, JavaScriptSourceContext sourceContext)
        {
            var buffer=Encoding.UTF8.GetBytes(script);
            uint length = (uint)buffer.Length;
            Native.ThrowIfError( Native.JsParseModuleSource(module, sourceContext, buffer, length, JavaScriptParseModuleSourceFlags.JsParseModuleSourceFlags_DataIsUTF8, out JavaScriptValue parseException));
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

        public static void SetHostUrl(JavaScriptModuleRecord module,string url)
        {
            var value= JavaScriptValue.FromString(url);
            Native.ThrowIfError(Native.JsSetModuleHostInfo(module, JavascriptModuleHostInfoKind.JsModuleHostInfo_Url, value));
        }

        /// <summary>
        /// Set callback from chakraCore when the module resolution is finished, either successfuly or unsuccessfully.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="callback"></param>
        public static void SetNotifyReady(JavaScriptModuleRecord module,NotifyModuleReadyCallbackDelegate callback)
        {
            Native.ThrowIfError(Native.JsSetModuleNotifyModuleReadyCallback(module, JavascriptModuleHostInfoKind.JsModuleHostInfo_NotifyModuleReadyCallback, callback));
        }


        /// <summary>
        /// Set callback from chakracore to fetch dependent module. 
        /// While this call will come back directly from ParseModuleSource, the additional
        /// task are treated as Promise that will be executed later.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="callback"></param>
        public static void SetFetchModuleCallback(JavaScriptModuleRecord module,FetchImportedModuleDelegate callback)
        {
            Native.ThrowIfError(Native.JsFetchImportedModuleCallback(module, JavascriptModuleHostInfoKind.JsModuleHostInfo_FetchImportedModuleCallback, callback));
        }

        /// <summary>
        /// Set callback from chakracore to fetch module dynamically during runtime. 
        /// While this call will come back directly from runtime script or module code, the additional
        /// task can be scheduled asynchronously that executed later.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="callback"></param>
        public static void SetFetchModuleScriptCallback(JavaScriptModuleRecord module,FetchImportedModuleFromScriptDelegate callback)
        {
            Native.ThrowIfError(Native.JsFetchImportedModuleFromScriptyCallback(module, JavascriptModuleHostInfoKind.JsModuleHostInfo_FetchImportedModuleFromScriptCallback, callback));
        }
    }
}
