using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.GC
{
    public class DelegateHandler
    {
        private List<JavaScriptNativeFunction> items = new List<JavaScriptNativeFunction>();
        public JavaScriptNativeFunction Hold(JavaScriptNativeFunction item)
        {
            items.Add(item);
            return item;
        }
    }
}
