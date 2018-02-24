using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Debug
{
    public struct Variable
    {
        //"name":"this","type":"object","className":"Object","display":"{...}","propertyAttributes":3,"handle":2,"value":123
        [JsonProperty(propertyName: "name")]
        public string Name;
        [JsonProperty(propertyName: "type")]
        public string Type;
        [JsonProperty(propertyName: "className")]
        public string ClassName;
        [JsonProperty(propertyName: "display")]
        public string Display;
        [JsonProperty(propertyName: "propertyAttributes")]
        public PropertyAttributesEnum PropertyAttributes;
        [JsonProperty(propertyName: "handle")]
        public uint Handle;
        [JsonProperty(propertyName: "value")]
        public string Value;
    }
}
