
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public interface ICallContextService:IService
    {
        JavaScriptValue Caller { get;  }
    }
}
