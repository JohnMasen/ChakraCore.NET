using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class UnknownModuleResolveProtocolException:HostingException
    {
        public string ProtocolName { get; set; }

        public UnknownModuleResolveProtocolException(string protocolName):base($"Module resolve protocol [{protocolName}] not registered")
        {
            ProtocolName = protocolName;
        }
    }
}
