using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace ChakraCore.NET.Debug
{
    public struct BreakPoint
    {
        [JsonProperty(propertyName: "breakpointId")]
        public uint BreakpointId;
        [JsonProperty(propertyName: "scriptId")]
        public uint ScriptId;
        [JsonProperty(propertyName: "line")]
        public uint Line;
        [JsonProperty(propertyName: "column")]
        public uint Column;
        [JsonProperty(propertyName: "sourceLength")]
        public uint SourceLength;
        [JsonProperty(propertyName: "sourceText")]
        public string SourceText;

        public override string ToString()
        {
            return $"BreakpointId={BreakpointId},ScriptId={ScriptId},Line={Line},Column={Column},SourceLength={SourceLength},SourceText={SourceText}";
        }
    }
}
