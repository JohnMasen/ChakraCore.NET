using ChakraCore.NET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET
{
    public static class BindingHelper
    {
        public static void SetTask<TResult>(this JSValueBinding binding, string name, Task<TResult> value)
        {
            binding.WriteProperty<Task<TResult>>(JavaScriptPropertyId.FromString(name), value);
        }
    }
}
