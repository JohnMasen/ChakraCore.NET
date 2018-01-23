using System;
using ChakraCore.NET.Hosting;
namespace ChakraCore.NET.Plugin.Common
{
    public class EchoProvider : IPluginInstaller,IScriptSDKProvider
    {
        public static Action<string> OnEcho;

        public string SDK => Properties.Resources.echo;

        public void Install(JSValue target)
        {
            target.Binding.SetMethod<string>("echo", (msg) => OnEcho?.Invoke(msg));
        }
    }
}
