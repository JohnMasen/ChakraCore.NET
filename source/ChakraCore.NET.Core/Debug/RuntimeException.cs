using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Debug
{
    public struct RuntimeException
    {
        [JsonProperty(propertyName: "scriptId")]
        public uint ScriptId;
        [JsonProperty(propertyName: "line")]
        public uint Line;
        [JsonProperty(propertyName: "column")]
        public uint Column;
        [JsonProperty(propertyName: "sourceLength")]
        public int SourceLength;
        [JsonProperty(propertyName: "sourceText")]
        public string SourceText;
        [JsonProperty(propertyName: "uncaught")]
        public bool Uncaught;
        [JsonProperty(propertyName: "exception")]
        public Variable ExceptionObject;

        public override string ToString()
        {
            return $"ScriptId={ScriptId},Line={Line},Column={Column},SourceLength={SourceLength},SourceText={SourceText},Uncaught={Uncaught},Exception={ExceptionObject}";
        }
    }
    
}
