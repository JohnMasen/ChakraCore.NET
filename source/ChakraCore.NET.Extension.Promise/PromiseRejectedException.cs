using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Extension.Promise
{
    


    public class PromiseRejectedException : Exception
    {
        public PromiseRejectedException(string message) : base(message) { }
        public PromiseRejectedException(string message, Exception inner) : base(message, inner) { }
    }
}
