
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    /// <summary>
    /// Provides javascript value manipulation service
    /// </summary>
    public interface IJSValueService:IService
    {
        /// <summary>
        /// Read a property from a <see cref="JavaScriptValue" and convert the result to a specified type/>
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="target">The javascript object where to retrieve the property value</param>
        /// <param name="id">Property id</param>
        /// <returns>A user defined type converted from retrieved value</returns>
        T ReadProperty<T>(JavaScriptValue target, JavaScriptPropertyId id);

        /// <summary>
        /// Write a property to a <see cref="JavaScriptValue"/> with a specified type
        /// </summary>
        /// <typeparam name="T">Type of the property value</typeparam>
        /// <param name="target">Type of the value</param>
        /// <param name="id">Property id</param>
        /// <param name="value"></param>
        void WriteProperty<T>(JavaScriptValue target,JavaScriptPropertyId id,T value);

        /// <summary>
        /// Read a property from a <see cref="JavaScriptValue" and convert the result to a specified type/>
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="target">The javascript object where to retrieve the property value</param>
        /// <param name="id">Property id</param>
        /// <returns>A user defined type converted from retrieved value</returns>
        T ReadProperty<T>(JavaScriptValue target, string id);
        /// <summary>
        /// Write a property to a <see cref="JavaScriptValue"/> with a specified type
        /// </summary>
        /// <typeparam name="T">Type of the property value</typeparam>
        /// <param name="target">Type of the value</param>
        /// <param name="id">Property id</param>
        /// <param name="value"></param>
        void WriteProperty<T>(JavaScriptValue target, string id, T value);
        /// <summary>
        /// Check if <paramref name="target"/> has a specified property
        /// </summary>
        /// <param name="target">the javascript object to perform the check</param>
        /// <param name="id">property id</param>
        /// <returns>True if contains the property, otherwise False</returns>
        bool HasProperty(JavaScriptValue target, JavaScriptPropertyId id);

        /// <summary>
        /// Check if <paramref name="target"/> has a specified property
        /// </summary>
        /// <param name="target">the javascript object to perform the check</param>
        /// <param name="id">property id</param>
        /// <returns>True if contains the property, otherwise False</returns>
        bool HasProperty(JavaScriptValue target, string id);
        /// <summary>
        /// Get the javascript undefined value from current context
        /// </summary>
        JavaScriptValue JSValue_Undefined { get; }
        /// <summary>
        /// Get the javascript null value from current context
        /// </summary>
        JavaScriptValue JSValue_Null { get; }
        /// <summary>
        /// Get the javascript true value from current context
        /// </summary>
        JavaScriptValue JSValue_True { get; }
        /// <summary>
        /// Get the javascript false value from current context
        /// </summary>
        JavaScriptValue JSValue_False { get; }
        /// <summary>
        /// Get the global object from current context
        /// </summary>
        JavaScriptValue JSGlobalObject { get; }

        /// <summary>
        /// Creates a javascript function points to a dotnet delegate
        /// </summary>
        /// <param name="function">The dotnet delegate to perform you actual operate</param>
        /// <param name="callbackData">Extra data that will be transfered while calling to the function, can be null</param>
        /// <returns>A javascript function</returns>
        JavaScriptValue CreateFunction(JavaScriptNativeFunction function, IntPtr callbackData);
        /// <summary>
        /// Creates an empty javascript object, equals "return {}" in javascript
        /// </summary>
        /// <returns>A javascript object</returns>
        JavaScriptValue CreateObject();

        /// <summary>
        /// Create an external object
        /// </summary>
        /// <remarks>
        /// External object is a javascript object with extra data, you can also specify a callback when the object has been collected by javascript GC
        /// </remarks>
        /// <param name="data">the extra data sticked with the new created object, can be null(IntPtr.Zero)</param>
        /// <param name="finalizeCallback">Callback when the object ben collected, can be null</param>
        /// <returns>A javascript object</returns>
        JavaScriptValue CreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback);

        /// <summary>
        /// Call to a javascript function, equals "return targetFunction()" in javascript
        /// </summary>
        /// <remarks>
        /// the first parameter should be the owner of the function(thisArg in javascript language)
        /// </remarks>
        /// <param name="target">The javascript function to be called</param>
        /// <param name="para">parameters, the first parameter should be the owner of the function(thisArg in javascript language)</param>
        /// <returns>the result of the function call</returns>
        JavaScriptValue CallFunction(JavaScriptValue target, params JavaScriptValue[] para);

        /// <summary>
        /// Call to a javascript function as constructor, equals "return new targetFunction()" in javascript
        /// </summary>
        /// <remarks>
        /// the first parameter should be the owner of the function(thisArg in javascript language)
        /// </remarks>
        /// <param name="target">The javascript function to be called</param>
        /// <param name="para">parameters, the first parameter should be the owner of the function(thisArg in javascript language)</param>
        /// <returns>the result of the function call</returns>
        JavaScriptValue ConstructObject(JavaScriptValue target, params JavaScriptValue[] para);

        /// <summary>
        /// Create a javascript array
        /// </summary>
        /// <param name="size">array size</param>
        /// <returns>a javascript array</returns>
        JavaScriptValue CreateArray(uint size);


        /// <summary>
        /// Throw an exception if the specified value is an javascript error object
        /// </summary>
        /// <param name="value">javascript error object</param>
        void ThrowIfErrorValue(JavaScriptValue value);
    }
}
