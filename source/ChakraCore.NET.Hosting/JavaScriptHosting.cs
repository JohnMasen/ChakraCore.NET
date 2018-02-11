using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET.Hosting
{
    public delegate string LoadModuleFunction(string name);
    public delegate IPluginInstaller LoadPluginInstallerFunction(string name);

    public class JavaScriptHosting
    {
        public static readonly JavaScriptHosting Default = new JavaScriptHosting();
        protected virtual ChakraRuntime createRuntime()
        {
            return ChakraRuntime.Create();
        }

        protected virtual ChakraContext createContext(ChakraRuntime runtime)
        {
            return runtime.CreateContext(false);
        }

        protected virtual void initContext(ChakraContext context,JavaScriptHostingConfig config)
        {
            PluginManager pluginManager = new PluginManager(context, config.LoadPlugin);
            if (config.DebugAdapter!=null)
            {
                context.ServiceNode.GetService<IRuntimeDebuggingService>().AttachAdapter(config.DebugAdapter);
            }
        }


        public ChakraContext CreateContext(JavaScriptHostingConfig config)
        {
            var result = createContext(createRuntime());
            initContext(result, config);
            return result;
        }

        public virtual void RunScript(string script,JavaScriptHostingConfig config)
        {
            CreateContext(config).RunScript(script);
        }

        public JSValue GetModuleClass(string moduleName,string className, JavaScriptHostingConfig config)
        {
            var context = CreateContext(config);
            return context.ProjectModuleClass(moduleName, className, config.LoadModule);
        }

        public JSValue GetModuleClass(string proxyTemplate, string moduleName, string className, JavaScriptHostingConfig config)
        {
            var context = CreateContext(config);
            string projectTo = "X"+Guid.NewGuid().ToString().Replace('-', '_');
            return context.ProjectModuleClass(proxyTemplate, moduleName, className, config.LoadModule);
        }

        public  Task<JSValue> GetModuleClassAsync(string moduleName, string className, JavaScriptHostingConfig config)
        {
            return Task.Factory.StartNew(()=>{ return GetModuleClass(moduleName, className, config); });
        }

        public Task<JSValue> GetModuleClassAsync(string proxyTemplate, string moduleName, string className, JavaScriptHostingConfig config)
        {
            return Task.Factory.StartNew(() => { return GetModuleClass(proxyTemplate, moduleName, className, config); });
        }

        public TResult GetModuleClass<TResult>(string moduleName, string className, JavaScriptHostingConfig config) where TResult:IJSValueWrapper,new()
        {
            var jsvalue = GetModuleClass(moduleName, className, config);
            TResult result = new TResult();
            result.SetValue(jsvalue);
            return result;
        }

        public TResult GetModuleClass<TResult>(string proxyTemplate, string moduleName, string className, JavaScriptHostingConfig config) where TResult : IJSValueWrapper, new()
        {
            var jsvalue = GetModuleClass(proxyTemplate, moduleName, className, config);
            TResult result = new TResult();
            result.SetValue(jsvalue);
            return result;
        }

        public Task<TResult> GetModuleClassAsync<TResult>(string moduleName, string className, JavaScriptHostingConfig config) where TResult : IJSValueWrapper, new()
        {
            return Task.Factory.StartNew(() => { return GetModuleClass<TResult>(moduleName, className, config); });
        }
        public Task<TResult> GetModuleClassAsync<TResult>(string proxyTemplate, string moduleName, string className, JavaScriptHostingConfig config) where TResult : IJSValueWrapper, new()
        {
            return Task.Factory.StartNew(() => { return GetModuleClass<TResult>(proxyTemplate,moduleName, className, config); });
        }

    }
}
