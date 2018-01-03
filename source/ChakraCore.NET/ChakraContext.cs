using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;
using ChakraCore.NET.API;


namespace ChakraCore.NET
{
    /// <summary>
    /// A helper class wraps the key feature of chakracore context
    /// </summary>
    public class ChakraContext : ServiceConsumerBase, IDisposable
    {
        //private static JavaScriptSourceContext currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        internal JavaScriptContext jsContext;
        private EventWaitHandle syncHandle;
        private CancellationTokenSource promiseTaskCTS = new CancellationTokenSource();
        private BlockingCollection<JavaScriptValue> promiseTaskQueue = new BlockingCollection<JavaScriptValue>();
        private JavaScriptPromiseContinuationCallback promiseContinuationCallback;
        private JavaScriptValue JSGlobalObject;
        private ContextService contextService;
        private ContextSwitchService contextSwitch;

        /// <summary>
        /// The global object of a context, it is the root of everything inside the context
        /// <para>A context is like an isolated class in javascript, everything directly defined in script is a property of the root object</para>
        /// </summary>
        public JSValue GlobalObject { get; private set; }

        /// <summary>
        /// The ChakraRuntime object this context belongs to
        /// </summary>
        public ChakraRuntime Runtime { get; private set; }

        private bool isDebug;
        internal ChakraContext(JavaScriptContext jsContext, ChakraRuntime runtime,EventWaitHandle handle):base(runtime.ServiceNode,"ChakraContext")
        {
            jsContext.AddRef();
            this.jsContext = jsContext;
            Runtime = runtime;
            syncHandle = handle;
        }

        internal void Init(bool enableDebug)
        {
            isDebug = enableDebug;
            contextSwitch = new ContextSwitchService(jsContext, syncHandle);
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

                

            JSGlobalObject = JavaScriptValue.GlobalObject;
                GlobalObject = new JSValue(ServiceNode, JSGlobalObject);
            Leave();
            
            
            contextService = new ContextService();
            ServiceNode.PushService<IContextService>(contextService);
            GlobalObject.InitTimer();
            
        }


        private void StartPromiseTaskLoop(CancellationToken token)
        {
            Task.Factory.StartNew((Action)(() =>
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
                        task.CallFunction((JavaScriptValue)this.JSGlobalObject);
                        Leave();
                        Debug.WriteLine("Promise task complete");
                    }
                })
                , token
                );
        }
        
        /// <summary>
        /// Try switch context to current thread
        /// </summary>
        /// <returns>true if release is required, false if context already running at current thread(no release call required)</returns>
        public bool Enter()
        {
            return ServiceNode.GetService<IContextSwitchService>().Enter();
        }

        /// <summary>
        /// If chakracore is running at current thread
        /// <para>True if context is running at current thread, otherwise false</para>
        /// </summary>
        public bool IsCurrentContext => ServiceNode.GetService<IContextSwitchService>().IsCurrentContext;

        /// <summary>
        /// Release the context from current thread, this method should be called before you call <see cref="Enter"/> on another thread
        /// </summary>
        public void Leave()
        {
            ServiceNode.GetService<IContextSwitchService>().Leave();

        }

        /// <summary>
        /// Execute a javascript and returns the script result in string
        /// </summary>
        /// <param name="script">Script text</param>
        /// <returns>Script running result</returns>
        public string RunScript(string script)
        {
            return ServiceNode.GetService<IContextService>().RunScript(script);
        }

        /// <summary>
        /// Execute a ES6 module
        /// </summary>
        /// <param name="script">script content</param>
        /// <param name="loadModuleCallback">callback to load imported script content</param>
        public void RunModule(string script,Func<string,string> loadModuleCallback)
        {
            ServiceNode.GetService<IContextService>().RunModule(script, loadModuleCallback);
        }
        /// <summary>
        /// Load a module script, create an instance of specified exported class and map it as a global variable
        /// </summary>
        /// <param name="projectTo">the global variable name mapped to</param>
        /// <param name="moduleName">module name</param>
        /// <param name="className">class name to create an instance</param>
        /// <param name="loadModuleCallback">local module script by name callback </param>
        /// <returns>the mapped value</returns>
        public JSValue ProjectModuleClass(string projectTo, string moduleName, string className, Func<string, string> loadModuleCallback)
        {
            string script_setRootObject = $"var {projectTo}={{}}";
            string script_importModule = $"import {{{className}}} from '{moduleName}'; {projectTo}=new {className}();";
            RunScript(script_setRootObject);
            RunModule(script_importModule, loadModuleCallback);
            return GlobalObject.ReadProperty<JSValue>(projectTo);
        }

        /// <summary>
        /// Parses a script and returns a function representing the script. 
        /// </summary>
        /// <remarks>The script will be wrapped into a javascript function, then return to the caller. Useful for support moduling in javascript
        /// </remarks>
        /// <param name="script">Script text</param>
        /// <returns>A javascript function in <see cref="JavaScriptValue"/></returns>
        public JavaScriptValue ParseScript(string script)
        {
            return ServiceNode.GetService<IContextService>().ParseScript(script);
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
