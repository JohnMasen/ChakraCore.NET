
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public interface IProxyMapService:IService,IDisposable
    {
        /// <summary>
        /// Create a javascript value as proxy which maps to a user object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="createBinding"></param>
        /// <returns></returns>
        JavaScriptValue Map<T>(T obj, Action<JSValueBinding,T,IServiceNode> createBinding) where T : class;
        /// <summary>
        /// Get a mapped user object from mapping table with the javascript value
        /// </summary>
        /// <typeparam name="T">Type of user object</typeparam>
        /// <param name="value">The mapped value</param>
        /// <returns>The mapped user object</returns>
        T Get<T>(JavaScriptValue value) where T : class;

        /// <summary>
        /// Release a mapped user object
        /// </summary>
        /// <typeparam name="T">Type of user object</typeparam>
        /// <param name="obj">The user object to release</param>
        void Release<T>(T obj) where T : class;

        /// <summary>
        /// Release a mapped user object
        /// </summary>
        /// <typeparam name="T">Type of user object</typeparam>
        /// <param name="value">The mapped javascript value</param>
        void Release<T>(JavaScriptValue value) where T : class;

        /// <summary>
        /// Clear mapping table
        /// </summary>
        void ReleaseAll();
    }
}
