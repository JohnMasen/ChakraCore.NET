using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public interface IJSValueService:IService
    {
        T ReadProperty<T>(JavaScriptValue target, JavaScriptPropertyId id);
        void WriteProperty<T>(JavaScriptValue target,JavaScriptPropertyId id,T value);

        T ReadProperty<T>(JavaScriptValue target, string id);
        void WriteProperty<T>(JavaScriptValue target, string id, T value);

    }
}
