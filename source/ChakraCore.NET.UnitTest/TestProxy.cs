using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
using System.Threading.Tasks;

namespace ChakraCore.NET.UnitTest
{
    class TestProxy
    {
        public string Name { get; private set; }
        public TestProxy(string name)
        {
            Name = name;
        }

        public TestProxy() : this("TestProxy")
        { }

        public string Echo(string s)
        {
            System.Diagnostics.Debug.WriteLine($"Echo {s}");
            return s + s;
        }

        public void CallBackToJS(Action<string> callback)
        {
            //System.GC.Collect();
            callback("callback from JS executed");
        }

        public string GetName()
        {
            return Name;
        }

        public async Task<int> AsyncCallAsync()
        {
            TestHelper.DumpThreadID("Async delay start");
            await Task.Delay(1000);
            //System.GC.Collect();
            TestHelper.DumpThreadID("Async delay stop");
            return 1;
        }



        public static void Inject(ChakraContext context)
        {
            context.ValueConverter.RegisterProxyConverter<TestProxy>((binding,value)=>
            {
                binding.SetFunction<string, string>("echo", value.Echo);
                binding.SetFunction<string>("GetName", value.GetName);
                binding.SetFunction<Task<int>>("asyncFunction", value.AsyncCallAsync);
                binding.RuntimeContext.ValueConverter.RegisterMethodConverter<string>();
                binding.SetMethod<Action<string>>("callBackToJs", value.CallBackToJS);
            });
        }

        

    }
}
