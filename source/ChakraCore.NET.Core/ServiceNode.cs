using System;
using System.Collections.Generic;
using System.Text;
using static ChakraCore.NET.Core.TypeComparer;
namespace ChakraCore.NET.Core
{
    public class ServiceNode<TService> : IServiceNode
    {
        public IServiceNode Parent { get; private set; }
        public string Name { get; private set; }
        private ServiceProviderDelegate<TService> provider;
        public ServiceNode(IServiceNode parent, string name, ServiceProviderDelegate<TService> provider)
        {
            Parent = parent;
            Name = name;
            this.provider = provider;
        }

        public virtual bool CanGetService<T>()
        {
            return AreSameGenericType<T, TService>();
        }

        public bool CanGetServiceFromChain<T>()
        {
            return CanGetService<T>() && (Parent?.CanGetService<T>() ?? false);
        }

        public virtual IServiceNode ChainService<T>(ServiceProviderDelegate<T> provider, string name)
        {
            return new ServiceNode<T>(this, name, provider);
        }

        public virtual TResult GetService<TResult>()
        {
            if (CanGetService<TResult>())
            {
                return (provider as ServiceProviderDelegate<TResult>)(this);
            }
            else
            {
                if (Parent != null)
                {
                    return Parent.GetService<TResult>();
                }
                throw new ServiceNotRegisteredException<TResult>();
            }
        }

        public static IServiceNode CreateRoot()
        {
            return new ServiceNode<RootNode>(null, "Root", null);
        }

        private class RootNode { }
    }
}
