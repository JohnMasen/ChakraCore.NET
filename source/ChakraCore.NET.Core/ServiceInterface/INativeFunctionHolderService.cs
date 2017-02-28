
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public interface INativeFunctionHolderService:IService
    {
        JavaScriptNativeFunction HoldFunction(JavaScriptNativeFunction function);
        
    }
}
