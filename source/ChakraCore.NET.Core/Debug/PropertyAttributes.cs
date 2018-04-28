using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Debug
{
    [Flags]
    public enum PropertyAttributesEnum
    {
        NONE = 0x1,
        HAVE_CHILDRENS = 0x2,
        READ_ONLY_VALUE = 0x4,
        IN_TDZ = 0x8,
    }
}
