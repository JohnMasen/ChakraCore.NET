using System;
using ChakraCore.NET.API;

namespace ChakraCore.NET.Promise
{
    public class PromiseRejectedException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ChakraCore.NET.Promise.PromiseRejectedException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public PromiseRejectedException(string message) 
            : base(message) { }

    }
}
