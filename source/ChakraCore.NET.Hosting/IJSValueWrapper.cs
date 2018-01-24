using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public interface IJSValueWrapper
    {
        void SetValue(JSValue value);
    }
}
