using ChakraCore.NET;
using System;

namespace EchoProvider
{
    public class EchoProvider : INativePlugin
    {
        public void Install(ChakraContext context)
        {
            context.GlobalObject.Binding.SetMethod<string>("echo",s=>Console.WriteLine(s));
        }
    }
}
