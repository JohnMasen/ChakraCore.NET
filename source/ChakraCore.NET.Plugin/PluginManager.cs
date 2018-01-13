using ChakraCore.NET.API;
using System;
using System.Collections.Generic;

namespace ChakraCore.NET.Plugin
{
    public class PluginManager
    {
        private ChakraContext context;
        private const string API_OBJECT_NAME = "__API__";
        private JSValue apiContainer;
        public PluginManager(ChakraContext context,IEnumerable<IPluginLoader> loaders=null)
        {
            this.context = context;
            if (loaders!=null)
            {
                Loaders = new List<IPluginLoader>(loaders);
            }
            else
            {
                Loaders = new List<IPluginLoader>();
            }
        }
        public List<IPluginLoader> Loaders { get; private set; } 
        public void Init(ChakraContext context)
        {
            context.GlobalObject.Binding.SetFunction<string, JavaScriptValue>("RequireNative", installPlugin);
            var jsvalue=context.ServiceNode.GetService<IJSValueService>().CreateObject();
            apiContainer = new JSValue(context.GlobalObject.ServiceNode, jsvalue);
            context.GlobalObject.WriteProperty(API_OBJECT_NAME, apiContainer);
        }

        private JavaScriptValue installPlugin(string name)
        {
            foreach (var item in Loaders)
            {
                var plugin = item.Load(name);
                if (plugin!=null)
                {
                    var stub = createStubValue();
                    plugin.Install(stub);
                    return stub.ReferenceValue;
                }
            }
            throw new InvalidOperationException($"Plugin install failed, none of loaders can load [{name}]");
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
