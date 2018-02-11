namespace ChakraCore.NET.API
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    ///     Native interfaces.
    /// </summary>
    public static class Native

    {

        /// <summary>

        /// Throws if a native method returns an error code.

        /// </summary>

        /// <param name="error">The error.</param>

        public static void ThrowIfError(JavaScriptErrorCode error)

        {

            if (error != JavaScriptErrorCode.NoError)

            {

                switch (error)

                {

                    case JavaScriptErrorCode.InvalidArgument:

                        throw new JavaScriptUsageException(error, "Invalid argument.");



                    case JavaScriptErrorCode.NullArgument:

                        throw new JavaScriptUsageException(error, "Null argument.");



                    case JavaScriptErrorCode.NoCurrentContext:

                        throw new JavaScriptUsageException(error, "No current context.");



                    case JavaScriptErrorCode.InExceptionState:

                        throw new JavaScriptUsageException(error, "Runtime is in exception state.");



                    case JavaScriptErrorCode.NotImplemented:

                        throw new JavaScriptUsageException(error, "Method is not implemented.");



                    case JavaScriptErrorCode.WrongThread:

                        throw new JavaScriptUsageException(error, "Runtime is active on another thread.");



                    case JavaScriptErrorCode.RuntimeInUse:

                        throw new JavaScriptUsageException(error, "Runtime is in use.");



                    case JavaScriptErrorCode.BadSerializedScript:

                        throw new JavaScriptUsageException(error, "Bad serialized script.");



                    case JavaScriptErrorCode.InDisabledState:

                        throw new JavaScriptUsageException(error, "Runtime is disabled.");



                    case JavaScriptErrorCode.CannotDisableExecution:

                        throw new JavaScriptUsageException(error, "Cannot disable execution.");



                    case JavaScriptErrorCode.AlreadyDebuggingContext:

                        throw new JavaScriptUsageException(error, "Context is already in debug mode.");



                    case JavaScriptErrorCode.HeapEnumInProgress:

                        throw new JavaScriptUsageException(error, "Heap enumeration is in progress.");



                    case JavaScriptErrorCode.ArgumentNotObject:

                        throw new JavaScriptUsageException(error, "Argument is not an object.");



                    case JavaScriptErrorCode.InProfileCallback:

                        throw new JavaScriptUsageException(error, "In a profile callback.");



                    case JavaScriptErrorCode.InThreadServiceCallback:

                        throw new JavaScriptUsageException(error, "In a thread service callback.");



                    case JavaScriptErrorCode.CannotSerializeDebugScript:

                        throw new JavaScriptUsageException(error, "Cannot serialize a debug script.");



                    case JavaScriptErrorCode.AlreadyProfilingContext:

                        throw new JavaScriptUsageException(error, "Already profiling this context.");



                    case JavaScriptErrorCode.IdleNotEnabled:

                        throw new JavaScriptUsageException(error, "Idle is not enabled.");



                    case JavaScriptErrorCode.OutOfMemory:

                        throw new JavaScriptEngineException(error, "Out of memory.");



                    case JavaScriptErrorCode.ScriptException:

                        {
                            string msg = extractErrorObject(out var errorObject);
                            



                            throw new JavaScriptScriptException(error, errorObject, $"Script threw an exception. {msg}");

                        }



                    case JavaScriptErrorCode.ScriptCompile:

                        {

                            string msg = extractErrorObject(out var errorObject);

                            throw new JavaScriptScriptException(error, errorObject, $"Compile error. {msg}");

                        }



                    case JavaScriptErrorCode.ScriptTerminated:

                        throw new JavaScriptScriptException(error, JavaScriptValue.Invalid, "Script was terminated.");



                    case JavaScriptErrorCode.ScriptEvalDisabled:

                        throw new JavaScriptScriptException(error, JavaScriptValue.Invalid, "Eval of strings is disabled in this runtime.");



                    case JavaScriptErrorCode.Fatal:

                        throw new JavaScriptFatalException(error);



                    default:

                        throw new JavaScriptFatalException(error);

                }

            }

        }

        private static string extractErrorObject(out JavaScriptValue errorObject)
        {
            JavaScriptErrorCode result;
            result = JsGetAndClearException(out errorObject);
            if (result!=JavaScriptErrorCode.NoError)
            {
                throw new JavaScriptFatalException(result, "failed to get and clear exception");
            }

            
            JavaScriptPropertyId messageName;
            result = Native.JsGetPropertyIdFromName("message",
                out messageName);
            if ( result!= JavaScriptErrorCode.NoError)
            {
                throw new JavaScriptFatalException(result, "failed to get error message id");
            }

            JavaScriptValue messageValue;
            result = JsGetProperty(errorObject, messageName, out messageValue);
            if (result!=JavaScriptErrorCode.NoError)
            {
                throw new JavaScriptFatalException(result, "failed to get error message");
            }
            
            IntPtr message;
            UIntPtr length;
            result = JsStringToPointer(messageValue, out message, out length);
            if (result!=JavaScriptErrorCode.NoError)
            {
                throw new JavaScriptFatalException(result, "failed to convert error message");
            }
            return Marshal.PtrToStringUni(message);
        }

        const string DllName = "ChakraCore.dll";



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService, out JavaScriptRuntime runtime);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCollectGarbage(JavaScriptRuntime handle);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsDisposeRuntime(JavaScriptRuntime handle);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetRuntimeMemoryUsage(JavaScriptRuntime runtime, out UIntPtr memoryUsage);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetRuntimeMemoryLimit(JavaScriptRuntime runtime, out UIntPtr memoryLimit);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetRuntimeMemoryLimit(JavaScriptRuntime runtime, UIntPtr memoryLimit);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntime runtime, IntPtr callbackState, JavaScriptMemoryAllocationCallback allocationCallback);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetRuntimeBeforeCollectCallback(JavaScriptRuntime runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback);



        [DllImport(DllName, EntryPoint = "JsAddRef")]

        public static extern JavaScriptErrorCode JsContextAddRef(JavaScriptContext reference, out uint count);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsAddRef(JavaScriptValue reference, out uint count);



        [DllImport(DllName, EntryPoint = "JsRelease")]

        public static extern JavaScriptErrorCode JsContextRelease(JavaScriptContext reference, out uint count);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsRelease(JavaScriptValue reference, out uint count);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateContext(JavaScriptRuntime runtime, out JavaScriptContext newContext);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetCurrentContext(out JavaScriptContext currentContext);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetCurrentContext(JavaScriptContext context);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetRuntime(JavaScriptContext context, out JavaScriptRuntime runtime);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsIdle(out uint nextIdleTick);



        [DllImport(DllName, CharSet = CharSet.Unicode)]

        public static extern JavaScriptErrorCode JsParseScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValue result);



        [DllImport(DllName, CharSet = CharSet.Unicode)]

        public static extern JavaScriptErrorCode JsRunScript(string script, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValue result);



        [DllImport(DllName, CharSet = CharSet.Unicode)]

        public static extern JavaScriptErrorCode JsSerializeScript(string script, byte[] buffer, ref ulong bufferSize);



        [DllImport(DllName, CharSet = CharSet.Unicode)]

        public static extern JavaScriptErrorCode JsParseSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValue result);



        [DllImport(DllName, CharSet = CharSet.Unicode)]

        public static extern JavaScriptErrorCode JsRunSerializedScript(string script, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValue result);



        [DllImport(DllName, CharSet = CharSet.Unicode)]

        public static extern JavaScriptErrorCode JsGetPropertyIdFromName(string name, out JavaScriptPropertyId propertyId);



        [DllImport(DllName, CharSet = CharSet.Unicode)]

        public static extern JavaScriptErrorCode JsGetPropertyNameFromId(JavaScriptPropertyId propertyId, out string name);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetUndefinedValue(out JavaScriptValue undefinedValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetNullValue(out JavaScriptValue nullValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetTrueValue(out JavaScriptValue trueValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetFalseValue(out JavaScriptValue falseValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsBoolToBoolean(bool value, out JavaScriptValue booleanValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsBooleanToBool(JavaScriptValue booleanValue, out bool boolValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsConvertValueToBoolean(JavaScriptValue value, out JavaScriptValue booleanValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetValueType(JavaScriptValue value, out JavaScriptValueType type);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsDoubleToNumber(double doubleValue, out JavaScriptValue value);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsIntToNumber(int intValue, out JavaScriptValue value);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsNumberToDouble(JavaScriptValue value, out double doubleValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsConvertValueToNumber(JavaScriptValue value, out JavaScriptValue numberValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetStringLength(JavaScriptValue sringValue, out int length);



        [DllImport(DllName, CharSet = CharSet.Unicode)]

        public static extern JavaScriptErrorCode JsPointerToString(string value, UIntPtr stringLength, out JavaScriptValue stringValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsStringToPointer(JavaScriptValue value, out IntPtr stringValue, out UIntPtr stringLength);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsConvertValueToString(JavaScriptValue value, out JavaScriptValue stringValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetGlobalObject(out JavaScriptValue globalObject);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateObject(out JavaScriptValue obj);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback, out JavaScriptValue obj);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsConvertValueToObject(JavaScriptValue value, out JavaScriptValue obj);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetPrototype(JavaScriptValue obj, out JavaScriptValue prototypeObject);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetPrototype(JavaScriptValue obj, JavaScriptValue prototypeObject);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetExtensionAllowed(JavaScriptValue obj, out bool value);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsPreventExtension(JavaScriptValue obj);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId, out JavaScriptValue value);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetOwnPropertyDescriptor(JavaScriptValue obj, JavaScriptPropertyId propertyId, out JavaScriptValue propertyDescriptor);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetOwnPropertyNames(JavaScriptValue obj, out JavaScriptValue propertyNames);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId, JavaScriptValue value, bool useStrictRules);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsHasProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId, out bool hasProperty);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsDeleteProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId, bool useStrictRules, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsDefineProperty(JavaScriptValue obj, JavaScriptPropertyId propertyId, JavaScriptValue propertyDescriptor, out bool result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsHasIndexedProperty(JavaScriptValue obj, JavaScriptValue index, out bool result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetIndexedProperty(JavaScriptValue obj, JavaScriptValue index, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetIndexedProperty(JavaScriptValue obj, JavaScriptValue index, JavaScriptValue value);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsDeleteIndexedProperty(JavaScriptValue obj, JavaScriptValue index);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsEquals(JavaScriptValue obj1, JavaScriptValue obj2, out bool result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsStrictEquals(JavaScriptValue obj1, JavaScriptValue obj2, out bool result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsHasExternalData(JavaScriptValue obj, out bool value);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetExternalData(JavaScriptValue obj, out IntPtr externalData);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetExternalData(JavaScriptValue obj, IntPtr externalData);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateArray(uint length, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCallFunction(JavaScriptValue function, JavaScriptValue[] arguments, ushort argumentCount, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsConstructObject(JavaScriptValue function, JavaScriptValue[] arguments, ushort argumentCount, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr externalData, out JavaScriptValue function);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateError(JavaScriptValue message, out JavaScriptValue error);


        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateRangeError(JavaScriptValue message, out JavaScriptValue error);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateReferenceError(JavaScriptValue message, out JavaScriptValue error);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateSyntaxError(JavaScriptValue message, out JavaScriptValue error);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateTypeError(JavaScriptValue message, out JavaScriptValue error);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateURIError(JavaScriptValue message, out JavaScriptValue error);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsHasException(out bool hasException);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetAndClearException(out JavaScriptValue exception);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetException(JavaScriptValue exception);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsDisableRuntimeExecution(JavaScriptRuntime runtime);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsEnableRuntimeExecution(JavaScriptRuntime runtime);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsIsRuntimeExecutionDisabled(JavaScriptRuntime runtime, out bool isDisabled);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetObjectBeforeCollectCallback(JavaScriptValue reference, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback beforeCollectCallback);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateNamedFunction(JavaScriptValue name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState, out JavaScriptValue function);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateArrayBuffer(uint byteLength, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValue arrayBuffer, uint byteOffset,

            uint elementLength, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateDataView(JavaScriptValue arrayBuffer, uint byteOffset, uint byteOffsetLength, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetArrayBufferStorage(JavaScriptValue arrayBuffer, out IntPtr data, out uint bufferLength);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetTypedArrayStorage(JavaScriptValue typedArray, out IntPtr data, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetDataViewStorage(JavaScriptValue dataView, out IntPtr data, out uint bufferLength);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetPropertyIdType(JavaScriptPropertyId propertyId, out JavaScriptPropertyIdType propertyIdType);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateSymbol(JavaScriptValue description, out JavaScriptValue symbol);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetSymbolFromPropertyId(JavaScriptPropertyId propertyId, out JavaScriptValue symbol);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetPropertyIdFromSymbol(JavaScriptValue symbol, out JavaScriptPropertyId propertyId);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetOwnPropertySymbols(JavaScriptValue obj, out JavaScriptValue propertySymbols);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsNumberToInt(JavaScriptValue value, out int intValue);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetIndexedPropertiesToExternalData(JavaScriptValue obj, IntPtr data, JavaScriptTypedArrayType arrayType, uint elementLength);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetIndexedPropertiesExternalData(JavaScriptValue obj, IntPtr data, out JavaScriptTypedArrayType arrayType, out uint elementLength);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsHasIndexedPropertiesExternalData(JavaScriptValue obj, out bool value);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsInstanceOf(JavaScriptValue obj, JavaScriptValue constructor, out bool result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState, out JavaScriptValue obj);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetTypedArrayInfo(JavaScriptValue typedArray, out JavaScriptTypedArrayType arrayType, out JavaScriptValue arrayBuffer, out uint byteOffset, out uint byteLength);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetContextOfObject(JavaScriptValue obj, out JavaScriptContext context);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsGetContextData(JavaScriptContext context, out IntPtr data);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsSetContextData(JavaScriptContext context, IntPtr data);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsParseSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback,

            JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValue result);



        [DllImport(DllName)]

        public static extern JavaScriptErrorCode JsRunSerializedScriptWithCallback(JavaScriptSerializedScriptLoadSourceCallback scriptLoadCallback,

            JavaScriptSerializedScriptUnloadCallback scriptUnloadCallback, byte[] buffer, JavaScriptSourceContext sourceContext, string sourceUrl, out JavaScriptValue result);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsInitializeModuleRecord(JavaScriptModuleRecord parent, JavaScriptValue name, out JavaScriptModuleRecord result);


        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsParseModuleSource(JavaScriptModuleRecord moduel, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags flags, out JavaScriptValue parseException);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsModuleEvaluation(JavaScriptModuleRecord moduel, out JavaScriptValue result);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsSetModuleHostInfo(JavaScriptModuleRecord module, JavascriptModuleHostInfoKind kind, JavaScriptValue value);

        [DllImport(DllName, EntryPoint = "JsSetModuleHostInfo")]
        public static extern JavaScriptErrorCode JsSetModuleNotifyModuleReadyCallback(JavaScriptModuleRecord module, JavascriptModuleHostInfoKind kind, NotifyModuleReadyCallbackDelegate value);

        [DllImport(DllName, EntryPoint = "JsSetModuleHostInfo")]
        public static extern JavaScriptErrorCode JsFetchImportedModuleCallback(JavaScriptModuleRecord module, JavascriptModuleHostInfoKind kind, FetchImportedModuleDelegate value);

        [DllImport(DllName, EntryPoint = "JsSetModuleHostInfo")]
        public static extern JavaScriptErrorCode JsFetchImportedModuleFromScriptyCallback(JavaScriptModuleRecord module, JavascriptModuleHostInfoKind kind, FetchImportedModuleFromScriptDelegate value);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsRun(JavaScriptValue script, JavaScriptSourceContext sourceContext, JavaScriptValue sourceUrl, JavaScriptParseScriptAttributes parseAttributes, out JavaScriptValue result);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsCreatePromise(out JavaScriptValue promise, out JavaScriptValue resolve, out JavaScriptValue reject);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagStartDebugging(JavaScriptRuntime runtimeHandle, JsDiagDebugEventCallback debugEventCallback, IntPtr callbackState);
        /// <summary>
        ///     Stops debugging in the given runtime.
        /// </summary>
        /// <param name="runtimeHandle">Runtime to stop debugging.</param>
        /// <param name="callbackState">User provided state that was passed in JsDiagStartDebugging.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The runtime should be active on the current thread and in debug state.
        /// </remarks>
        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagStopDebugging(JavaScriptRuntime runtimeHandle,out IntPtr callbackState);

        /// <summary>
        ///     Request the runtime to break on next JavaScript statement.
        /// </summary>
        /// <param name="runtimeHandle">Runtime to request break.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The runtime should be in debug state. This API can be called from another runtime.
        /// </remarks>
        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagRequestAsyncBreak(JavaScriptRuntime runtimeHandle);

        /// <summary>
        ///     List all breakpoints in the current runtime.
        /// </summary>
        /// <param name="breakpoints">Array of breakpoints.</param>
        /// <remarks>
        ///     <para>
        ///     [{
        ///         "breakpointId" : 1,
        ///         "scriptId" : 1,
        ///         "line" : 0,
        ///         "column" : 62
        ///     }]
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetBreakpoints(out JavaScriptValue breakpoints);


        /// <summary>
        ///     Sets breakpoint in the specified script at give location.
        /// </summary>
        /// <param name="scriptId">Id of script from JsDiagGetScripts or JsDiagGetSource to put breakpoint.</param>
        /// <param name="lineNumber">0 based line number to put breakpoint.</param>
        /// <param name="columnNumber">0 based column number to put breakpoint.</param>
        /// <param name="breakpoint">Breakpoint object with id, line and column if success.</param>
        /// <remarks>
        ///     <para>
        ///     {
        ///         "breakpointId" : 1,
        ///         "line" : 2,
        ///         "column" : 4
        ///     }
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagSetBreakpoint(uint scriptId, uint lineNumber, uint columnNumber, out JavaScriptValue breakpoint);


        /// <summary>
        ///     Remove a breakpoint.
        /// </summary>
        /// <param name="breakpointId">Breakpoint id returned from JsDiagSetBreakpoint.</param>
        /// <returns>
        ///     The code <c>JsNoError</c> if the operation succeeded, a failure code otherwise.
        /// </returns>
        /// <remarks>
        ///     The current runtime should be in debug state. This API can be called when runtime is at a break or running.
        /// </remarks>
        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagRemoveBreakpoint(uint breakpointId);


        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagSetBreakOnException(JavaScriptRuntime runtimeHandle, JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetBreakOnException(JavaScriptRuntime runtimeHandle, out JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagSetStepType(JavaScriptDiagStepType stepType);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetScripts(out JavaScriptValue scriptsArray);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetSource(uint scriptId, out JavaScriptValue source);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetFunctionPosition(JavaScriptValue function, out JavaScriptValue functionPosition);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetStackTrace(out JavaScriptValue stackTrace);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetStackProperties(uint stackFrameIndex, out JavaScriptValue properties);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetProperties(uint objectHandle, uint fromCount, uint totalCount, out JavaScriptValue propertiesObject);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagGetObjectFromHandle(uint objectHandle, out JavaScriptValue handleObject);

        [DllImport(DllName)]
        public static extern JavaScriptErrorCode JsDiagEvaluate(JavaScriptValue expression, uint stackFrameIndex, JavaScriptParseScriptAttributes parseAttributes, bool forceSetValueProp, out JavaScriptValue evalResult);

        //[DllImport(DllName)]
        //public static extern JavaScriptErrorCode JsCopyString(JavaScriptValue value,StringBuilder buffer,uint bufferSize,out UIntPtr length);

        [DllImport(DllName,CharSet =CharSet.Unicode)]
        public static extern JavaScriptErrorCode JsCopyStringUtf16(JavaScriptValue value, int start, int length, StringBuilder stringBuilder, out UIntPtr size);

        [DllImport(DllName,CharSet =CharSet.Unicode)]
        public static extern JavaScriptErrorCode JsCreateStringUtf16(string value, UIntPtr size, out JavaScriptValue reference);
    }
}
