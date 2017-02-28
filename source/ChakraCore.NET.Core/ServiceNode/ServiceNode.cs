using System;
using System.Collections.Generic;
using System.Text;
using static ChakraCore.NET.Core.TypeComparer;
namespace ChakraCore.NET.Core
{
    public class ServiceNode : IServiceNode
    {
        public IServiceNode Parent { get; private set; }
        public string Name { get; private set; }

        private SortedDictionary<Type, object> providerList;

        
        public ServiceNode(IServiceNode parent, string name)
        {
            Parent = parent;
            Name = name;
            providerList = new SortedDictionary<Type, object>(TypeComparer.Instance);
        }

        public virtual bool CanGetService<T>() where T:IService
        {
            return providerList.ContainsKey(typeof(T));
        }

        public virtual bool CanGetServiceFromChain<T>() where T:IService
        {
            return CanGetService<T>() && (Parent?.CanGetService<T>() ?? false);
        }

        public virtual IServiceNode Chain(string name)
        {
            return new ServiceNode(this, name);
        }

        public virtual TResult GetService<TResult>() where TResult : IService
        {
            return GetService<TResult>(this);
        }

        public virtual TResult GetService<TResult>(IServiceNode currentNode) where TResult : IService
        {
            if (tryGetService<TResult>(out TResult result))
            {
                result.CurrentNode = currentNode;
                return result;
            }
            else
            {
                if (Parent != null)
                {
                    var r = Parent.GetService<TResult>(currentNode);
                    PushService<TResult>(r);//cache the service instace
                    r.CurrentNode = currentNode;
                    return r;
                }
                throw new ServiceNotRegisteredException<TResult>();
            }
        }


        public virtual void PushService<T>(T service) where T : IService
        {
            Stack<T> item;
            if (CanGetService<T>())
            {
                item = providerList[typeof(T)] as Stack<T>;
                if (item==null)
                {
                    throw new InvalidOperationException("internal service stack corrupted");
                }
            }
            else
            {
                item = new Stack<T>();
            }
            item.Push(service);
        }

        protected bool tryGetServiceStack<T>(out Stack<T> result) where T : IService
        {
            if (CanGetService<T>())
            {
                result = providerList[typeof(T)] as Stack<T>;
                if (result==null || result.Count==0)
                {
                    throw new InvalidOperationException("internal service stack corrupted");
                }
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        protected bool tryGetService<T>(out T result) where T:IService
        {
            if(tryGetServiceStack<T>(out Stack<T> stack))
            {
                result = stack.Peek();
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        public virtual void PopService<T>() where T : IService
        {
            if (tryGetServiceStack<T>(out Stack<T> result))
            {
                result.Pop();
            }
            else
            {
                throw new ServiceNotRegisteredException<T>();
            }
        }


        public static IServiceNode CreateRoot()
        {
            return new ServiceNode(null, "Root");
        }

        public virtual void Detach()
        {
            Parent = null;
        }

       
    }
}
