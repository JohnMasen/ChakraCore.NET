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
                binding.SetFunction<Action, int, Guid>("setInterval", value.SetInterval);
                binding.SetMethod<Guid>("clearInterval", value.ClearInterval);
            });
        }

        public static JSTimer InitTimer(this JSValue globalObject)
        {
            JSTimer result = new JSTimer(globalObject.ServiceNode);
            globalObject.WriteProperty<JSTimer>("timer", result);
            return result;
        }
    }
}
