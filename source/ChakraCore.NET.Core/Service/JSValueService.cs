using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Core.API;

namespace ChakraCore.NET.Core
{
    public class JSValueService : ServiceBase, IJSValueService
    {
        public T ReadProperty<T>(JavaScriptValue target, JavaScriptPropertyId id)
        {
            var convert = serviceNode.GetService<IJSValueConverter>();
            return serviceNode.GetService<IContextSwitchService>().With<T>(
                () =>
                {
                    return convert.FromJSValue<T>( target.GetProperty(id));
                });

                ;
        }

        public void WriteProperty<T>(JavaScriptValue target, JavaScriptPropertyId id, T value)
        {
            var convert = serviceNode.GetService<IJSValueConverter>();
            serviceNode.GetService<IContextSwitchService>().With(
                () =>
                {
                    target.SetProperty(id, convert.ToJSValue<T>(value),true);
                });

            ;
        }
    }
}
