using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class PluginInstallerNotFoundException:Exception
    {
        public PluginInstallerNotFoundException(string pluginName):base($"Can not found installer for plugin [{pluginName}]")
        {

        }
    }
}
