using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.Core.API;

namespace ChakraCore.NET.Core
{
    public class ContextService : ServiceBase, IContextService
    {

        public JavaScriptValue JSValue_Undefined => serviceNode.WithContext<JavaScriptValue>(() =>
         {
             Native.ThrowIfError(Native.JsGetUndefinedValue(out JavaScriptValue result));
             return result;
         });

        public JavaScriptValue JSValue_Null => serviceNode.WithContext<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetUndefinedValue(out JavaScriptValue result));
            return result;
        });

        public JavaScriptValue JSValue_True => serviceNode.WithContext<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetTrueValue(out JavaScriptValue result));
            return result;
        });

        public JavaScriptValue JSValue_False => serviceNode.WithContext<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetFalseValue(out JavaScriptValue result));
            return result;
        });

        public JavaScriptValue GlobalObject => serviceNode.WithContext<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetGlobalObject(out JavaScriptValue result));
            return result;
        });

        public JavaScriptValue ParseScript(string script)
        {
            return serviceNode.WithContext<JavaScriptValue>(() =>
            {
                return JavaScriptContext.ParseScript(script);
            });
        }

        public string RunScript(string script)
        {
            return serviceNode.WithContext<string>(() =>
              {
                  var result= JavaScriptContext.RunScript(script);
                  return result.ConvertToString().ToString();
              });
        }

        public JavaScriptValue CreateFunction(JavaScriptNativeFunction function,IntPtr callbackData)
        {
            return serviceNode.WithContext<JavaScriptValue>(() =>
            {
                var f = serviceNode.GetService<INativeFunctionHolderService>().HoldFunction(function);
                return JavaScriptValue.CreateFunction(function, callbackData);
            });
                
        }

        public JavaScriptValue CreateObject()
        {
            return serviceNode.WithContext<JavaScriptValue>(() =>
            {
                return JavaScriptValue.CreateObject();
            });
        }

        public JavaScriptValue CreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback)
        {
            return serviceNode.WithContext<JavaScriptValue>(() =>
            {
                return JavaScriptValue.CreateExternalObject(data, finalizeCallback);
            });
        }

        public JavaScriptValue CallFunction(JavaScriptValue target, params JavaScriptValue[] para)
        {
            return serviceNode.WithContext<JavaScriptValue>(()=>
            {
                return target.CallFunction(para);
            });
        }

        public JavaScriptValue ConstructObject(JavaScriptValue target, params JavaScriptValue[] para)
        {
            return serviceNode.WithContext<JavaScriptValue>(() =>
            {
                return target.ConstructObject(para);
            });
        }
    }
}
