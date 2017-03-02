
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    /// <summary>
    /// Stores call context info
    /// </summary>
    public interface ICallContextService:IService
    {
        /// <summary>
        /// Current caller while you're calling to a javascript function, the thisArg in javascript
        /// </summary>
        JavaScriptValue Caller { get;  }
    }
}
