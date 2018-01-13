using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Plugin
{
    public class ManualPluginLoader : IPluginLoader
    {
        private Dictionary<string, INativePlugin> items = new Dictionary<string, INativePlugin>();
        public void RegisterPlugin(string name, INativePlugin plugin)
        {
            items.Add(name, plugin);
        }

        public void RegisterPlugin<T>(string name) where T:INativePlugin,new()
        {
            items.Add(name, new T());
        }
        public INativePlugin Load(string name)
        {
            items.TryGetValue(name, out INativePlugin result);
            return result;
        }

    }
}
