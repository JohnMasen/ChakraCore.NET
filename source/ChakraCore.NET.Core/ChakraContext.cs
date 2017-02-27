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

        internal JavaScriptContext jsContext;

        private EventWaitHandle waitHanlder;

        private CancellationTokenSource promiseTaskCTS = new CancellationTokenSource();
        private BlockingCollection<JavaScriptValue> promiseTaskQueue = new BlockingCollection<JavaScriptValue>();
        private JavaScriptPromiseContinuationCallback promiseContinuationCallback;
        private JavaScriptValue GlobalObject;
        ContextService contextService;
        //public GC.StackTraceNode GCStackTrace { get; private set; }

        public JSValue RootObject { get; private set; }


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

            GlobalObject = JavaScriptValue.GlobalObject;
            Leave();
            
            ServiceNode.PushService<IContextSwitchService>(new ContextSwitchService(jsContext));
            contextService = new ContextService();
            ServiceNode.PushService<IContextService>(contextService);
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
