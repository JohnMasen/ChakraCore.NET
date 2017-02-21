using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
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
            return s + s;
        }

        public string GetName()
        {
            return Name;
        }

        private static void Inject(JSValueBinding binding,TestProxy value)
        {
            binding.SetFunction<string, string>("echo", value.Echo);
            binding.SetFunction<string>("GetName", value.GetName);
        }

        public static void RegisterValueConverter(ChakraContext context)
        {
            context.ValueConverter.RegisterProxyConverter<TestProxy>(Inject);
        }

    }
}
