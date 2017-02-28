using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class JSValueService : ServiceBase, IJSValueService
    {
        public JavaScriptValue JSValue_Undefined => contextSwitch.With<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetUndefinedValue(out JavaScriptValue result));
            return result;
        });

        public JavaScriptValue JSValue_Null => contextSwitch.With<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetUndefinedValue(out JavaScriptValue result));
            return result;
        });

        public JavaScriptValue JSValue_True => contextSwitch.With<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetTrueValue(out JavaScriptValue result));
            return result;
        });

        public JavaScriptValue JSValue_False => contextSwitch.With<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetFalseValue(out JavaScriptValue result));
            return result;
        });

        public JavaScriptValue GlobalObject => contextSwitch.With<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetGlobalObject(out JavaScriptValue result));
            return result;
        });
        public T ReadProperty<T>(JavaScriptValue target, JavaScriptPropertyId id)
        {
            var convert = serviceNode.GetService<IJSValueConverterService>();
            return serviceNode.GetService<IContextSwitchService>().With<T>(
                () =>
                {
                    return convert.FromJSValue<T>( target.GetProperty(id));
                });
                ;
        }

        public void WriteProperty<T>(JavaScriptValue target, JavaScriptPropertyId id, T value)
        {
            var convert = serviceNode.GetService<IJSValueConverterService>();
            serviceNode.GetService<IContextSwitchService>().With(
                () =>
                {
                    target.SetProperty(id, convert.ToJSValue<T>(value),true);
                });

            ;
        }

        public T ReadProperty<T>(JavaScriptValue target, string id)
        {
            var convert = serviceNode.GetService<IJSValueConverterService>();
            return serviceNode.GetService<IContextSwitchService>().With<T>(
                () =>
                {
                    return convert.FromJSValue<T>(target.GetProperty(JavaScriptPropertyId.FromString(id) ));
                });
            ;
        }

        public void WriteProperty<T>(JavaScriptValue target, string id, T value)
        {
            var convert = serviceNode.GetService<IJSValueConverterService>();
            serviceNode.GetService<IContextSwitchService>().With(
                () =>
                {
                    target.SetProperty(JavaScriptPropertyId.FromString(id), convert.ToJSValue<T>(value), true);
                });

            ;
        }
        public JavaScriptValue CreateFunction(JavaScriptNativeFunction function, IntPtr callbackData)
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                var f = serviceNode.GetService<INativeFunctionHolderService>().HoldFunction(function);
                return JavaScriptValue.CreateFunction(function, callbackData);
            });

        }

        public JavaScriptValue CreateObject()
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                return JavaScriptValue.CreateObject();
            });
        }

        public JavaScriptValue CreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback)
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                return JavaScriptValue.CreateExternalObject(data, finalizeCallback);
            });
        }

        public JavaScriptValue CallFunction(JavaScriptValue target, params JavaScriptValue[] para)
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                return target.CallFunction(para);
            });
        }

        public JavaScriptValue ConstructObject(JavaScriptValue target, params JavaScriptValue[] para)
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                return target.ConstructObject(para);
            });
        }

        public JavaScriptValue CreateArray(uint length)
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                return JavaScriptValue.CreateArray(length);
            });
        }
    }
}
