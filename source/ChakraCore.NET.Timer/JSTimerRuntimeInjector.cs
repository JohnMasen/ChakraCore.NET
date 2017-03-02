using ChakraCore.NET.API;
using ChakraCore.NET.Timer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public static class JSTimerRuntimeInjector
    {
        public static void InjecTimerService(this IServiceNode target)
        {
            target.GetService<IJSValueConverterService>().RegisterProxyConverter<JSTimer>((binding, value, node) =>
            {
                binding.SetMethod<Action, int>("setTimeout", value.SetTimeout);
                binding.SetFunction<JavaScriptValue, int, Guid>("setInterval", value.SetInterval);
                binding.SetMethod<Guid>("clearInterval", value.ClearInterval);
            });
        }

        public static void InitTimer(this JSValue globalObject)
        {
            globalObject.WriteProperty<JSTimer>("timer", new JSTimer(globalObject.ServiceNode));
        }
    }
}
