using System;
using ChakraCore.NET;

namespace TestProvider1
{
    public class Testprovider1 : ChakraCore.NET.INativePlugin
    {
        public void Install(ChakraContext context)
        {
            TestLibrary1.Class1 c = new TestLibrary1.Class1();
        }
    }
}
