using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    
    public class JavaScriptHostingConfig
    {

        public List<LoadPluginInstallerFunction> PluginLoaders { get; private set; } = new List<LoadPluginInstallerFunction>();
        public List<LoadModuleFunction> ModuleLoaders { get; private set; } = new List<LoadModuleFunction>();
        public JavaScriptHostingConfig()
        {

        }
        public JavaScriptHostingConfig(JavaScriptHostingConfig source)
        {
            PluginLoaders.AddRange(source.PluginLoaders);
            ModuleLoaders.AddRange(source.ModuleLoaders);
        }

        public string LoadModule(string name)
        {
            foreach (var item in ModuleLoaders)
            {
                string result = item(name);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            throw new ModuleNotFoundException(name);
        }

        public IPluginInstaller LoadPlugin(string name)
        {
            foreach (var item in PluginLoaders)
            {
                var result = item(name);
                if (result!=null)
                {
                    return result;
                }
            }
            throw new PluginInstallerNotFoundException(name);
        }

    }
}
