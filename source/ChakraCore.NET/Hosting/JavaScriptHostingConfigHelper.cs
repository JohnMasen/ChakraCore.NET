using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public static class JavaScriptHostingConfigHelper
    {
        public static JavaScriptHostingConfig AddModuleLoader(this JavaScriptHostingConfig config, LoadModuleFunction func)
        {
            config.ModuleLoaders.Add(func);
            return config;
        }

        public static JavaScriptHostingConfig AddModuleScript<T>(this JavaScriptHostingConfig config,string name,string script) 
        {
            config.AddModuleLoader((n)=> 
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
            config.AddModuleLoader(locator.LoadModule);
            return config;
        }


        public static JavaScriptHostingConfig AddPluginLoader(this JavaScriptHostingConfig config,LoadPluginInstallerFunction func)
        {
            config.PluginLoaders.Add(func);
            return config;
        }


    }
}
