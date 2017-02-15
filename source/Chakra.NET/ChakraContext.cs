using System;
using Chakra.NET.API;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;
using Chakra.NET.GC;
using Microsoft.Extensions.Logging;

namespace Chakra.NET
{
    public class ChakraContext : LoggableObjectBase<ChakraContext>,IDisposable
    {
        private static JavaScriptSourceContext currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        //private Dictionary<object, List<object>> holder = new Dictionary<object, List<object>>();
        private static Queue<JavaScriptValue> taskQueue = new Queue<JavaScriptValue>();

        private Dictionary<Type, object> proxyList = new Dictionary<Type, object>();

        internal JavaScriptContext jsContext;
        public JSValueConverter ValueConverter { get; set; }

        private AutoResetEvent waitHanlder;

        public JavaScriptValue JSValue_Undefined;
        public JavaScriptValue JSValue_Null;

        //public GC.StackTraceNode GCStackTrace { get; private set; }

        internal JavaScriptValue GlobalObject
        {
            get
            {
                return With<JavaScriptValue>(() =>
                {
                    JavaScriptValue result;
                    result = JavaScriptValue.GlobalObject;
                    return result;
                }
                );
            }
        }

        public JSValue RootObject { get; private set; }

        private bool isDebug;
        internal ChakraContext(JavaScriptContext jsContext, AutoResetEvent syncHandler)
        {
            this.jsContext = jsContext;
            this.waitHanlder = syncHandler;
        }

        internal void Init(bool enableDebug, params string[] winRTNamespace)
        {
            isDebug = enableDebug;
            JavaScriptContext.Current = jsContext;//TODO: use With()
            JavaScriptPromiseContinuationCallback promiseContinuationCallback = delegate (JavaScriptValue task, IntPtr callbackState)
            {
                taskQueue.Enqueue(task);
            };

            if (Native.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero) != JavaScriptErrorCode.NoError)
                throw new InvalidOperationException("failed to setup callback for ES6 Promise");
            foreach (var item in winRTNamespace)
            {
                if (Native.JsProjectWinRTNamespace(item) != JavaScriptErrorCode.NoError)
                    throw new InvalidOperationException($"failed to project {item} namespace.");
            }

            if (isDebug && Native.JsStartDebugging() != JavaScriptErrorCode.NoError)
                throw new InvalidOperationException("failed to start debugging.");

            ValueConverter = new JSValueConverter();
            JSValue_Undefined = JavaScriptValue.Undefined;
            JSValue_Null = JavaScriptValue.Null;

            RootObject = JSValue.CreateRoot(this);
            JavaScriptContext.Current = JavaScriptContext.Invalid;

        }


        internal JavaScriptValue CreateProxy<T>(T source, out DelegateHandler proxyDelegateHandler)
        {

            Dictionary<T, Tuple<JavaScriptValue, DelegateHandler, JavaScriptObjectFinalizeCallback>> currentList;
            #region get or create a proxy map item
            if (proxyList.ContainsKey(typeof(T)))
            {
                currentList = (proxyList[typeof(T)] as Dictionary<T, Tuple<JavaScriptValue, DelegateHandler, JavaScriptObjectFinalizeCallback>>);
                if (currentList == null)
                {
                    throw new InvalidOperationException("internal error, delegate handler corrupted");
                }
            }
            else
            {
                currentList = new Dictionary<T, Tuple<JavaScriptValue, DelegateHandler, JavaScriptObjectFinalizeCallback>>();
                proxyList.Add(typeof(T), currentList);
            }
            DelegateHandler handler;
            if (currentList.ContainsKey(source))
            {
                proxyDelegateHandler = null;
                return currentList[source].Item1;//proxy already created, directly return
            }
            else
            {
                handler = new DelegateHandler();
            }
            #endregion

            var objHandle = GCHandle.Alloc(source);

            JavaScriptObjectFinalizeCallback callback = (p) =>
              {
                  T key = (T)GCHandle.FromIntPtr(p).Target;
                  var list = proxyList[typeof(T)] as Dictionary<T, Tuple<JavaScriptValue, DelegateHandler, JavaScriptObjectFinalizeCallback>>;
                  list.Remove(key);
              };
            JavaScriptValue result = With<JavaScriptValue>(() =>
              {
                  return JavaScriptValue.CreateExternalObject((IntPtr)objHandle, callback);
              }
            );
            currentList.Add(source, new Tuple<JavaScriptValue, DelegateHandler, JavaScriptObjectFinalizeCallback>(result, handler, callback));
            proxyDelegateHandler = handler;
            return result;
        }




        /// <summary>
        /// try switch context to current thread
        /// </summary>
        /// <returns>true if release is required, false if context already running at current thread(no release call required)</returns>
        public bool Enter()
        {
            lock (waitHanlder)//thread safe for status check
            {
                if (IsCurrentContext)
                {
                    log.LogDebug("nested enter, do nothing");
                    return false;//no operation required
                }
            }
            log.LogDebug("wait otheres release context");
            waitHanlder.WaitOne();//wait other call complete
            JavaScriptContext.Current = jsContext;
            log.LogDebug("context switch complete");
            return true;
        }

        public bool IsCurrentContext
        {
            get
            {
                return JavaScriptContext.Current == jsContext;
            }
        }

        public void Leave()
        {
            JavaScriptContext.Current = JavaScriptContext.Invalid;
            waitHanlder.Set();
        }

        public void With(Action a)
        {
            if (Enter())
            {
                a();
                Leave();
            }
            else
            {
                a();
            }
        }

        public T With<T>(Func<T> f)
        {
            if (Enter())
            {
                T tmp = f();
                Leave();
                return tmp;
            }
            else
            {
                return f();
            }
        }


        public string RunScript(string script)
        {
            JavaScriptValue result;
            return With<string>(() =>
            {
                if (isDebug)
                {
                    result = JavaScriptContext.RunScript(script, currentSourceContext++, string.Empty);
                }
                else
                {
                    result = JavaScriptContext.RunScript(script);
                }
                return result.ConvertToString().ToString();
            });
        }


        public JavaScriptValue ParseScript(string script)
        {
            return With<JavaScriptValue>(() =>
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
            );
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
