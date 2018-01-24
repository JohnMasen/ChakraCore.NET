using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class MissingProtocolProcessorException:HostingException
    {
        public string Protocol { get; private set; }
        public MissingProtocolProcessorException(string protocolName):base($"Cannot find processor for protocol {protocolName}")
        {
            Protocol = protocolName;
        }
    }
}
