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
        Lazy<ChakraContext> defaultContext;
        JavaScriptHostingConfig config;
        public static readonly JavaScriptHosting Default = new JavaScriptHosting(new JavaScriptHostingConfig());
        public JavaScriptHosting(JavaScriptHostingConfig config)
        {
            this.config = config;
            defaultContext = new Lazy<ChakraContext>(() => { return getContext(true); }, true);
        }
        protected virtual ChakraRuntime createRuntime()
        {
            return ChakraRuntime.Create();
        }

        protected virtual ChakraContext createContext(ChakraRuntime runtime)
        {
            return runtime.CreateContext(false);
        }

        private ChakraContext getContext(bool useNewRunTime)
        {
            if (useNewRunTime)
            {
                return createContext(createRuntime());
            }
            else
            {
                return defaultContext.Value;
            }
        }
        public virtual void RunScript(string script,bool useNewRuntime=true)
        {
            getContext(useNewRuntime).RunScript(script);
        }

        public JSValue GetModuleClass(string moduleName,string className,bool useNewRunTime=true)
        {
            var context = getContext(useNewRunTime);
            string projectTo = Guid.NewGuid().ToString().Replace('-', '_');
            return context.ProjectModuleClass(projectTo, moduleName, className, config.LoadModule);
        }

        public  Task<JSValue> GetModuleClassAsync(string moduleName, string className)
        {
            return Task.Factory.StartNew(()=>{ return GetModuleClass(moduleName, className, true); });
        }

        public TResult GetModuleClass<TResult>(string moduleName, string className, bool useNewRunTime = true) where TResult:IJSValueWrapper,new()
        {
            var jsvalue = GetModuleClass(moduleName, className, useNewRunTime);
            TResult result = new TResult();
            result.SetValue(jsvalue);
            return result;
        }

        public Task<TResult> GetModuleClassAsync<TResult>(string moduleName, string className) where TResult : IJSValueWrapper, new()
        {
            return Task.Factory.StartNew(() => { return GetModuleClass<TResult>(moduleName, className, true); });
        }

    }
}
