using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChakraCore.NET;
using ChakraCore.NET.Plugin;
namespace RunScript
{
    public class HostingInstaller : INativePluginInstaller
    {
        public static string PluginFolder;
        public static string RootFolder;
        public void Install(JSValue stub)
        {
            var converter = stub.ServiceNode.GetService<IJSValueConverterService>();
            registerProxy(converter);
            stub.Binding.SetFunction<string, string, Task<HostingProxy>>("CreateHosting", CreateHosting);
        }

        private static void registerProxy(IJSValueConverterService converter)
        {
            converter.RegisterProxyConverter<HostingProxy>(
                (binding, obj, node) =>
                {
                    binding.SetFunction<string, string, Task<string>>("Dispatch", obj.Dispatch);
                    //binding.SetFunction<string, string, Task<string>>("DispatchAsync", obj.DispatchAsync);
                }


                );
            converter.RegisterTask<HostingProxy>();
            converter.RegisterTask<string>();
        }

        public static Task<HostingProxy> CreateHosting(string moduleName, string className)
        {
            return Task.Factory.StartNew(() =>
            {
                var runtime = ChakraRuntime.Create();
                var context = runtime.CreateContext(false);
                context
                    .EnablePluginManager()
                    .AddPlugin<SysInfoPluginInstaller>("SysInfo")
                    .SetPluginRootFolder(PluginFolder);
                var converter = context.ServiceNode.GetService<IJSValueConverterService>();
                registerProxy(converter);
                var value = context.ProjectModuleClass(moduleName, className, RootFolder);
                return new HostingProxy(value);
            });
        }
    }

    public class HostingProxy
    {
        JSValue reference;
        public HostingProxy(JSValue value)
        {
            reference = value;
        }
        public Task<string> Dispatch(string functionName, string JSONparameter)
        {
            return Task.Factory.StartNew(() =>
            {
                var x = reference.CallFunction<string, string, string>("Dispatch", functionName, JSONparameter);
                return x;
            });

        }

        /// <summary>
        /// Do not use, still have potencial thread conflict issue, may cause application crash
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="JSONparameter"></param>
        /// <returns></returns>
        public Task<string> DispatchAsync(string functionName, string JSONparameter)
        {
            //
            var t = Task.Factory.StartNew(() =>
              {
                  var x = reference.CallFunctionAsync<string, string, string>("DispatchAsync", functionName, JSONparameter);
                  x.Wait(); //force wait on caller thread, otherwise may cause chakracontext thread confilict
                  return x.Result;
              });
            return t;
        }
    }


}
