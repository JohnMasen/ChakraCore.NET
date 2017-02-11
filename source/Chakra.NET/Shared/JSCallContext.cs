using ChakraHost.Hosting;
using Chakra.NET.GC;
using System;
using System.Collections.Generic;

namespace Chakra.NET
{
    [Flags]
    public enum CallContextOption
    {
        NoChange,
        NewJSRelease,
        NewDotnetRelease,
        NewJSObject
    }
    public class JSCallContext
    {
        public struct CallContextInfo
        {
            public string Text;
            public DelegateHolder Holder;
            public JavaScriptValue CurrentObject;
            public object UserData;
            public CallContextInfo(string text,DelegateHolder holder, JavaScriptValue currentObj,object userData)
            {
                Text = text;
                Holder = holder;
                CurrentObject = currentObj;
                UserData = userData;
            }
        }
        private Stack<CallContextInfo> stackTrace = new Stack<CallContextInfo>();

        public CallContextInfo StackInfo;
        public string Text { get; set; }
        //
        DelegateHolderManager holderManager;
        private JSCallContext(string text,DelegateHolderManager manager,DelegateHolder holder, JavaScriptValue currentObject,object userData)
        {
            holderManager = manager;
            StackInfo = new CallContextInfo(text,holder,currentObject,userData);
            stackTrace.Push(StackInfo);
        }

        public static JSCallContext CreateRoot(JavaScriptValue rootJSObject)
        {
            DelegateHolderManager manager = new DelegateHolderManager();
            var holder = manager.CreateHolder(false, false);//a root holder denines hold anything, this help avoid accidently reigster callback at root level
            return new JSCallContext("Root",new DelegateHolderManager(), holder, rootJSObject,null);
        }

        public DelegateHolder.InternalHanlder Push(string text, CallContextOption option, JavaScriptValue? currentObject,object userData)
        {
            DelegateHolder holder;
            DelegateHolder.InternalHanlder result=null;
            bool allowInternalRelease = option.HasFlag(CallContextOption.NewDotnetRelease);
            bool allowExternalRelease = option.HasFlag(CallContextOption.NewJSRelease);
            if (allowExternalRelease || allowInternalRelease)
            {
                holder = holderManager.CreateHolder(allowInternalRelease, allowExternalRelease);
                if (allowInternalRelease)
                {
                    result = holder.GetInternalHandler();
                }
            }
            else
            {
                holder = StackInfo.Holder;
            }
            if (option.HasFlag(CallContextOption.NewJSObject) && currentObject?.ValueType!=JavaScriptValueType.Object)
            {
                throw new ArgumentException("value can only be JavaScriptValue with valuetype=Object with CallContextOption.NewJSObject is set", nameof(currentObject));
            }
            StackInfo = new CallContextInfo(text,holder, currentObject ?? StackInfo.CurrentObject,userData);
            stackTrace.Push(StackInfo);
            return result;
        }

        public void Push(string text,object userData=null)
        {
            Push(text,CallContextOption.NoChange, null, userData);
        }


        public void Pop()
        {
            StackInfo = stackTrace.Pop();
        }

    }
}
