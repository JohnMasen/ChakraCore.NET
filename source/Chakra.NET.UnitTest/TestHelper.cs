using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chakra.NET.UnitTest
{
    class TestHelper
    {
        static bool requireSetup = true;
        private static void setupLogging()
        {
            if (requireSetup)
            {
                requireSetup = false;
                ChakraLogging.LoggerFactory.AddDebug();
            }
        }
        public static ChakraContext CreateContext()
        {
            setupLogging();
            var runtime = ChakraRuntime.Create();
            return runtime.CreateContext(true);
        }

        public static string script(string name)
        {
            string filename = System.IO.Directory.GetCurrentDirectory() + string.Format(@"\Scripts\{0}", name);
            return System.IO.File.OpenText(filename).ReadToEnd();
        }


        public static string JSRunScript = "var a='test'; a;";
    }
}
