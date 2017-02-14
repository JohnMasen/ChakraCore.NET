using Chakra.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chakra.NET
{
    public class JSObjectMap
    {
        //TODO: it's a map!


        public JavaScriptValue JavaScriptValue { get; set; }

        public object CLRValue { get; set; }

        public List<JavaScriptNativeFunction> CallBackFunctions { get; set; }
        public struct FunctionBinding
        {
            public JavaScriptValue JavaScriptValue;
        }

        public enum FunctionBindingType
        {
            FromJS,
            ToJS
        }
    }
}
