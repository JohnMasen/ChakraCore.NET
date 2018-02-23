using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Debug
{
    public struct StackProperties
    {
        [JsonProperty(propertyName: "thisObject")]
        public Variable ThisObject;
        [JsonProperty(propertyName: "arguments")]
        public Variable Arguments;
        [JsonProperty(propertyName: "locals")]
        public Variable[] Locals;
        [JsonProperty(propertyName: "globals")]
        public Variable Global;
    }
}
