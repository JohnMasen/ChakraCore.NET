using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.IoC
{
    public enum ServiceFactoryCreateOption
    {
        ThrowIfExists,
        OverrideIfExists,
        IgnoreIfExists
    }
}
