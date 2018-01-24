using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    //public struct ModuleExportInfo
    //{
    //    public string ModuleName;
    //    public string ClassName;
    //    public string EntryPointName;
    //}
    public class JavaScriptHostingConfig : IJavaScriptHostingConfig
    {
        public const string MODULE_LOADER_PROTOCOL_FILE = "file";
        public const string MODULE_LOADER_PROTOCOL_SDK = "sdk";
        public const string PLUGIN_LOADER_PROTOCOL_INSTANCE = "instance";
        public ProtocolResolver<IPluginInstaller> RequireNativeResolver { get; private set; } = new ProtocolResolver<IPluginInstaller>(PLUGIN_LOADER_PROTOCOL_INSTANCE);
        public ProtocolResolver<string> ModuleScriptResolver { get; private set; } = new ProtocolResolver<string>(MODULE_LOADER_PROTOCOL_FILE);

        public List<LoadPluginInstallerFunction> PluginLoaders { get; private set; } = new List<LoadPluginInstallerFunction>();
        public List<LoadModuleFunction> ModuleFileLoaders { get; private set; } = new  List<LoadModuleFunction>();
        
        private IPluginInstaller getPlugin(string name)
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

        public JavaScriptHostingConfig()
        {
            RequireNativeResolver.Add(getPlugin);//add default protocol resolver
            ModuleScriptResolver.Add(loadModuleFile);//add default file loader resolver
            ModuleScriptResolver.Add(MODULE_LOADER_PROTOCOL_SDK, (name)=> { return getPlugin(name).GetSDK(); });
        }

        private string loadModuleFile(string name)
        {
            foreach (var item in ModuleFileLoaders)
            {
                var result = item(name);
                if (result!=null)
                {
                    return result;
                }
            }
            throw new ModuleNotFoundException(name,"file");
        }

        public JavaScriptHostingConfig(JavaScriptHostingConfig source):this()
        {
            PluginLoaders.AddRange(source.PluginLoaders);
            ModuleFileLoaders.AddRange(source.ModuleFileLoaders);
        }
        



        public string LoadModule(string name) => ModuleScriptResolver.Process(name);

        public IPluginInstaller LoadPlugin(string name) => RequireNativeResolver.Process(name);

    }
}
