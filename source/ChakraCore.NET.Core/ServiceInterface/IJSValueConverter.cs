using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public delegate JavaScriptValue toJSValueDelegate<T>(IServiceNode serviceNode, T value);
    public delegate TResult fromJSValueDelegate<out TResult>(IServiceNode serviceNode, JavaScriptValue value);
    interface IJSValueConverter:IService
    {
        void RegisterConverter<T>(toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue, bool throwIfExists = true);
        T FromJSValue<T>(JavaScriptValue value);
        JavaScriptValue ToJSValue<T>(T value);
        bool CanConvert<T>();
       
    }
}
