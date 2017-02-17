using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.IoC
{
    public abstract class ServiceNodeBase : IServiceNode
    {
        private IServiceNode parent;
        public IServiceNode Parent => parent;
        public ServiceNodeBase(IServiceNode parent)
        {
            this.parent = parent;
        }
        public abstract bool CanGet<T>();

        public abstract T Get<T>();

        public abstract void RegisterFactory<T>(Func<T> factory, ServiceFactoryCreateOption option, bool registerGlobal);
    }
}
