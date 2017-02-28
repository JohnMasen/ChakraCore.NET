
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public partial class JSValueBinding : ServiceConsumerBase
    {
        IJSValueConverterService converter;
        IJSValueService valueService;
        readonly JavaScriptValue jsValue;
        public JSValueBinding(IServiceNode parentNode,JavaScriptValue value) : base(parentNode, "JSValueBinding")
        {
            jsValue = value;
            ServiceNode.PushService<INativeFunctionHolderService>(new NativeFunctionHolderService(false));
            converter = ServiceNode.GetService<IJSValueConverterService>();
            valueService = ServiceNode.GetService<IJSValueService>();
        }


    }
}
