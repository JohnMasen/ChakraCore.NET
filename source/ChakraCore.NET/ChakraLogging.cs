using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    
    public static class ChakraLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; } = new LoggerFactory();
        public static ILogger CreateLogger<T>() =>
        LoggerFactory.CreateLogger<T>();


        #region EventID Consts

        public static class EventIds
        {
            public const int CoreEvent = 0;
        }
        
        #endregion
    }
}
