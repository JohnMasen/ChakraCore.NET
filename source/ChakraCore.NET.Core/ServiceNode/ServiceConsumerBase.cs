using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public abstract class ServiceConsumerBase
    {
        protected IServiceNode service;
        public ServiceConsumerBase(IServiceNode service,string consumerName)
        {
            this.service = service.Chain(consumerName);
        }
    }
}
