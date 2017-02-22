using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChakraCore.NET.UnitTest
{
    class TimerHelper
    {
        public void SetTimeout(Action a, int delay)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(delay);
                a();
            });
        }

        public static void RegisterConverter(ChakraContext context)
        {
            context.ValueConverter.RegisterMethodConverter();
            context.ValueConverter.RegisterProxyConverter<TimerHelper>((binding, value) =>
            {
                binding.SetMethod<Action, int>("setTimeout", value.SetTimeout);
            }

            );
        }
    }
}
