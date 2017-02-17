using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.IoC
{
    public static partial class DelegateHelper
    {
        public static void RegisterMethodService<T>(this IServiceNode source,Action<IServiceNode,T> a,ServiceFactoryCreateOption option= ServiceFactoryCreateOption.IgnoreIfExists, bool registerGlobal=true)
        {
            source.RegisterFactory<Action<IServiceNode,T>>(()=>{ return a; }, option, registerGlobal);
        }

        public static void RegisterFunctionService<T, TResult>(this IServiceNode source, Func<IServiceNode, T, TResult> f, ServiceFactoryCreateOption option = ServiceFactoryCreateOption.IgnoreIfExists, bool registerGlobal = true)
        {
            source.RegisterFactory<Func<IServiceNode, T, TResult>>(() => { return f; }, option, registerGlobal);
        }

        public static void CallMethod<T>(this IServiceNode source, T para1)
        {
            source.Get<Action<IServiceNode, T>>()(source,para1);
        }

        public static TResult CallFunction<T,TResult>(this IServiceNode source, T para1)
        {
            return source.Get<Func<IServiceNode, T,TResult>>()(source,para1);
        }
    }
}
