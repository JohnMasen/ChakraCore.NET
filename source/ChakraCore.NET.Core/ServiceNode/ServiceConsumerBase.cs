using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public abstract class ServiceConsumerBase
    {
        public IServiceNode ServiceNode { get; protected set; }
        public ServiceConsumerBase(IServiceNode parentNode,string consumerName)
        {
            this.ServiceNode = parentNode.Chain(consumerName);
        }
    }
}
