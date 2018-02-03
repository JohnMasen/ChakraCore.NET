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
            ModuleScriptResolver.Add(MODULE_LOADER_PROTOCOL_SDK, (name)=> 
            {
                string result = createInstance<ISDKProvider>(name)?.GetSDK();
                if (result==null)
                {
                    foreach (var item in PluginLoaders)
                    {
                        var tmp = item(name);
                        if (tmp != null)
                        {
                            return tmp.GetSDK();
                        }
                    }
                }
                else
                {
                    return result;
                }
                throw new SDKProviderNotFoundException(name);
            });
        }

        private T createInstance<T>(string typeName) where T:class
        {
            Type t;
            t=Type.GetType(typeName, false);
            if (t==null)
            {
                var tmp = typeName.Split(',');
                if (tmp.Length==2)
                {
                    t = Type.GetType($"{tmp[1]}.{tmp[0]},{tmp[1]}",false);
                    if (t==null)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return Activator.CreateInstance(t) as T;
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
