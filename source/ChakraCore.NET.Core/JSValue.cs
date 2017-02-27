using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public class JSValue : ServiceConsumerBase
    {
        public JavaScriptValue ReferenceValue { get; private set; }
        public JSValue(IServiceNode parentNode,JavaScriptValue value) : base(parentNode, "JSValue")
        {
            ReferenceValue = value;
            //inject service
            ServiceNode.PushService<INativeFunctionHolderService>(new NativeFunctionHolderService(false));
            //init binding here

            //chain the function holder to one time for ad-hoc call
            ServiceNode = ServiceNode.Chain("JSValue onetime node");
            ServiceNode.PushService<INativeFunctionHolderService>(new NativeFunctionHolderService(true));
        }
    }
}
