using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{

    public class HostingException : Exception
    {
        public HostingException() { }
        public HostingException(string message) : base(message) { }
        public HostingException(string message, Exception inner) : base(message, inner) { }
    }
}
