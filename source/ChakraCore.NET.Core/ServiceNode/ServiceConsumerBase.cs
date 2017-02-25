using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public abstract class ServiceConsumerBase
    {
        protected IServiceNode serviceNode;
        public ServiceConsumerBase(IServiceNode serviceNode,string consumerName)
        {
            this.serviceNode = serviceNode.Chain(consumerName);
        }
    }
}
