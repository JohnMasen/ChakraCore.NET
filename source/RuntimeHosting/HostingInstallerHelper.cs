using ChakraCore.NET;
using ChakraCore.NET.Plugin;
using RuntimeHosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Plugin
{
    public static class HostingInstallerHelper
    {
        public static PluginManager EnableHosting(this PluginManager manager, string rootFolder, Action<ChakraContext> initContextCallback)
        {
            manager.AddLoader("Hosting", () => { return new HostingInstaller(rootFolder, initContextCallback); });
            return manager;
        }
    }
}
