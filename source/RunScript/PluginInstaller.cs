using ChakraCore.NET;
using System;
using System.Collections.Generic;
using System.Text;

namespace RunScript
{
    public class PluginInstaller
    {
        IEnumerable<INativePlugin> items;
        public PluginInstaller(IEnumerable<INativePlugin> plugins)
        {
            items = plugins;
        }
        public void Install(ChakraContext context)
        {
            foreach (var item in items)
            {
                Console.Write($"Installing {item.GetType().ToString()}...");
                item.Install(context);
                Console.WriteLine("done");
            }
        }

        public static void InstallPlugins(string path,ChakraContext context)
        {
            StructureMap.Container container = new StructureMap.Container();
            container.Configure(config =>
            {
                //config.For<PluginInstaller>();
                config.Scan(_ =>
                {
                    Console.WriteLine("Loading primary dll");
                    _.AssembliesFromApplicationBaseDirectory();
                    Console.WriteLine($"Scanning folder {path}");
                    _.AssembliesFromPath(path);
                    _.AddAllTypesOf<INativePlugin>();
                });
            });
            var installer = container.GetInstance<PluginInstaller>();
            installer.Install(context);
        }
    }
}
