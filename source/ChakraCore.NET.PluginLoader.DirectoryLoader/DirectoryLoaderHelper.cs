using ChakraCore.NET.PluginLoader.DirectoryLoader;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public static class DirectoryLoaderHelper
    {
        public static JavaScriptHostingConfig AddPluginFolder(this JavaScriptHostingConfig config, string folderPath)
        {
            var loader = new DirectoryLoader(folderPath);
            return config.AddPluginLoader(loader.Load);
        }
    }
}
