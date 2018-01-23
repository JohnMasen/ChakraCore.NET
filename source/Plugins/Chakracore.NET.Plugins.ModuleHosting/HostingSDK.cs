using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Hosting;
namespace ChakraCore.NET.Plugin.ModuleHosting
{
    public class HostingSDK : IScriptSDKProvider
    {
        public string SDK => Properties.Resources.Hosting;
    }
}
