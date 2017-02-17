using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.IoC
{
    public partial class ServiceCallContext:ServiceNodeBase
    {
        public readonly Dictionary<object, object> externalInfo = new Dictionary<object, object>();
        public readonly Stack<Type> callStack = new Stack<Type>();
        
        public ServiceCallContext(IServiceNode parent) : base(parent)
        {
        }

        public override bool CanGet<T>()
        {
            return Parent.CanGet<T>();//not going to support anything, let parent create object
        }

        public override T Get<T>()
        {
            callStack.Push(typeof(T));//prevent nested reference
            var result= Parent.Get<T>();
            callStack.Pop();
            return result;
        }
        
        public override void RegisterFactory<T>(Func<T> factory, ServiceFactoryCreateOption option, bool registerGlobal)
        {
            Parent.RegisterFactory(factory, option, registerGlobal);//context does not support register facotyr, route to parent
        }
    }
}
