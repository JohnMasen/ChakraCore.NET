using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;

namespace ChakraCore.NET.Promise
{
    public class PromiseNativeFunctionHolderService : ServiceBase, INativeFunctionHolderService
    {
        private Dictionary<Guid, Tuple<JavaScriptNativeFunction, JavaScriptNativeFunction>> list = new Dictionary<Guid, Tuple<JavaScriptNativeFunction, JavaScriptNativeFunction>>();
        int count = 0;
        Guid id ;
        Tuple<JavaScriptNativeFunction, JavaScriptNativeFunction> value;
        public void StartSession()
        {
            count = 0;
            id = Guid.NewGuid();
        }
        public void StopSession()
        {
            if (count!=2)
            {
                throw new InvalidOperationException("Promise delegate should be paired");
            }
        }
        
        public JavaScriptNativeFunction HoldFunction(JavaScriptNativeFunction function)
        {
            Guid sessionID = id;
            JavaScriptNativeFunction result = new JavaScriptNativeFunction((callee, isConstructCall, arguments, argumentCount, callbackData) =>
            {
                try
                {
                    var r = function(callee, isConstructCall, arguments, argumentCount, callbackData);
                    return r;
                }
                catch(Exception)
                {
                    
                    throw;
                }
                finally
                {
                    list.Remove(sessionID);//always make sure delegate is released ;
                }
            });
            count++;
            if (count==1)
            {
                value = new Tuple<JavaScriptNativeFunction, JavaScriptNativeFunction>(result, null);
            } else if (count==2)
            {
                value = new Tuple<JavaScriptNativeFunction, JavaScriptNativeFunction>(value.Item1, result);
                list.Add(sessionID, value);
            }
            else
            {
                throw new InvalidOperationException("Promise delegate should be exactly 2 functions(fullfill and reject)");
            }
            return result;
        }
    }
}
