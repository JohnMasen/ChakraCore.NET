using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    class JavaScriptValueComparer : IComparer<JavaScriptValue>
    {
        public static readonly JavaScriptValueComparer Instance = new JavaScriptValueComparer();
        private JavaScriptValueComparer()
        {

        }
        public int Compare(JavaScriptValue x, JavaScriptValue y)
        {
            return x.reference.ToInt32() - y.reference.ToInt32();
        }

    }
}
