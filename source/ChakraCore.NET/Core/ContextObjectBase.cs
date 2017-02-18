using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public abstract class ContextObjectBase
    {
        public ChakraContext RuntimeContext { get; private set; }
        public ContextObjectBase(ChakraContext context)
        {
            RuntimeContext = context;
        }


    }
}
