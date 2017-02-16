using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public abstract class LoggableObjectBase<T> where T:LoggableObjectBase<T>
    {

        protected ILogger log;
        public LoggableObjectBase()
        {
            log = ChakraLogging.CreateLogger<T>();
        }

    }
}
