using ChakraCore.NET;
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RunScript
{
    public class PluginInstaller
    {
        private string root;
        string pluginFolder;
        ChakraContext context;
        const string pluginContainerName = "__API__";
        public PluginInstaller(string rootPath, ChakraContext context)
        {
            root = rootPath;
            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += Default_Resolving;
            this.context = context;
            context.RunScript($"var {pluginContainerName}={{}}");
            context.GlobalObject.Binding.SetFunction<string, JavaScriptValue>("RequireNative", RequireNative);
        }

        private System.Reflection.Assembly Default_Resolving(System.Runtime.Loader.AssemblyLoadContext arg1, System.Reflection.AssemblyName arg2)
        {
            string filename = Path.Combine(pluginFolder, arg2.Name + ".dll");
            Console.Write($"Loading dependency file {filename}");
            
            var result= System.Reflection.Assembly.LoadFile(filename);
            Console.WriteLine("...Done");
            return result;

        }

        

        public JavaScriptValue RequireNative(string name)
        {
            Console.WriteLine($"Native Plugin {name} required");
            pluginFolder = Path.Combine(root, name);
            string dllName = Path.Combine(pluginFolder, $"{name}.dll");
            Console.Write($"Loading Native Plugin {dllName}");
            var loaded = System.Reflection.Assembly.LoadFile(dllName);
            Console.WriteLine("...Done");
            var instance=loaded.CreateInstance($"{name}.{name}");

            //find the native api container
            JSValue container = context.GlobalObject.ReadProperty<JSValue>(pluginContainerName);
            //create new api stub
            JavaScriptValue result = JavaScriptValue.CreateObject();
            container.WriteProperty<JavaScriptValue>("api__"+Guid.NewGuid().ToString().Replace('-','_'),result);

            //wrap the result to JSValue for the plugin install on
            JSValue stub = new JSValue(container.ServiceNode, result);
            (instance as INativePlugin).Install(stub);

            Console.WriteLine($"Native Plugin {name} installed");
            return result;
        }

        

        
    }
}
