using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public static class JavaScriptHostingConfigHelper
    {
        public static JavaScriptHostingConfig AddModuleScript<T>(this JavaScriptHostingConfig config,string name,string script) 
        {
            config.ModuleFileLoaders.Add((n)=> 
            {
                if (n==name)
                {
                    return script; ;
                }
                return null;
            });
            return config;
        }

        public static JavaScriptHostingConfig AddModuleFolder(this JavaScriptHostingConfig config,string scriptRootFolder)
        {
            ModuleLocator locator = new ModuleLocator(scriptRootFolder);
            config.ModuleFileLoaders.Add(locator.LoadModule);
            return config;
        }

        public static JavaScriptHostingConfig AddPlugin(this JavaScriptHostingConfig config, LoadPluginInstallerFunction loadCallback)
        {
            config.PluginLoaders.Add(loadCallback);
            return config;
        }
        public static JavaScriptHostingConfig AddPlugin(this JavaScriptHostingConfig config,IPluginInstaller plugin)
        {
            return config.AddPlugin((name)=>
            {
                if (plugin.Name==name)
                {
                    return plugin;
                }
                return null;
            });
        }

        public static JavaScriptHostingConfig AddPlugin<T>(this JavaScriptHostingConfig config) where T:IPluginInstaller,new()
        {
            return config.AddPlugin(new T());
        }

        public static JavaScriptHostingConfig Clone(this JavaScriptHostingConfig config)
        {
            return new JavaScriptHostingConfig(config);
        }
#if NETSTANDARD2_0
        public static JavaScriptHostingConfig AddModuleFolderFromCurrentAssembly(this JavaScriptHostingConfig config)
        {
            string folder = new System.IO.FileInfo(System.Reflection.Assembly.GetCallingAssembly().Location).DirectoryName;
            return config.AddModuleFolder(folder);
        }
#endif
    }
}
