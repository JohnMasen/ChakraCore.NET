using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Debug
{
    public struct SourceCode
    {
        [JsonProperty(propertyName: "fileName")]
        public string FileName;
        [JsonProperty(propertyName: "lineCount")]
        public int LineCount;
        [JsonProperty(propertyName: "sourceLength")]
        public int SourceLength;
        [JsonProperty(propertyName: "scriptId")]
        public uint ScriptId;
        [JsonProperty(propertyName: "source")]
        public string Source;

        public override string ToString()
        {
            return $"Filename={FileName},LineCount={LineCount},SourceLength={SourceLength},ScriptId={ScriptId}";
        }
    }
}
