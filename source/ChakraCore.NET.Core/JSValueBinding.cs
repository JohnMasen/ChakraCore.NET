
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public partial class JSValueBinding : ServiceConsumerBase
    {
        IJSValueConverterService converter => ServiceNode.GetService<IJSValueConverterService>();
        IJSValueService valueService => ServiceNode.GetService<IJSValueService>();
        readonly JavaScriptValue jsValue;
        public JSValueBinding(IServiceNode parentNode, JavaScriptValue value) : base(parentNode, "JSValueBinding")
        {
            jsValue = value;
        }


    }
}
