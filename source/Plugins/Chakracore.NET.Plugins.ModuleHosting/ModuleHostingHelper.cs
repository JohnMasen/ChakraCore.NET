using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Plugin.ModuleHosting;

namespace ChakraCore.NET.Hosting
{
    public static class ModuleHostingHelper
    {
        public static JavaScriptHostingConfig EnableHosting(this JavaScriptHostingConfig config,ResolveConfigFunction resolve)
        {
            ModuleHosting hosting= new ModuleHosting(resolve);
            return config.AddPlugin("Hosting", () => { return hosting; });
        }
    }
}
