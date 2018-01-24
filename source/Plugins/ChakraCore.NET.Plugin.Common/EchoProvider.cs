using System;
using ChakraCore.NET.Hosting;
namespace ChakraCore.NET.Plugin.Common
{
    public class EchoProvider : IPluginInstaller
    {
        private IEchoFunctionProvider instance;
        public EchoProvider(IEchoFunctionProvider instance)
        {
            this.instance = instance;
        }
        public string SDK => Properties.Resources.echo;

        public string Name => "Echo";

        public string GetSDK()
        {
            return Properties.Resources.echo;
        }

        public void Install(JSValue target)
        {
            target.Binding.SetMethod<string>("echo", (msg) => instance.Echo(msg));
        }
    }
}
