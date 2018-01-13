using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Plugin
{
    public static class FileSystemLoaderHelper
    {
        public static PluginManager SetPluginRootFolder(this PluginManager manager,string rootFolder)
        {
            manager.Loaders.Add(new FileSystemLoader(rootFolder));
            return manager;
        }
    }
}
