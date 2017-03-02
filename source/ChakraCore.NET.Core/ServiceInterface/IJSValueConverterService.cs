
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    /// <summary>
    /// Delegate that transfer a user object to a <see cref="JavaScriptValue"/>
    /// </summary>
    /// <typeparam name="T">Type of user object</typeparam>
    /// <param name="serviceNode">Service node which provides necessary service during the translation</param>
    /// <param name="value">The user object</param>
    /// <returns>A <see cref="JavaScriptValue" /> which represents the user object</returns>
    public delegate JavaScriptValue toJSValueDelegate<T>(IServiceNode serviceNode, T value);

    /// <summary>
    /// Delegate that transfer a <see cref="JavaScriptValue"/> to a user object
    /// </summary>
    /// <typeparam name="TResult">Type of user object</typeparam>
    /// <param name="serviceNode">Service node which provides necessary service during the translation</param>
    /// <param name="value">The <see cref="JavaScriptValue"/> to be transfer </param>
    /// <returns>The user object</returns>
    public delegate TResult fromJSValueDelegate<out TResult>(IServiceNode serviceNode, JavaScriptValue value);

    /// <summary>
    /// Translate the user object from/to <see cref="JavaScriptValue"/>
    /// </summary>
    public interface IJSValueConverterService:IService
    {
        /// <summary>
        /// Register a converter
        /// </summary>
        /// <typeparam name="T">Type of user object to translate</typeparam>
        /// <param name="toJSValue">Converter function for translate user object to <see cref="JavaScriptValue"/> </param>
        /// <param name="fromJSValue">Converter function for translate <see cref="JavaScriptValue"/> to user object </param>
        /// <param name="throwIfExists">Throwss an exception of the type is already registered</param>
        /// <exception cref="ArgumentException">If the type is already registered and <paramref name="throwIfExists"/> is true </exception>
        void RegisterConverter<T>(toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue, bool throwIfExists = true);
        /// <summary>
        /// Convert a user object from a <see cref="JavaScriptValue"/>
        /// </summary>
        /// <typeparam name="T">Type of user object</typeparam>
        /// <param name="value">the value to convert from</param>
        /// <returns>a instance of user object</returns>
        T FromJSValue<T>(JavaScriptValue value);

        /// <summary>
        /// Convert a user object to a <see cref="JavaScriptValue"/>
        /// </summary>
        /// <typeparam name="T">Type of user object</typeparam>
        /// <param name="value">user object</param>
        /// <returns>A <see cref="JavaScriptValue" /> which represents the user object</returns>
        JavaScriptValue ToJSValue<T>(T value);

        /// <summary>
        /// Check if a user type is registered for convert
        /// </summary>
        /// <typeparam name="T">Type of user object</typeparam>
        /// <returns>True is already registerd, otherwise False</returns>
        bool CanConvert<T>();
       
    }
}
