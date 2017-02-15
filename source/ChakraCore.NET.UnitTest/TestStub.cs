using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;
namespace ChakraCore.NET.UnitTest
{
    class TestStub
    {


        public string Echo(string s)
        {
            return s + s;
        }
        private static void Inject(JSValueBinding binding,TestStub value)
        {
            binding.SetFunction<string, string>("echo", value.Echo);
        }

        public static void RegisterValueConverter(ChakraContext context)
        {
            context.ValueConverter.RegisterProxyConverter<TestStub>(Inject);
        }

    }
}
