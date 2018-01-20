using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class JSApp:JSClassWrapperBase
    {
        public string EntryPoint { get; set; }

        public virtual void Run()
        {
            Reference.CallMethod(EntryPoint);
        }

        public virtual void Run<T>(T parameter)
        {
            Reference.CallMethod<T>(EntryPoint, parameter);
        }

        public static JSApp Create(string moduleName="app",string className="app",string entryPointName="main",JavaScriptHosting hosting=null)
        {
            hosting = hosting ?? JavaScriptHosting.Default;
            var result = hosting.GetModuleClass<JSApp>(moduleName, className);
            result.EntryPoint = entryPointName;
            return result;
        }
    }
}
