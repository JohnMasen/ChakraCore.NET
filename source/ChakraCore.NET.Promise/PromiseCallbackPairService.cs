using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Promise
{
    public class PromiseCallbackPairService:ServiceBase
    {
        PromiseNativeFunctionHolderService service = new PromiseNativeFunctionHolderService();
        public void Begin()
        {
            serviceNode.PushService<INativeFunctionHolderService>(service);//temporary enable our sepcial delegate holder
            service.StartSession();
        }

        public void End()
        {
            service.StopSession();
            serviceNode.PopService<INativeFunctionHolderService>();//restore previous service
        }
        
    }
}
