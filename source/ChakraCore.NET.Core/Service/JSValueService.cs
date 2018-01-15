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

        public JavaScriptValue JSGlobalObject => contextSwitch.With<JavaScriptValue>(() =>
        {
            Native.ThrowIfError(Native.JsGetGlobalObject(out JavaScriptValue result));
            return result;
        });
        public T ReadProperty<T>(JavaScriptValue target, JavaScriptPropertyId id)
        {
            return contextSwitch.With<T>(
                () =>
                {
                    return converter.FromJSValue<T>( target.GetProperty(id));
                });
                ;
        }

        public void WriteProperty<T>(JavaScriptValue target, JavaScriptPropertyId id, T value)
        {
            contextSwitch.With(
                () =>
                {
                    target.SetProperty(id, converter.ToJSValue<T>(value),true);
                });

            ;
        }

        public T ReadProperty<T>(JavaScriptValue target, string id)
        {
            return contextSwitch.With<T>(
                () =>
                {
                    return converter.FromJSValue<T>(target.GetProperty(JavaScriptPropertyId.FromString(id) ));
                });
            ;
        }

        public void WriteProperty<T>(JavaScriptValue target, string id, T value)
        {
            contextSwitch.With(() =>
                {
                    target.SetProperty(JavaScriptPropertyId.FromString(id), converter.ToJSValue<T>(value), true);
                });

            ;
        }
        public JavaScriptValue CreateFunction(JavaScriptNativeFunction function, IntPtr callbackData)
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                var result= JavaScriptValue.CreateFunction(function, callbackData);
                serviceNode.GetService<IGCSyncService>().SyncWithJsValue(function,result);//keep delegate alive until related javascript value is released
                return result;
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

        public bool HasProperty(JavaScriptValue target, JavaScriptPropertyId id)
        {
            
            return contextSwitch.With<bool>(() =>
            {
                return target.HasProperty(id);
            });
        }

        public bool HasProperty(JavaScriptValue target, string id)
        {
            return contextSwitch.With<bool>(() =>
            {
                return target.HasProperty(JavaScriptPropertyId.FromString(id));
            });
        }

        public void ThrowIfErrorValue(JavaScriptValue value)
        {
            if (value.IsValid && value.ValueType==JavaScriptValueType.Error)
            {
                string message = ReadProperty<string>(value, "description");
                throw new JavaScriptFatalException(JavaScriptErrorCode.Fatal, message);
            }
        }
    }
}
