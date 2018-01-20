using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    class PluginManager
    {
        private ChakraContext context;
        private const string API_OBJECT_NAME = "__API__";
        private JSValue apiContainer;
        private List<Func<string, IPluginInstaller>> Loaders;
        public PluginManager(ChakraContext context,LoadPluginInstallerFunction loadPluginInstallerCallback)
        {
            context.GlobalObject.Binding.SetFunction<string, JavaScriptValue>("RequireNative", (name)=>
            {
                var plugin = loadPluginInstallerCallback(name);
                var stub = createStubValue();
                plugin.Install(stub);
                return stub.ReferenceValue;
            });
            var jsvalue = context.ServiceNode.GetService<IJSValueService>().CreateObject();
            apiContainer = new JSValue(context.GlobalObject.ServiceNode, jsvalue);
            context.GlobalObject.WriteProperty(API_OBJECT_NAME, apiContainer);
        }
        
        

        private JSValue createStubValue()
        {
            JSValue stub = new JSValue(apiContainer.ServiceNode, JavaScriptValue.CreateObject());
            string id = "__NativeAPI__" + Guid.NewGuid().ToString().Replace('-', '_');
            apiContainer.WriteProperty(id, stub);
            return stub;
        }
    }
}
