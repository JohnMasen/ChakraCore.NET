using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace ChakraCore.NET.Plugin
{
    public class FileSystemLoader : IPluginLoader
    {
        public string PluginRootFolder { get; private set; }
        string currentPluginFolder;
        public FileSystemLoader(string pluginRootFolder)
        {
            PluginRootFolder = pluginRootFolder;
            AssemblyLoadContext.Default.Resolving += Default_Resolving;
        }

        private Assembly Default_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            string fileName = Path.Combine(currentPluginFolder, $"{arg2.Name}.dll");
            return arg1.LoadFromAssemblyPath(fileName);
        }

        public INativePlugin Load(string name)
        {
            currentPluginFolder = Path.Combine(PluginRootFolder, name);
            if (!Directory.Exists(currentPluginFolder))
            {
                return null;
            }
            string dllName = Path.Combine(currentPluginFolder, $"{name}.dll");
            var dll=Assembly.LoadFile(dllName);
            string typeName = $"{name}.{name}";
            var result= dll.CreateInstance(typeName) as INativePlugin;
            return result;
        }

        
    }
}

