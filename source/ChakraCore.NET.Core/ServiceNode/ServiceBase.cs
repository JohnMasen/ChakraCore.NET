using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public abstract class ServiceBase:IService
    {
        protected IServiceNode serviceNode;
        public void SetupNode(IServiceNode service)
        {
            serviceNode = service;
        }
    }
}
