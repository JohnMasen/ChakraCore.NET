using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public static class JavaScriptHostingConfigHelper
    {
        public static JavaScriptHostingConfig AddModuleFileLoader(this JavaScriptHostingConfig config, LoadModuleFunction loadCallback)
        {
            config.AddModuleLoader(JavaScriptHostingConfig.MODULE_LOADER_PROTOCOL_FILE, loadCallback);
            return config;
        }

        public static JavaScriptHostingConfig AddModuleScript<T>(this JavaScriptHostingConfig config,string name,string script) 
        {
            config.AddModuleFileLoader((n)=> 
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
            config.AddModuleFileLoader(locator.LoadModule);
            return config;
        }


        public static JavaScriptHostingConfig AddPluginLoader(this JavaScriptHostingConfig config,LoadPluginInstallerFunction func)
        {
            config.PluginLoaders.Add(func);
            return config;
        }
        public static JavaScriptHostingConfig AddPlugin<T>(this JavaScriptHostingConfig config,string pluginName, Func<T> creationCallback) where T:IPluginInstaller
        {
            return config.AddPluginLoader((name) =>
            {
                if (name == pluginName)
                {
                    return creationCallback();
                }
                else
                {
                    return null;
                }
            });
        }

        public static JavaScriptHostingConfig AddPlugin<T>(this JavaScriptHostingConfig config, string pluginName) where T:IPluginInstaller,new()
        {
            return AddPlugin<T>(config, pluginName, () => { return new T(); });
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
