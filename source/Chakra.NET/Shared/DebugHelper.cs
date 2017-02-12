using System;
using System.Diagnostics;

namespace Chakra.NET
{
    public class DebugHelper
    {
        static GC.DelegateHolder.InternalHanlder handler;

        static Action<string> jsEcho;
        static Action<string> debugWrite = (s) => { Debug.WriteLine(s); };

        public void Echo(string s)
        {
            debugWrite(s);
        }

        public static void Inject(ChakraContext context)
        {
            jsEcho = (s) => { debugWrite(s); };
            context.ValueConverter.RegisterProxyConverter<DebugHelper>(
                (output, source) =>
                {
                    output.SetMethod<string>("echo", source.Echo);
                });
            context.WriteProperty<DebugHelper>(context.GlobalObject, "debug", new DebugHelper());
            
        }
        public static void RedirectEcho(Action<string> a)
        {
            debugWrite = a;
        }
    }
}
