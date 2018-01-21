using System;
using System.IO;
using System.Text;
using ChakraCore.NET.Hosting;
namespace RunScript
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                showUsage();
            }
            else
            {
                ScriptConfig config = null;
                try
                {
                    config = ScriptConfig.Parse(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("invalid parameter");
                    Console.WriteLine(ex.Message);
                    showUsage();
                    Console.WriteLine("Press any key to exit");
                    Console.Read();
                    return;
                }
                JavaScriptHostingConfig hostingConfig = new JavaScriptHostingConfig();
                hostingConfig
                    .AddPluginFolder(config.PluginRootFolder)
                    .AddPlugin<SysInfoPluginInstaller>("SysInfo")
                    .AddModuleFolder(config.RootFolder)
                    .EnableHosting((moduleName) => { return hostingConfig; });
                    
                
                    
                string script = File.ReadAllText(config.File);
                Console.WriteLine("---Script Start---");
                if (config.IsModule)
                {
                    var app=JavaScriptHosting.Default.GetModuleClass<JSApp>(config.FileName, config.ModuleClass, hostingConfig);
                    app.EntryPoint = config.ModuleEntryPoint;
                    app.Run();
                }
                else
                {
                    JavaScriptHosting.Default.RunScript(script, hostingConfig);
                }
            }

            Console.WriteLine("Press Enter to exit");
            Console.Read();
        }
        

        

        static void showUsage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin(
                Environment.NewLine
                , "RunScript useage:"
                , "/file:FileName                    run a javascript file"
                , "/module                           run a javascript as module"
                , "/class:ClassName                  the entrypoint class name of module, default is \"app\""
                , "/entrypoint:FunctionName          the entrypoint function name of module, default is \"main\""
                );
            Console.WriteLine(sb);
        }
    }
}
