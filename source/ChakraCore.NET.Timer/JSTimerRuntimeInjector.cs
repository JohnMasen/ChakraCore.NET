using ChakraCore.NET.API;
using ChakraCore.NET.Timer;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public static class JSTimerRuntimeInjector
    {
        public static void InjecTimerService(this ChakraRuntime runtime)
        {
            runtime.ServiceNode.GetService<IJSValueConverterService>().RegisterProxyConverter<JSTimer>((binding, value, node) =>
            {
                binding.SetMethod<Action, int>("setTimeout", value.SetTimeout);
                binding.SetFunction<JavaScriptValue, int, Guid>("setInterval", value.SetInterval);
                binding.SetMethod<Guid>("clearInterval", value.ClearInterval);
            });
        }

        public static void InitTimer(this ChakraContext context)
        {
            context.GlobalObject.WriteProperty<JSTimer>("timer", new JSTimer(context.ServiceNode));
        }
    }
}
