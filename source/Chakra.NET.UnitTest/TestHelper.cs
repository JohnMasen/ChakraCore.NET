using System;
using System.Collections.Generic;
using System.Text;

namespace Chakra.NET.UnitTest
{
    class TestHelper
    {
        public static ChakraContext CreateContext()
        {
            var runtime = ChakraRuntime.Create();
            return runtime.CreateContext(true);
        }

        public static string script(string name)
        {
            string filename = System.IO.Directory.GetCurrentDirectory() + string.Format(@"\Scripts\{0}", name);
            return System.IO.File.OpenText(filename).ReadToEnd();
        }
    }
}
