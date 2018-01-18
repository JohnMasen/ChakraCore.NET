using ChakraCore.NET;
using ChakraCore.NET.Plugin;
using System;
using System.Threading.Tasks;

namespace RuntimeHosting
{
    public class HostingInstaller : INativePluginInstaller
    {
        private string rootFolder;
        private Action<ChakraContext> initContext;
        private const string proxyScript = "import{{className}}from'{moduleName}';class{exportClass}extends{className}{__Dispatch__(name,para){let result;let args=JSON.parse(para);result=this[name].apply(this,args);if(result){return JSON.stringify(result)}else{return 'null' }}}{exportValue}=new{exportClass}();";
        internal HostingInstaller(string rootFolder, Action<ChakraContext> initContext)
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

        public JSValue projectProxyClass(ChakraContext context, string moduleName, string className)
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
}
