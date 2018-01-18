using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
namespace ChakraCore.NET.Plugin
{
    public static class PluginManagerHelper
    {
        public static PluginManager EnablePluginManager(this ChakraContext context)
        {
            var result= new PluginManager(context);
            result.Init();
            return result;
        }
        public static PluginManager AddPlugin<T>(this PluginManager manager,string name) where T:INativePluginInstaller,new()
        {
            manager.Loaders.Add(new SingleTypeLoader<T>(name));
            return manager;
        }

        public static PluginManager AddLoader(this PluginManager manager,IPluginLoader loader)
        {
            manager.Loaders.Add(loader);
            return manager;
        }

        public static PluginManager AddLoader(this PluginManager manager,string name,Func<INativePluginInstaller> createInstallerCallback)
        {
            manager.Loaders.Add(new DynamicLoader(name, createInstallerCallback));
            return manager;
        }
    }
}
