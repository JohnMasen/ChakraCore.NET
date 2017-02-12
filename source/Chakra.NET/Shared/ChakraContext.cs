using System;
using Chakra.NET.API;
using System.Collections.Generic;
using Chakra.NET.GC;
using System.Threading;

namespace Chakra.NET
{
    public class ChakraContext : IDisposable
    {
        private static JavaScriptSourceContext currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        //private Dictionary<object, List<object>> holder = new Dictionary<object, List<object>>();
        private static Queue<JavaScriptValue> taskQueue = new Queue<JavaScriptValue>();

        internal JavaScriptContext jsContext;
        public JSValueConverter ValueConverter { get; set; }

        private AutoResetEvent waitHanlder;

        //public GC.StackTraceNode GCStackTrace { get; private set; }

        public JavaScriptValue GlobalObject
        {
            get
            {
                JavaScriptValue result ;
                using (With())
                {
                    result = JavaScriptValue.GlobalObject;
                }
                return result;
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
        internal ChakraContext(JavaScriptContext jsContext, AutoResetEvent syncHandler)
        {
            this.jsContext = jsContext;
            this.waitHanlder = syncHandler;
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
            JavaScriptContext.Current = JavaScriptContext.Invalid;
            JSValueConverterExtend.Inject(this);
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

        public JavaScriptValue CreateProxyObject<T>(T owner, Action<JSValueWithContext> injector)
        {
            //if (GCStackTrace.IsRoot)
            //{
            //    throw new InvalidOperationException("CreateProxyObject can only be called inside a Using(With(timespan,object)){} block. possible incorrect valueconverter registered");
            //}
            JavaScriptValue result;
            using (var h = With(CallContextOption.NewJSRelease, null, "CreateProxyObject"))
            {
                result = JavaScriptValue.CreateExternalObject(IntPtr.Zero, CallContext.StackInfo.Holder.GetExternalReleaseDelegate());
            }
            injector(result.WithContext(this));
            return result;
        }

        //public ContextSwitcher With(TimeSpan timeout)
        //{
        //    return With(timeout, null);
        //}

        public ContextSwitcher With(CallContextOption stackOperate, JavaScriptValue? newJSObject, string text)
        {
            return ContextSwitcher.waitAndSwitch(this, stackOperate, newJSObject, text, waitHanlder);
        }
        public void Enter()
        {
            JavaScriptContext.Current = jsContext;
            var x = IsCurrentContext;
        }

        public bool IsCurrentContext
        {
            get
            {
                return JavaScriptContext.Current.reference == jsContext.reference;
            }
        }

        public void Leave()
        {
            JavaScriptContext.Current = JavaScriptContext.Invalid;
        }

        public ContextSwitcher With()
        {
            return With(CallContextOption.NoChange, null, string.Empty);
        }



        public T RunScript<T>(string script)
        {
            JavaScriptValue result;
            using (With())
            {
                if (isDebug)
                {
                    result = JavaScriptContext.RunScript(script, currentSourceContext++, string.Empty);
                }
                else
                {
                    result = JavaScriptContext.RunScript(script);
                }
            }
            return ValueConverter.FromJSValue<T>(result);
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

        public void WriteProperty<T>(JavaScriptValue target, string name, T value)
        {
            var tmp = ValueConverter.ToJSValue<T>(value);
            using (With())
            {
                target.SetProperty(JavaScriptPropertyId.FromString(name), tmp,true);
            }
        }

        public T ReadProperty<T>(JavaScriptValue target, string name)
        {
            JavaScriptValue tmp;
            using (With())
            {
                var id = JavaScriptPropertyId.FromString(name);
                
                if (target.HasProperty(id))
                {
                    tmp=target.GetProperty(JavaScriptPropertyId.FromString(name));
                }
                else
                {
                    return default(T);
                }
            }
            return ValueConverter.FromJSValue<T>(tmp);
        }

        public T ReadValue<T>(JavaScriptValue target)
        {
            //using (With())
            //{
            return ValueConverter.FromJSValue<T>(target);
            //}
        }

        public JavaScriptValue CreateValue<T>(T source)
        {
            //using (With())
            //{
            return ValueConverter.ToJSValue<T>(source);
            //}
        }





        public class ContextSwitcher : IDisposable
        {
            static bool alreadyCalled = false;
            ChakraContext context;
            AutoResetEvent syncHandler;
            bool isNested;
            /// <summary>
            /// hold this handler if you want handle the delegate release in your code
            /// </summary>
            public DelegateHolder.InternalHanlder Handler { get; private set; }

            private ContextSwitcher(bool isNested, ChakraContext context, CallContextOption stackOperate, JavaScriptValue? newJSObject, string text, AutoResetEvent syncHandler)
            {
                this.isNested = isNested;
                this.context = context;
                context.CallContext.Push(text, stackOperate, newJSObject, null);
                if (stackOperate.HasFlag(CallContextOption.NewDotnetRelease))
                {
                    this.Handler = context.CallContext.StackInfo.Holder.GetInternalHandler();
                }
                this.syncHandler = syncHandler;
            }


            internal static ContextSwitcher waitAndSwitch(ChakraContext context, CallContextOption stackOperate, JavaScriptValue? newJSObject, string text, AutoResetEvent syncHandler)
            {
                if (context.IsCurrentContext)//nested call from callback, do not block 
                {
                    return new ContextSwitcher(true,context, stackOperate, newJSObject, text, syncHandler);
                }
                if (alreadyCalled)
                {
                    throw new InvalidOperationException("With method cannot be nested");
                }
                else
                {
                    alreadyCalled = true;
                }
                syncHandler.WaitOne();
                context.Enter();
                return new ContextSwitcher(false,context, stackOperate, newJSObject, text, syncHandler);

            }
            #region IDisposable Support

            public void Dispose()
            {
                context.CallContext.Pop();
                if (!isNested)//do not leave in a nested call
                {
                    context.Leave();
                    syncHandler.Set();
                }
                alreadyCalled = false;

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
