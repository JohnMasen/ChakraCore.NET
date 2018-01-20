using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class JSApp:JSClassWrapperBase
    {
        public string EntryPoint { get; set; } = "main";

        public virtual void Run()
        {
            Reference.CallMethod(EntryPoint);
        }

        public virtual void Run<T>(T parameter)
        {
            Reference.CallMethod<T>(EntryPoint, parameter);
        }

    }
}
