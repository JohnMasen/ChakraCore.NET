using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class NativeFunctionHolderService : ServiceBase, INativeFunctionHolderService
    {
        SortedDictionary<Guid, JavaScriptNativeFunction> dict = new SortedDictionary<Guid, JavaScriptNativeFunction>();
        public bool IsOneTime { get; private set; }
        public NativeFunctionHolderService(bool isOneTime)
        {
            IsOneTime = isOneTime;
        }
        
        public JavaScriptNativeFunction HoldFunction(JavaScriptNativeFunction function)
        {
            Guid id = Guid.NewGuid();
            if (!IsOneTime)
            {
                dict.Add(id, function);
                return function;
            }
            else
            {
                JavaScriptNativeFunction result = new JavaScriptNativeFunction((callee, isConstructCall, arguments, argumentCount, callbackData) =>
                  {
                      var r=function(callee, isConstructCall, arguments, argumentCount, callbackData);
                      dict.Remove(id);
                      return r;
                  });
                dict.Add(id, result);
                return result;
            }
        }
    }
}
