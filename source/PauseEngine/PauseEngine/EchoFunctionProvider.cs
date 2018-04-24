using System;
using System.Collections.Generic;
using System.Text;

namespace PauseEngine
{
    class EchoFunctionProvider : ChakraCore.NET.Plugin.Common.IEchoFunctionProvider
    {
        public void Echo(string message)
        {
            Console.WriteLine(message);
        }
    }
}
