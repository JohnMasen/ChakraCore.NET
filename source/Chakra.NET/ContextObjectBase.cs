using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chakra.NET
{
    public abstract class ContextObjectBase<T>:LoggableObjectBase<T> where T: LoggableObjectBase<T>
    {
        public ChakraContext RuntimeContext { get; private set; }
        public ContextObjectBase(ChakraContext context)
        {
            RuntimeContext = context;
        }


    }
}
