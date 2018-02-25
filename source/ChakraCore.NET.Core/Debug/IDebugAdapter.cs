using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET.Debug
{
    public interface IDebugAdapter
    {
        void Init(IRuntimeDebuggingService debuggingService);
    }
}
