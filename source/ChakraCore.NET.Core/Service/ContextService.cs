using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class ContextService : ServiceBase, IContextService
    {

        public JavaScriptValue ParseScript(string script)
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                return JavaScriptContext.ParseScript(script);
            });
        }

        public string RunScript(string script)
        {
            return contextSwitch.With<string>(() =>
              {
                  var result= JavaScriptContext.RunScript(script);
                  return result.ConvertToString().ToString();
              });
        }

        
    }
}
