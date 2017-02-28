using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ChakraCore.NET;

namespace ChakraCore.NET.UnitTest
{
    class TimerHelper
    {
        public void SetTimeout(Action a, int delay)
        {
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(delay);//TODO: need to hold the callback object or it will be GC by chakracore
                a();
            });
        }

        public static void RegisterTimer(ChakraRuntime runtime)
        {
            //ij
            //conv.RegisterMethodConverter();
            //context.ValueConverter.RegisterProxyConverter<TimerHelper>((binding, value) =>
            //{
            //    binding.SetMethod<Action, int>("setTimeout", value.SetTimeout);
            //}

            //);
        }
    }
}
