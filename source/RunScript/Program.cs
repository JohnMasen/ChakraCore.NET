using System;
using System.Text;
using ChakraCore.NET;
using Microsoft.Extensions.DependencyInjection;

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

                var context = prepareContext();
                if (!string.IsNullOrEmpty(config.PluginRootFolder))
                {
                    PluginInstaller.InstallPlugins(config.PluginRootFolder, context);
                }
                string script = System.IO.File.ReadAllText(config.File);
                if (config.IsModule)
                {
                    var jsClass = context.ProjectModuleClass(config.FileName, config.ModuleClass, config.RootFolder);
                    jsClass.CallMethod(config.ModuleEntryPoint);
                }
                else
                {
                    context.RunScript(script);
                }
            }

            Console.WriteLine("Press any key to exit");
            Console.Read();
        }
        

        static ChakraContext prepareContext()
        {
            ChakraRuntime runtime = ChakraRuntime.Create();
            ChakraContext result = runtime.CreateContext(false);
            //result.GlobalObject.Binding.SetMethod<string>("echo", s => { Console.WriteLine(s); });
            return result;
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
