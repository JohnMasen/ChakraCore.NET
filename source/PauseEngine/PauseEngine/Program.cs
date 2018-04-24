using ChakraCore.NET;
using ChakraCore.NET.Hosting;
using ChakraCore.NET.Plugin.Common;
using System;
using System.Threading.Tasks;

namespace PauseEngine
{
    class Program
    {
        static EchoProvider echo = new EchoProvider(new EchoFunctionProvider());
        static void Main(string[] args)
        {
            JavaScriptHostingConfig config = new JavaScriptHostingConfig();
            config.AddModuleFolder("Scripts");
            config.AddPlugin(echo);

            InterruptableHost host = new InterruptableHost();
            var app=host.GetModuleClass("app", "app", config);
            var runtimeService = app.ServiceNode.GetService<IRuntimeService>();
            StopEngine(runtimeService);
            var switchService = app.ServiceNode.GetService<IContextSwitchService>();
            try
            {
                app.CallMethod("loop");
            }
            catch (Exception ex) when( ex.Message== "Script was terminated.")
            {
                //switchService.Leave();
                Console.WriteLine("Script terminated");
                runtimeService.Disabled = false;
                Console.WriteLine("Engine Enabled");
                
            }
            catch(Exception)
            {
                throw;
            }
            //GC.KeepAlive(host);
            Console.Read();

        }
        private static async void StopEngine(IRuntimeService service)
        {
            await Task.Delay(3000);
            Console.WriteLine("Trying to stop engine");
            service.Disabled = true;
            Console.WriteLine("Engine Disabled");
        }
    }
}
