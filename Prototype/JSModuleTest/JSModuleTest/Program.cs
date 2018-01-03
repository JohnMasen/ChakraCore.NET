using System;
using ChakraCore.NET;
namespace JSModuleTest
{
    class Program
    {
        static ChakraContext context;
        static ModuleLoader loader = new ModuleLoader();
        static void Main(string[] args)
        {
            var runtime = ChakraRuntime.Create();
            context = runtime.CreateContext(false);
            context.ServiceNode.GetService<IJSValueConverterService>().RegisterProxyConverter<ModuleLoader>(
                (value, obj, node) =>
                {
                    value.SetFunction<string, ChakraCore.NET.API.JavaScriptValue>("Import", obj.LoadModule);
                    value.SetMethod<string>("Echo", obj.Echo);
                }

                );
            
            context.GlobalObject.WriteProperty<ModuleLoader>("__Sys", loader);
            string script = System.IO.File.ReadAllText("ModuleShell.js");
            //context.ServiceNode.GetService<IContextSwitchService>().With(() => 
            //{
            //    loader.InitModuleCallback();
            //});

            //context.RunScript(script);
            //context.GlobalObject.CallFunction<string>("RunModule");

            //context.ServiceNode.GetService<IContextSwitchService>().With(
            //    () => { loader.LoadModule("Module1.js"); }
            //    );
            //context.GlobalObject.CallMethod<string>("Main","call from c# ok");
            projectModuleClass("_output", "Module1.js", "abc");
            JSValue x = context.GlobalObject.ReadProperty<JSValue>("_output");
            x.CallMethod<string>("main", "call from projected ok");
            Console.Write("Press Enter to exit");
            Console.ReadLine();
        }
        static void projectModuleClass(string projectTo,string moduleName,string className)
        {
            string script_setRootObject = $"var {projectTo}={{}}";
            string script_importModule = $"import {{{className}}} from '{moduleName}'; {projectTo}=new {className}();";
            context.RunScript(script_setRootObject);
            System.IO.File.WriteAllText("__M.js", script_importModule);
            context.ServiceNode.GetService<IContextSwitchService>().With(
                () => { loader.LoadModule("__M.js"); }
                );
        }
    }
}
