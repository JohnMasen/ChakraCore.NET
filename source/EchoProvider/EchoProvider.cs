using ChakraCore.NET;
using ChakraCore.NET.Plugin;
using System;

namespace EchoProvider
{
    public class EchoProvider : INativePlugin
    {
        public void Install(JSValue stub)
        {
            stub.Binding.SetMethod<string>("echo",s=>Console.WriteLine(s));
        }
    }
}
