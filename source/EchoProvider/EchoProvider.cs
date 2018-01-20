using ChakraCore.NET;
using ChakraCore.NET.Hosting;
using System;

namespace EchoProvider
{
    public class EchoProvider : IPluginInstaller
    {
        public void Install(JSValue stub)
        {
            stub.Binding.SetMethod<string>("echo",s=>Console.WriteLine(s));
        }
    }
}
