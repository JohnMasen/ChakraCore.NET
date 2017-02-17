using System;
using System.Collections.Generic;

namespace ChakraCore.NET.IoC
{
    public class ServiceNode:ServiceNodeBase
    {
        private SortedDictionary<Type, Object> factoryCollection = new SortedDictionary<Type, object>();

        public ServiceNode(IServiceNode parent) : base(parent)
        {
        }

        public override  bool CanGet<T>()
        {
            return factoryCollection.ContainsKey(typeof(T));
        }

        public override T Get<T>()
        {
            if (!CanGet<T>())
            {
                if (Parent!=null)
                {
                    return Parent.Get<T>();
                }
                throw new ServiceNotImplementedException<T>();
            }
            Func<T> factory = factoryCollection[typeof(T)] as Func<T>;
            if (factory == null)
            {
                throw new InvalidOperationException("serviceFactory corrupted");
            }
            return factory();
        }


        public override void RegisterFactory<T>(Func<T> factory, ServiceFactoryCreateOption option, bool registerGlobal)
        {
            if (CanGet<T>())
            {
                switch (option)
                {
                    case ServiceFactoryCreateOption.ThrowIfExists:
                        throw new InvalidOperationException($"factory of type {typeof(T)} already exists");
                    case ServiceFactoryCreateOption.OverrideIfExists:
                        factoryCollection.Remove(typeof(T));
                        break;
                    case ServiceFactoryCreateOption.IgnoreIfExists:
                    default:
                        return;
                }
            }
            factoryCollection.Add(typeof(T), factory);
        }

    }
}
