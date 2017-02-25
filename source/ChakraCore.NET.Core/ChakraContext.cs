using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;
using ChakraCore.NET.Core.API;

namespace ChakraCore.NET.Core
{
    public class ChakraContext : ServiceConsumerBase, IDisposable
    {
        private static JavaScriptSourceContext currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        //private Dictionary<object, List<object>> holder = new Dictionary<object, List<object>>();
        //private static Queue<JavaScriptValue> taskQueue = new Queue<JavaScriptValue>();

        //private Dictionary<Type, object> proxyList = new Dictionary<Type, object>();

        internal JavaScriptContext jsContext;

        private EventWaitHandle waitHanlder;

        private CancellationTokenSource promiseTaskCTS = new CancellationTokenSource();
        private BlockingCollection<JavaScriptValue> promiseTaskQueue = new BlockingCollection<JavaScriptValue>();
        private JavaScriptPromiseContinuationCallback promiseContinuationCallback;
        public JavaScriptValue JSValue_Undefined;
        public JavaScriptValue JSValue_Null;
        public JavaScriptValue JSValue_True;
        public JavaScriptValue JSValue_False;
        public JavaScriptValue GlobalObject;

        //public GC.StackTraceNode GCStackTrace { get; private set; }

        


        private bool isDebug;
        internal ChakraContext(JavaScriptContext jsContext, EventWaitHandle syncHandle,IServiceNode service):base(service,"ChakraContext")
        {
            jsContext.AddRef();
            this.jsContext = jsContext;
            this.waitHanlder = syncHandle;
        }

        internal void Init(bool enableDebug)
        {
            isDebug = enableDebug;
            Enter();
            promiseContinuationCallback = delegate (JavaScriptValue task, IntPtr callbackState)
            {
                promiseTaskQueue.Add(task);
            };

            if (Native.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero) != JavaScriptErrorCode.NoError)
            {
                throw new InvalidOperationException("failed to setup callback for ES6 Promise");
            }
            StartPromiseTaskLoop(promiseTaskCTS.Token);

            //Native.JsSetObjectBeforeCollectCallback()


            JSValue_Undefined = JavaScriptValue.Undefined;
            JSValue_Null = JavaScriptValue.Null;
            JSValue_True = JavaScriptValue.True;
            JSValue_False = JavaScriptValue.False;
            GlobalObject = JavaScriptValue.GlobalObject;
            Leave();

        }


        private void StartPromiseTaskLoop(CancellationToken token)
        {
            Task.Factory.StartNew(
                ()=>
                {
                    Debug.WriteLine("Promise task loop started");
                    while (true)
                    {
                        JavaScriptValue task;
                        try
                        {
                            task = promiseTaskQueue.Take(token);
                            Debug.WriteLine("Promise task taken");
                        }
                        catch(OperationCanceledException)
                        {
                            Debug.WriteLine("Promise task stop");
                            return;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        Enter();
                        task.CallFunction(GlobalObject);
                        Leave();
                        Debug.WriteLine("Promise task complete");
                    }
                }
                ,token
                );
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
                    return false;//no operation required
                }
            }
            waitHanlder.WaitOne();//wait other call complete
            JavaScriptContext.Current = jsContext;
#if DEBUG
            Debug.WriteLine("context enter"); 
#endif
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
#if DEBUG
            Debug.WriteLine("context leave");
#endif

        }

        //public void With(Action a)
        //{
        //    if (Enter())
        //    {
                
        //        a();
        //        Leave();
        //    }
        //    else
        //    {
        //        a();
        //    }
        //}

        
        

        //public T With<T>(Func<T> f)
        //{
        //    if (Enter())
        //    {
        //        T tmp = f();
        //        Leave();
        //        return tmp;
        //    }
        //    else
        //    {
        //        return f();
        //    }
        //}



        public string RunScript(string script)
        {
            JavaScriptValue result;
            Enter();
            if (isDebug)
            {
                result = JavaScriptContext.RunScript(script, currentSourceContext++, string.Empty);
            }
            else
            {
                result = JavaScriptContext.RunScript(script);
            }
            Leave();
            return result.ConvertToString().ToString();
        }


        public JavaScriptValue ParseScript(string script)
        {
            Enter();
            JavaScriptValue result;
            if (isDebug)
            {
                result = JavaScriptContext.ParseScript(script, currentSourceContext++, string.Empty);
            }
            else
            {
                result = JavaScriptContext.ParseScript(script);
            }
            Leave();
            return result;
        }






        

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    promiseTaskCTS.Cancel();
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
