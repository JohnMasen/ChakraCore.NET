using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public abstract class JSClassWrapperBase:JSValueWrapperBase
    {
        public override void SetValue(JSValue value)
        {
            base.SetValue(value);
            RegisterCustomValueConverter(value.ServiceNode.GetService<IJSValueConverterService>());
        }
        protected void RegisterCustomValueConverter(IJSValueConverterService converterService)
        {

        }
    }
}
