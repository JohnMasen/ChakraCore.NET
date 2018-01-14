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
        //private SysInfoPlugin(string args,bool is64)
        //{
        //    CommandArguments = args;
        //    Is64BitProcess = is64;
        //}
        public static SysInfoPlugin Default = new SysInfoPlugin()
        {
            CommandArguments = string.Join(" ", Environment.GetCommandLineArgs()),
            Is64BitProcess = Environment.Is64BitProcess
        };
    }

}
