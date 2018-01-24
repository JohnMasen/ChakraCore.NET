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
    public class JavaScriptHostingConfig
    {
        public const string MODULE_LOADER_PROTOCOL_FILE = "file";
        public const string MODULE_LOADER_PROTOCOL_SDK = "sdk";
        public List<LoadPluginInstallerFunction> PluginLoaders { get; private set; } = new List<LoadPluginInstallerFunction>();
        public Dictionary<string,List<LoadModuleFunction>> ModuleLoaders { get; private set; } = new Dictionary<string, List<LoadModuleFunction>>();
        public static IPluginInstaller DefaultPluginInstaller(string pluginTypeName)
        {
            Type pluginType = Type.GetType(pluginTypeName, false);
            if (pluginType != null)
            {
                return Activator.CreateInstance(pluginType) as IPluginInstaller;
            }
            return null;
        }

        public static string DefaultModuleSDKLoader(string sdkTypeName)
        {
            Type sdkType = Type.GetType(sdkTypeName,false);
            if (sdkType!=null)
            {
                return (Activator.CreateInstance(sdkType) as IScriptSDKProvider)?.SDK;
            }
            return null;
        }

        public JavaScriptHostingConfig(bool enableDefaultPluginInstaller=true,bool enableDefaultModuleSDKLoader=true)
        {
            if (enableDefaultPluginInstaller)
            {
                PluginLoaders.Add(DefaultPluginInstaller);
            }
            if (enableDefaultModuleSDKLoader)
            {
                AddModuleLoader(MODULE_LOADER_PROTOCOL_SDK, DefaultModuleSDKLoader);
            }

        }
        
        public void AddModuleLoader(string protocol, LoadModuleFunction loadCallback)
        {
            protocol = protocol.ToLower();
            if (ModuleLoaders.TryGetValue(protocol,out var result))
            {
                result.Add(loadCallback);
            }
            else
            {
                ModuleLoaders.Add(protocol, new List<LoadModuleFunction>() { loadCallback });
            }
        }

        public JavaScriptHostingConfig(JavaScriptHostingConfig source)
        {
            PluginLoaders.AddRange(source.PluginLoaders);
            foreach (var item in source.ModuleLoaders)
            {
                ModuleLoaders.Add(item.Key, item.Value);
            }
            
        }

        public string LoadModule(string name)
        {
            var tmp = name.Split('@');
            string protocol;
            string address;
            switch (tmp.Length)
            {
                case 1:
                    protocol = MODULE_LOADER_PROTOCOL_FILE;
                    address = tmp[0];
                    break;
                case 2:
                    protocol = tmp[0].ToLower();
                    address = tmp[1];
                    break;
                default:
                    throw new ArgumentException("Invalid module name");
            }
            
            if (ModuleLoaders.ContainsKey(protocol))
            {
                foreach (var item in ModuleLoaders[protocol])
                {
                    string result = item(address);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                }
                throw new ModuleNotFoundException(name,protocol);
            }
            else
            {
                throw new UnknownModuleResolveProtocolException(protocol);
            }
        }

        public IPluginInstaller LoadPlugin(string name)
        {
            foreach (var item in PluginLoaders)
            {
                var result = item(name);
                if (result != null)
                {
                    return result;
                }
            }
            throw new PluginInstallerNotFoundException(name);
        }

    }
}
