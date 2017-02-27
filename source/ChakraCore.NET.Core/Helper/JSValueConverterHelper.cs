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
    }
}
