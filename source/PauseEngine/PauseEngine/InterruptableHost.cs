using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
using ChakraCore.NET.API;

namespace PauseEngine
{
    class InterruptableHost:ChakraCore.NET.Hosting.JavaScriptHosting
    {
        protected override ChakraRuntime createRuntime()
        {
            return ChakraRuntime.Create(JavaScriptRuntimeAttributes.AllowScriptInterrupt);
        }
    }
}
