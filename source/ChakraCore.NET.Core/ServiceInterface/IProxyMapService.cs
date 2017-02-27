using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public interface IProxyMapService:IService
    {
        JavaScriptValue Map<T>(T obj, Action<JSValueBinding> createBinding) where T : class;

        T Get<T>(JavaScriptValue value) where T : class;

        void Release<T>(T obj) where T : class;
        
        void ReleaseAll();
    }
}
