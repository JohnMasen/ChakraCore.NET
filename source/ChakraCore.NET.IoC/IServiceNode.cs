using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.IoC
{
    public interface IServiceNode
    {
        IServiceNode Parent { get; }
        T Get<T>();
        bool CanGet<T>();
        void RegisterFactory<T>(Func<T> factory, ServiceFactoryCreateOption option, bool registerGlobal);
    }
}
