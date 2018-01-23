using System;
using ChakraCore.NET.Hosting;
namespace ChakraCore.NET.Plugin.Common
{
    public class EchoProvider : IPluginInstaller
    {
        public static Action<string> OnEcho;
        public void Install(JSValue target)
        {
            target.Binding.SetMethod<string>("echo", (msg) => OnEcho?.Invoke(msg));
        }
    }
}
