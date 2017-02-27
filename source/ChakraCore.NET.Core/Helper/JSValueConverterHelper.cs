using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public static partial class JSValueConverterHelper
    {
        public static void RegisterStructConverter<T>(this IJSValueConverterService service, toJSValueDelegate<T> toJSValue, fromJSValueDelegate<T> fromJSValue) where T : struct
        {
            service.RegisterConverter<T>(toJSValue, fromJSValue);
        }

        public static void RegisterProxyConverter<T>(this IJSValueConverterService service,Action<JSValueBinding> createBinding) where T : class
        {
            
            toJSValueDelegate<T> tojs = (IServiceNode node, T value) =>
            {
                var mapService = node.GetService<IProxyMapService>();
                return mapService.Map<T>(value,createBinding);
                
            };
            fromJSValueDelegate<T> fromjs = (IServiceNode node, JavaScriptValue value) =>
            {
                var mapService = node.GetService<IProxyMapService>();
                return mapService.Get<T>(value);
            };
            service.RegisterConverter<T>(tojs, fromjs);
        }
    }
}
