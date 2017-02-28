
using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    class JavaScriptValueComparer : IComparer<JavaScriptValue>
    {
        public static readonly JavaScriptValueComparer Instance = new JavaScriptValueComparer();
        private JavaScriptValueComparer()
        {

        }
        public int Compare(JavaScriptValue x, JavaScriptValue y)
        {
            return x.reference.ToInt64().CompareTo(y.reference.ToInt64());
        }

    }
}
