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

        public string GetName()
        {
            return Name;
        }

        public async Task<int> AsyncCall()
        {
            await Task.Delay(1);
            return 1;
        }

        

        public static void Inject(ChakraContext context)
        {
            context.ValueConverter.RegisterProxyConverter<TestProxy>((binding,value)=>
            {
                binding.SetFunction<string, string>("echo", value.Echo);
                binding.SetFunction<string>("GetName", value.GetName);
                binding.SetFunction<Task<int>>("asyncFunction", value.AsyncCall);
            });
        }

        

    }
}
