using System;
using System.Collections.Generic;
using System.Text;

namespace Chakra.NET
{
    public abstract class ContextObjectBase
    {
        public ChakraContext Context { get; private set; }
        public ContextObjectBase(ChakraContext context)
        {
            Context = context;
        }


    }
}
