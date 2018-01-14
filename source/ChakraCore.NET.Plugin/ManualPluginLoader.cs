using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Plugin
{
    public class ManualPluginLoader : IPluginLoader
    {
        private Dictionary<string, INativePluginInstaller> items = new Dictionary<string, INativePluginInstaller>();
        public void RegisterPlugin(string name, INativePluginInstaller plugin)
        {
            items.Add(name, plugin);
        }

        public void RegisterPlugin<T>(string name) where T:INativePluginInstaller,new()
        {
            items.Add(name, new T());
        }
        public INativePluginInstaller Load(string name)
        {
            items.TryGetValue(name, out INativePluginInstaller result);
            return result;
        }

    }
}
