using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public interface INativeFunctionHolderService:IService
    {
        JavaScriptNativeFunction HoldFunction(JavaScriptNativeFunction function);
        
    }
}
