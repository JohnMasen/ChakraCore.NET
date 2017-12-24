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

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ChakraCore.NET.Promise.PromiseRejectedException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public PromiseRejectedException(string message, Exception inner) 
            : base(message, inner) { }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ChakraCore.NET.API.PromiseRejectedException" /> class. 
        /// </summary>
        /// <param name="error">The JavaScript error object.</param>
        public PromiseRejectedException(JavaScriptValue error)
            : this(error, GetMessage(error)) { }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ChakraCore.NET.API.PromiseRejectedException" /> class. 
        /// </summary>
        /// <param name="error">The JavaScript error object.</param>
        /// <param name="message">The error message.</param>
        public PromiseRejectedException(JavaScriptValue error, string message) 
            : this(message)
        {
            Error = error;
        }

        /// <summary>
        /// Gets a JavaScript object representing the script error.
        /// </summary>
        public JavaScriptValue Error { get; }

        private static string GetMessage(JavaScriptValue error)
        {
            var messageProperty = JavaScriptPropertyId.FromString("message");
            var message = error.HasProperty(messageProperty)
                ? error.GetProperty(messageProperty).ConvertToString().ToString()
                : "Promise rejected.";
            return message;
        }
    }
}
