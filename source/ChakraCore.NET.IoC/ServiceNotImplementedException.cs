using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.IoC
{

    public class ServiceNotImplementedException<T> : Exception
    {
        public ServiceNotImplementedException() : base($"service {typeof(T).ToString()} not implemented") { }

    }
}
