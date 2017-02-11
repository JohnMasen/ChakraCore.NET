using System;
using System.Diagnostics;

namespace Chakra.NET
{
    public class DebugHelper
    {
        static GC.DelegateHolder.InternalHanlder handler;

        static Action<string> jsEcho;
        static Action<string> debugWrite = (s) => { Debug.WriteLine(s); };

        public static void Inject(ChakraContext context)
        {
            jsEcho = (s) => { debugWrite(s); };
            using (var h=context.With(CallContextOption.NewDotnetRelease, null, "debug inject"))
            {
                handler = h.Handler;
                context.GlobalObjectWithContext.SetMethod<string>("echo", jsEcho);
            }
            
        }
        public static void RedirectEcho(Action<string> a)
        {
            jsEcho = a;
        }
    }
}
