using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Debug
{
    public struct VariableProperties
    {
        [JsonProperty(propertyName: "totalPropertiesOfObject")]
        public int StackProperties;
        [JsonProperty(propertyName: "properties")]
        public Variable[] Properties;
        [JsonProperty(propertyName: "debuggerOnlyProperties")]
        public Variable[] DebuggerOnlyProperties;
    }
}
