using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Plugin
{
    public interface IPluginLoader
    {
        INativePlugin Load(string name);
    }
}
