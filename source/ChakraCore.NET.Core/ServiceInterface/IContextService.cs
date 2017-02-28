
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public interface IContextService:IService
    {
        string RunScript(string script);
        JavaScriptValue ParseScript(string script);
        
        
    }
}
