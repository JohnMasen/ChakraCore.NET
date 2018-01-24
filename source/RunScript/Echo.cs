using ChakraCore.NET.Plugin.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RunScript
{
    class Echo : IEchoFunctionProvider
    {
        void IEchoFunctionProvider.Echo(string message)
        {
            Console.WriteLine(message);
        }
    }
}
