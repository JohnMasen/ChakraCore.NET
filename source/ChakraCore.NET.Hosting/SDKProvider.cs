using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class SDKProvider : ISDKProvider
    {
        private string sdk;
        public SDKProvider(string sdkScript)
        {
            sdk = sdkScript;
        }
        public string GetSDK() => sdk;
        
    }
}
