
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public partial class JSValue : ServiceConsumerBase
    {
        protected IJSValueConverterService converter => ServiceNode.GetService<IJSValueConverterService>();
        protected IJSValueService valueService=> ServiceNode.GetService<IJSValueService>();

        public JSValueBinding Binding { get; private set; }
        public JavaScriptValue ReferenceValue { get; private set; }

        public JSValue(IServiceNode parentNode,JavaScriptValue value) : base(parentNode, "JSValue")
        {
            ReferenceValue = value;
            ServiceNode.PushService<ICallContextService>(new CallContextService(value));
            //inject service
            Binding = new JSValueBinding(ServiceNode,value);//binding will create a branch of current service node to persistent hold all delegates created by binding function
            
        }

        public T ReadProperty<T>(JavaScriptPropertyId id)
        {
            return valueService.ReadProperty<T>(ReferenceValue, id);
        }

        public void WriteProperty<T>(JavaScriptPropertyId id, T value)
        {
            valueService.WriteProperty(ReferenceValue, id, value);
        }

        public T ReadProperty<T>(string id)
        {
            return valueService.ReadProperty<T>(ReferenceValue, id);
        }

        public void WriteProperty<T>(string id, T value)
        {
            valueService.WriteProperty(ReferenceValue, id, value);
        }
        
    }
}
