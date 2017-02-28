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
        //private static JavaScriptSourceContext currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        internal JavaScriptContext jsContext;

        private CancellationTokenSource promiseTaskCTS = new CancellationTokenSource();
        private BlockingCollection<JavaScriptValue> promiseTaskQueue = new BlockingCollection<JavaScriptValue>();
        private JavaScriptPromiseContinuationCallback promiseContinuationCallback;
        private JavaScriptValue GlobalObject;
        private ContextService contextService;
        private ContextSwitchService contextSwitch;

        public JSValue RootObject { get; private set; }
        public ChakraRuntime Runtime { get; private set; }

        private bool isDebug;
        internal ChakraContext(JavaScriptContext jsContext, ChakraRuntime runtime):base(runtime.ServiceNode,"ChakraContext")
        {
            jsContext.AddRef();
            this.jsContext = jsContext;
            Runtime = runtime;
        }

        internal void Init(bool enableDebug)
        {
            isDebug = enableDebug;
            contextSwitch = new ContextSwitchService(jsContext);
            ServiceNode.PushService<IContextSwitchService>(contextSwitch);

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

                if (enableDebug)
                {
                    Native.ThrowIfError(Native.JsStartDebugging());
                }

                GlobalObject = JavaScriptValue.GlobalObject;
            Leave();
            
            
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
            return contextSwitch.Enter();
        }

        public bool IsCurrentContext => contextSwitch.IsCurrentContext;

        public void Leave()
        {
            contextSwitch.Leave();

        }


        public string RunScript(string script)
        {
            return contextService.RunScript(script);
        }


        public JavaScriptValue ParseScript(string script)
        {
            return contextService.ParseScript(script);
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
                    contextSwitch.Dispose();
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
