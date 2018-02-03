using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    class SDKProviderNotFoundException:HostingException
    {
        public SDKProviderNotFoundException(string sdkName):base($"cannot found SDK provider {sdkName}")
        {

        }
    }
}
