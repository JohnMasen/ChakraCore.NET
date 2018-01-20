using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public abstract class JSValueWrapperBase : IJSValueWrapper
    {
        protected JSValue Reference;
        public virtual void SetValue(JSValue value)
        {
            Reference = value;
        }
    }
}
