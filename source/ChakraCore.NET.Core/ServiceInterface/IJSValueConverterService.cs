
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public delegate JavaScriptValue toJSValueDelegate<T>(IServiceNode serviceNode, T value);
    public delegate TResult fromJSValueDelegate<out TResult>(IServiceNode serviceNode, JavaScriptValue value);
    public interface IJSValueConverterService:IService
    {
        void RegisterConverter<T>(toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue, bool throwIfExists = true);
        T FromJSValue<T>(JavaScriptValue value);
        JavaScriptValue ToJSValue<T>(T value);
        bool CanConvert<T>();
       
    }
}
