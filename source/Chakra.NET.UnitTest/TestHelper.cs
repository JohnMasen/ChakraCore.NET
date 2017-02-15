using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Chakra.NET.UnitTest
{
    class TestHelper
    {
        public static string script(string name)
        {
            string filename = System.IO.Directory.GetCurrentDirectory() + string.Format(@"\Scripts\{0}", name);
            return System.IO.File.OpenText(filename).ReadToEnd();
        }


        public static string JSRunScript = "var a='test'; a;";
        public static string JSValueTest = "var b=a;";
        public static string JSCall = "function add(s){return s+s}function addcallback(s,callback){return s+callback(s)}";
        public static string JSProxy= "var callProxyResult = myStub.echo('hi');";

    }
}
