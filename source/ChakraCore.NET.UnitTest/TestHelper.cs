using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.UnitTest
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
        public static string JSArrayBuffer = "var array=new Int8Array(buffer);array.fill(0x0f);";
        public static string JSArrayBufferSetGet = "var array=new Int8Array(buffer1);array.fill(0x0f);var buffer2=buffer1;";

    }
}
