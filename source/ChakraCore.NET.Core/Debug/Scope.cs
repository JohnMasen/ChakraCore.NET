using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Debug
{
    public struct Scope
    {
        [JsonProperty(propertyName: "index")]
        public int Index;
        [JsonProperty(propertyName: "handle")]
        public uint Handle;
    }
}
