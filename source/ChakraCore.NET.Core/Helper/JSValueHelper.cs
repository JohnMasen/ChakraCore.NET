using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public static class JSValueHelper
    {
        public static T ReadProperty<T>(this IJSValueService service,JavaScriptValue target, JavaScriptPropertyId id,T defaultValue)
        {
            if (service.HasProperty(target,id))
            {
                return service.ReadProperty<T>(target, id);
            }
            else
            {
                return defaultValue;
            }
        }

        public static T ReadProperty<T>(this IJSValueService service,JavaScriptValue target,string id,T defaultValue)
        {
            if (service.HasProperty(target, id))
            {
                return service.ReadProperty<T>(target, id);
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
