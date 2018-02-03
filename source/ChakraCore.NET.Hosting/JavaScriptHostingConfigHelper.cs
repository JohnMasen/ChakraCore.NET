using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
#if UAP10_0_16299
using Windows.Storage;
#endif

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
#if UAP10_0_16299
        public static JavaScriptHostingConfig AddModuleFolder(this JavaScriptHostingConfig config,StorageFolder folder )
        {
            config.ModuleFileLoaders.Add((name) =>
            {
                var t = loadModuleFromFolderAsync(folder, name);
                t.Wait();
                return t.Result;
            });
            return config;
        }
        private static async Task<string> loadModuleFromFolderAsync(StorageFolder folder,string name)
        {
            return
                await loadModuleFromRootAsync(folder, null, $"{name}.js")
                ?? await loadModuleFromRootAsync(folder, name, $"{name}.js")
                ?? await loadModuleFromRootAsync(folder, name, "index.js");
        }

        private static async Task<string> loadModuleFromRootAsync(StorageFolder folder, string folderName, string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(folderName))
                {
                    folder = await folder.GetFolderAsync(folderName);
                }
                var file = await folder.GetFileAsync(fileName);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }
#endif
    }
}
