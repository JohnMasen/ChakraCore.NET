using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using ChakraCore.NET.Hosting;
using ChakraCore.NET.Plugin.Common;
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
                    .AddPlugin<SysInfoPluginInstaller>()
                    .AddModuleFolder(config.RootFolder)
                    .AddPlugin(new EchoProvider(new Echo()))
                    .AddModuleFolderFromCurrentAssembly()
                    .EnableHosting((moduleName) => { return hostingConfig; })
                    ;
                var session = new VSCodeDebugAdapter();
                hostingConfig.DebugAdapter = session;
                RunServer(session, 4711);
                //hostingConfig.DebugAdapter = new TestDebugAdapter();

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

        private static void RunSession(VSCodeDebugAdapter session, Stream inputStream, Stream outputStream)
        {
            session.Start(inputStream, outputStream).Wait();
        }

        private static void RunServer(VSCodeDebugAdapter session, int port)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            serverSocket.Start();
            Console.WriteLine($"StartListening at {serverSocket.LocalEndpoint}");
            new System.Threading.Thread(() => {
                while (true)
                {
                    var clientSocket = serverSocket.AcceptSocket();
                    if (clientSocket != null)
                    {
                        Console.WriteLine(">> accepted connection from client");

                        new System.Threading.Thread(() => {
                            using (var networkStream = new NetworkStream(clientSocket))
                            {
                                try
                                {
                                    RunSession(session,networkStream, networkStream);
                                }
                                catch (Exception e)
                                {
                                    Console.Error.WriteLine("Exception: " + e);
                                }
                            }
                            clientSocket.Close();
                            Console.Error.WriteLine(">> client connection closed");
                        }).Start();
                    }
                }
            }).Start();
        }
    }
}
