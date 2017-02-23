using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET
{
    public static class JSValueHelper
    {
        public static Task<TResult> CallPromise<TResult>(this JSValue source,string name)
        {
            return source.ReadProperty<Task<TResult>>(API.JavaScriptPropertyId.FromString(name));
        }
    }
}
