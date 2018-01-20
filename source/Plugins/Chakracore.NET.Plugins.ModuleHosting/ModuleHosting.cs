using ChakraCore.NET.Hosting;
using ChakraCore.NET.Plugin.ModuleHosting.Properties;
using System;
using System.Threading.Tasks;

namespace ChakraCore.NET.Plugin.ModuleHosting
{
    public delegate JavaScriptHostingConfig ResolveConfigFunction(string moduleName);
    public class ModuleHosting : IPluginInstaller
    {
        private Action<ChakraContext> initContext;
        private ResolveConfigFunction resolveConfig;
        public ModuleHosting(ResolveConfigFunction resolve)
        {
            this.resolveConfig = resolve;
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
                }
                );
            converter.RegisterTask<HostingProxy>();
            converter.RegisterTask<string>();
        }

        public Task<HostingProxy> CreateHosting(string moduleName, string className)
        {
            var config = resolveConfig(moduleName);
            return JavaScriptHosting.Default.GetModuleClassAsync<HostingProxy>(Resources.JSProxy, moduleName, className, config);
        }

        //public JSValue projectProxyClass(ChakraContext context, string moduleName, string className)
        //{
        //    string projectTo = "__Proxy__";
        //    string script_setRootObject = $"var {projectTo}={{}};";
        //    string script_importModule = Resources.JSProxy
        //        .Replace("{className}", className)
        //        .Replace("{moduleName}", moduleName)
        //        .Replace("{exportClass}", "proxy")
        //        .Replace("{exportValue}", projectTo);
        //    ModuleLocator locator = new ModuleLocator(rootFolder);
        //    context.RunScript(script_setRootObject);
        //    context.RunModule(script_importModule, locator.LoadModule);
        //    return context.GlobalObject.ReadProperty<JSValue>(projectTo);
        //}
    }
}
