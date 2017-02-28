
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public interface IProxyMapService:IService,IDisposable
    {
        JavaScriptValue Map<T>(T obj, Action<JSValueBinding,T,IServiceNode> createBinding) where T : class;

        T Get<T>(JavaScriptValue value) where T : class;

        void Release<T>(T obj) where T : class;

        void Release<T>(JavaScriptValue value) where T : class;

        void ReleaseAll();
    }
}
