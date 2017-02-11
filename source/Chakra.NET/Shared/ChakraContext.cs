using System;
using ChakraHost.Hosting;
using System.Collections.Generic;
using Chakra.NET.GC;

namespace Chakra.NET
{
    public class ChakraContext:IDisposable
    {
        private static JavaScriptSourceContext currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        //private Dictionary<object, List<object>> holder = new Dictionary<object, List<object>>();
        private static Queue<JavaScriptValue> taskQueue = new Queue<JavaScriptValue>();

        internal JavaScriptContext jsContext;
        public JSValueConverter ValueConverter { get; set; }

        //public GC.StackTraceNode GCStackTrace { get; private set; }

        public JavaScriptValue GlobalObject
        {
            get
            {
                using (With())
                {
                    return JavaScriptValue.GlobalObject;
                }
            }
        }

        public JSValueWithContext GlobalObjectWithContext
        {
            get
            {
                return GlobalObject.WithContext(this);
            }
        }
        private bool isDebug;
        internal ChakraContext(JavaScriptContext jsContext)
        {
            this.jsContext = jsContext;
        }
        
        internal void init(bool enableDebug)
        {
            isDebug = enableDebug;
            JavaScriptContext.Current = jsContext;
            JavaScriptPromiseContinuationCallback promiseContinuationCallback = delegate (JavaScriptValue task, IntPtr callbackState)
            {
                taskQueue.Enqueue(task);
            };

            if (Native.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero) != JavaScriptErrorCode.NoError)
                throw new InvalidOperationException("failed to setup callback for ES6 Promise");

            if (Native.JsProjectWinRTNamespace("Windows") != JavaScriptErrorCode.NoError)
                throw new InvalidOperationException("failed to project windows namespace.");
            if (isDebug && Native.JsStartDebugging() != JavaScriptErrorCode.NoError)
                throw new InvalidOperationException("failed to start debugging.");


            this.CallContext = JSCallContext.CreateRoot(JavaScriptValue.GlobalObject);
            ValueConverter = new JSValueConverter(this);
            JSValueConverterExtend.Inject(this);
            JavaScriptContext.Current = JavaScriptContext.Invalid;
            //GCStackTrace = StackTraceNode.CreateRoot();//stack trace root
            //PushCaller(GlobalObject);
            
            

        }

        public JSCallContext CallContext { get; private set; }

        //internal JavaScriptValue CurrentCaller;
        //private Stack<JavaScriptValue> callerStack = new Stack<JavaScriptValue>();
        ///// <summary>
        ///// set current caller, for call method and call function use
        ///// </summary>
        ///// <param name="value"></param>
        //internal void PushCaller(JavaScriptValue value)
        //{
        //    if (value.ValueType!=JavaScriptValueType.Object)
        //    {
        //        throw new InvalidOperationException("PushCaller only accept [Object] Javascript value");
        //    }
        //    CurrentCaller = value;
        //    callerStack.Push(value);
        //}

        //internal void PopCaller()
        //{
        //    CurrentCaller = callerStack.Pop();
        //}




        //private void pushStackTrace(object owner)
        //{
        //    GCStackTrace = GCStackTrace.CreateChild(owner);
        //}

        //private void popStackTrace()
        //{
        //    if (GCStackTrace.Parent==null)
        //    {
        //        throw new InvalidOperationException("cannot pop StackTrace at top level");
        //    }
        //    GCStackTrace = GCStackTrace.Parent;
        //}

        public JavaScriptValue CreateObject()
        {
            using (With())
            {
                return JavaScriptValue.CreateObject();
            }
        }

        public JavaScriptValue CreateProxyObject<T>(T owner,Action<JSValueWithContext> injector)
        {
            //if (GCStackTrace.IsRoot)
            //{
            //    throw new InvalidOperationException("CreateProxyObject can only be called inside a Using(With(timespan,object)){} block. possible incorrect valueconverter registered");
            //}
            using (var h=With(CallContextOption.NewJSRelease,null,"CreateProxyObject"))
            {
                var result= JavaScriptValue.CreateExternalObject(IntPtr.Zero, CallContext.StackInfo.Holder.GetExternalReleaseDelegate());
                injector(result.WithContext(this));
                return result;
            }
        }

        //public ContextSwitcher With(TimeSpan timeout)
        //{
        //    return With(timeout, null);
        //}

        public ContextSwitcher With(CallContextOption stackOperate,JavaScriptValue? newJSObject,string text)
        {
            return ContextSwitcher.waitAndSwitch(this,stackOperate,newJSObject,text);
        }

        public ContextSwitcher With()
        {
            return With(CallContextOption.NoChange, null, string.Empty);
        }



        public T RunScript<T>(string script)
        {
            using (With())
            {
                JavaScriptValue result;
                if (isDebug)
                {
                    result = JavaScriptContext.RunScript(script, currentSourceContext++, string.Empty);
                }
                else
                {
                    result = JavaScriptContext.RunScript(script);
                }
                return ValueConverter.FromJSValue<T>(result);
            }
        }

        //public void HoldObject(object obj)
        //{
        //    GCStackTrace.Hold(obj);
        //}

        //public JavaScriptValue CreateCallBackFunction(JavaScriptNativeFunction callback)
        //{
        //    using (With())
        //    {
        //        HoldObject(callback);
        //        return JavaScriptValue.CreateFunction(callback);
        //    }
        //}

        public JavaScriptValue ParseScript(string script)
        {
            using (With())
            {
                JavaScriptValue result;
                if (isDebug)
                {
                    result = JavaScriptContext.ParseScript(script, currentSourceContext++, string.Empty);
                }
                else
                {
                    result = JavaScriptContext.ParseScript(script);
                }
                return result;
            }
        }

        //public static ChakraContext CreateNew( bool enableDebug = true)
        //{
        //    if (!isReady)
        //    {
        //        initPrivate();
        //        isReady = true;
        //    }
        //    return CreateNew(runtime);
        //}

        //public static ChakraContext CreateNew(JavaScriptRuntime jsRuntime, bool enableDebug=true)
        //{
        //    var c = jsRuntime.CreateContext();
        //    var result = new ChakraContext(c);
        //    result.init(enableDebug);

        //    if (enableDebug)
        //    {
        //        DebugHelper.Inject(result);
        //    }
        //    return result;
        //}

        public void WriteProperty<T>(JavaScriptValue target,string name,T value)
        {
            using (With())
            {
                target.SetProperty(name, ValueConverter.ToJSValue<T>(value));
            }
        }

        public T ReadProperty<T>(JavaScriptValue target,string name)
        {
            using (With())
            {
                var id = JavaScriptPropertyId.FromString(name);
                if (target.HasProperty(id))
                {
                    return ValueConverter.FromJSValue<T>(target.GetProperty(name));
                }
                else
                {
                    return default(T);
                }
                
            }
        }

        public T ReadValue<T>(JavaScriptValue target)
        {
            using (With())
            {
                return ValueConverter.FromJSValue<T>(target);
            }
        }

        public JavaScriptValue CreateValue<T>(T source)
        {
            using (With())
            {
                return ValueConverter.ToJSValue<T>(source);
            }
        }

        //private static void initPrivate()
        //{
        //    runtime = JavaScriptRuntime.Create(JavaScriptRuntimeAttributes.None,JavaScriptRuntimeVersion.VersionEdge);
        //}

        

        public class ContextSwitcher:IDisposable
        {
            JavaScriptContext previous;
            ChakraContext context;

            /// <summary>
            /// hold this handler if you want handle the delegates in your code
            /// </summary>
            public DelegateHolder.InternalHanlder Handler { get; private set; }
            
            private ContextSwitcher(ChakraContext newContext, CallContextOption stackOperate,JavaScriptValue? newJSObject,string text)
            {
                this.previous = JavaScriptContext.Current;
                context = newContext;
                JavaScriptContext.Current = newContext.jsContext;
                
                context.CallContext.Push(text, stackOperate, newJSObject, null);
                if (stackOperate.HasFlag(CallContextOption.NewDotnetRelease))
                {
                    this.Handler = context.CallContext.StackInfo.Holder.GetInternalHandler();
                }
            }


            internal static ContextSwitcher waitAndSwitch(ChakraContext context, CallContextOption stackOperate, JavaScriptValue? newJSObject,string text)
            {
                //TODO: implement wait
                return  new ContextSwitcher(context, stackOperate,newJSObject,text);

            }
            #region IDisposable Support

            public void Dispose()
            {
                context.CallContext.Pop();
                
                JavaScriptContext.Current = previous;
            }
            #endregion

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    jsContext.Release();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
}
