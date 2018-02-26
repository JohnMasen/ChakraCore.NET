using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using ChakraCore.NET.Hosting;
using ChakraCore.NET.Plugin.Common;
using ChakraCore.NET.DebugAdapter.VSCode;
using System.Threading;

namespace RunScript
{
    class Program
    {
        static void Main(string[] args)
        {
            CancellationTokenSource debugCTS=null;
            ScriptConfig config=null;
            JSApp app = null;
            if (args.Length == 0)
            {
                showUsage();
                return;
            }
            else
            {
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
                    .AddPlugin<SysInfoPluginInstaller>()
                    .AddModuleFolder(config.RootFolder)
                    .AddPlugin(new EchoProvider(new Echo()))
                    .AddModuleFolderFromCurrentAssembly()
                    .EnableHosting((moduleName) => { return hostingConfig; })
                    ;
                if (config.DebugMode)
                {
                    debugCTS = new CancellationTokenSource();
                    var adapter = new VSCodeDebugAdapter(true);
                    hostingConfig.DebugAdapter = adapter;
                    adapter.OnAdapterMessage += (sender, msg) => { Console.WriteLine(msg); };
                    adapter.OnStatusChang += (sender, e) => { Console.WriteLine(e); };
                    //RunServer(adapter, 4711);
                    adapter.RunServer(4711, debugCTS.Token); ;
                    //hostingConfig.DebugAdapter = new TestDebugAdapter();
                }


                string script = File.ReadAllText(config.File);
                Console.WriteLine("---Script Start---");
                if (config.IsModule)
                {
                    app=JavaScriptHosting.Default.GetModuleClass<JSApp>(config.FileName, config.ModuleClass, hostingConfig);
                    app.EntryPoint = config.ModuleEntryPoint;
                    app.Run();
                }
                else
                {
                    JavaScriptHosting.Default.RunScript(script, hostingConfig);
                }
            }
            if (config.IsModule)
            {
                Console.WriteLine("input \"exit\" to exit, anything else to run the module again");
                string command = Console.ReadLine();
                while (command != "exit")
                {
                    app.Run();
                    Console.WriteLine("input \"exit\" to exit, anything else to run the module again");
                    command = Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Press Enter to exit");
                Console.Read();
            }
            
            

            
            debugCTS?.Cancel();
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
