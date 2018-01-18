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
        private string rootFolder;
        private Action<ChakraContext> initContext;
        internal HostingInstaller(string rootFolder,Action<ChakraContext> initContext)
        {
            this.rootFolder = rootFolder;
            this.initContext = initContext;
        }
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

        public Task<HostingProxy> CreateHosting(string moduleName, string className)
        {
            return Task.Factory.StartNew(() =>
            {
                var runtime = ChakraRuntime.Create();
                var context = runtime.CreateContext(false);
                initContext?.Invoke(context);
                var converter = context.ServiceNode.GetService<IJSValueConverterService>();
                registerProxy(converter);
                var value = projectProxyClass(context, moduleName, className);
                return new HostingProxy(value);
            });
        }

        public JSValue projectProxyClass(ChakraContext context, string moduleName,string className)
        {
            string projectTo = "__Proxy__";
            string script_setRootObject = $"var {projectTo}={{}};";
            string script_importModule = Properties.Resources.ResourceManager.GetString("JSProxy")
                .Replace("{className}", className)
                .Replace("{moduleName}", moduleName)
                .Replace("{exportClass}", "proxy")
                .Replace("{exportValue}", projectTo);
            ModuleLocator locator = new ModuleLocator(rootFolder);
            context.RunScript(script_setRootObject);
            context.RunModule(script_importModule, locator.LoadModule);
            return context.GlobalObject.ReadProperty<JSValue>(projectTo);
        }
    }

    public static class HostingInstallerHelper
    {
        public static PluginManager EnableHosting(this PluginManager manager,string rootFolder,Action<ChakraContext> initContextCallback)
        {
            manager.AddLoader("Hosting", () => { return new HostingInstaller(rootFolder, initContextCallback); });
            return manager;
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
                var x = reference.CallFunction<string, string, string>("__Dispatch__", functionName, JSONparameter);
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
