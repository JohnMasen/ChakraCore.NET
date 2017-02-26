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
            serviceNode.PushService<INativeFunctionHolder>(new NativeFunctionHolderService(false));
            //init binding here

            //chain the function holder to one time for ad-hoc call
            serviceNode = serviceNode.Chain("JSValue onetime node");
            serviceNode.PushService<INativeFunctionHolder>(new NativeFunctionHolderService(true));
        }
    }
}
