using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ChakraCore.NET
{
    public interface IGCSyncService:IService
    {
        /// <summary>
        /// Hold the managed object until the specified jsValue is released
        /// </summary>
        /// <param name="obj">managed object to hold</param>
        /// <param name="jsValue">javascript value to monitor</param>
        GCHandle SyncWithJsValue(object obj, JavaScriptValue jsValue);

        /// <summary>
        /// Create an disposable object which references to a specified javascript value
        /// chakracore GC woun't collect the value until the disposable object is disposed
        /// </summary>
        /// <param name="jsValue">the javascript value to be referenced</param>
        /// <returns>an object which internal references the jsValue</returns>
        IDisposable CreateJsGCWrapper(JavaScriptValue jsValue);
    }

}
