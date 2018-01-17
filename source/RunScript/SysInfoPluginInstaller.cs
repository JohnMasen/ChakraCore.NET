using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
using ChakraCore.NET.Plugin;
namespace RunScript
{
    class SysInfoPluginInstaller : INativePluginInstaller
    {
        public void Install(JSValue stub)
        {
            stub.ServiceNode.GetService<IJSValueConverterService>().RegisterStructConverter<SysInfoPlugin>(
                (value, obj) =>
                {
                    value.WriteProperty<string>("CommandArguments", obj.CommandArguments);
                    value.WriteProperty("Is64BitProcess", obj.Is64BitProcess);
                    value.WriteProperty("CurrentPath", obj.CurrentPath);
                    value.Binding.SetFunction("CurrentThread", () => { return System.Threading.Thread.CurrentThread.ManagedThreadId; });
                },
                (value) =>
                {
                    throw new NotImplementedException();
                });
            stub.WriteProperty("value", SysInfoPlugin.Default);

        }
    }

    struct SysInfoPlugin
    {
        public string CommandArguments;
        public bool Is64BitProcess;
        public string CurrentPath;
        public static SysInfoPlugin Default = new SysInfoPlugin()
        {
            CommandArguments = string.Join(" ", Environment.GetCommandLineArgs()),
            Is64BitProcess = Environment.Is64BitProcess,
            CurrentPath=Environment.CurrentDirectory
        };
    }

}
