using System;

namespace ChakraCore.NET
{
    public interface IServiceNode
    {
        IServiceNode Parent { get;}
        string Name { get; }
        TResult GetService<TResult>() where TResult : IService;

        TResult GetService<TResult>(IServiceNode currentNode) where TResult : IService;
        bool CanGetService<T>() where T : IService;

        void PushService<T>(T service) where T : IService;
        void PopService<T>() where T : IService;
        IServiceNode Chain(string name);
        void Detach();
    }
}
