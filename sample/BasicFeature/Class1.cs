using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChakraCore.NET;
using System.Diagnostics;

namespace BasicFeature
{
    class Class1
    {

        public void test1()
        {
            ChakraRuntime runtime = ChakraRuntime.Create();
            ChakraContext context = runtime.CreateContext(true);
            var s=context.RunScript("1+1");
            //context.GlobalObject.Binding.SetFunction<int, int>("add", Add);
            //context.RunScript("function test(callback){callback('hello world')})");
            //context.ServiceNode.GetService<IJSValueConverterService>().RegisterMethodConverter<string>();
            //context.GlobalObject.CallMethod<Action<string>>("test", echo);
            DebugEcho instance = new DebugEcho();
            //context.ServiceNode.GetService<IJSValueConverterService>().RegisterProxyConverter<DebugEcho>(
            //    (binding, instance, serviceNode) =>
            //    {
            //        binding.SetMethod<string>("echo",instance.Echo);
            //    });
            //DebugEcho obj = new DebugEcho();
            //context.GlobalObject.WriteProperty<DebugEcho>("debugEcho", obj);
        }
        public void echo(string s)
        {
            Debug.WriteLine(s);
        }

        public int Add(int value)
        {
            return value + 1;
        }

        public class DebugEcho
        {
            public void Echo(String s)
            {
                Debug.WriteLine(s);
            }
        }

    }
}
