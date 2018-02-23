using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Debug
{
    public struct StackTrace
    {
        //[{"index":0,"scriptId":3,"line":4,"column":8,"sourceLength":13,"sourceText":"let c = a + b","functionHandle":1}]
        [JsonProperty(propertyName: "index")]
        public int Index;
        [JsonProperty(propertyName: "scriptId")]
        public uint ScriptId;
        [JsonProperty(propertyName: "line")]
        public int Line;
        [JsonProperty(propertyName: "column")]
        public int Column;
        [JsonProperty(propertyName: "sourceLength")]
        public int SourceLength;
        [JsonProperty(propertyName: "sourceText")]
        public string SourceText;
        [JsonProperty(propertyName: "functionHandle")]
        public uint FunctionHandle;
    }
}
