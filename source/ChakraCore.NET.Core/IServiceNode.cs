using System;

namespace ChakraCore.NET.Core
{
    public delegate T ServiceProviderDelegate<out T>(IServiceNode service);
    public interface IServiceNode
    {
        IServiceNode Parent { get;}
        string Name { get; }
        TResult GetService<TResult>();
        bool CanGetService<T>();
        IServiceNode ChainService<T>(ServiceProviderDelegate<T> provider,string name);
    }
}
