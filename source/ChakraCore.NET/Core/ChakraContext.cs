using System;
using ChakraCore.NET.API;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;
using ChakraCore.NET.GC;

namespace ChakraCore.NET
{
    public class ChakraContext : IDisposable
    {
        private static JavaScriptSourceContext currentSourceContext = JavaScriptSourceContext.FromIntPtr(IntPtr.Zero);

        //private Dictionary<object, List<object>> holder = new Dictionary<object, List<object>>();
        private static Queue<JavaScriptValue> taskQueue = new Queue<JavaScriptValue>();

        //private Dictionary<Type, object> proxyList = new Dictionary<Type, object>();
        public ProxyMapManager ProxyMapManager { get; private set; } = new ProxyMapManager();

        internal JavaScriptContext jsContext;
        public JSValueConverter ValueConverter { get; set; }

        private AutoResetEvent waitHanlder;

        public JavaScriptValue JSValue_Undefined;
        public JavaScriptValue JSValue_Null;
        public JavaScriptValue JSValue_True;
        public JavaScriptValue JSValue_False;

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

        internal void Init(bool enableDebug)
        {
            isDebug = enableDebug;
            JavaScriptContext.Current = jsContext;//TODO: use With()
            JavaScriptPromiseContinuationCallback promiseContinuationCallback = delegate (JavaScriptValue task, IntPtr callbackState)
            {
                taskQueue.Enqueue(task);
            };

            if (Native.JsSetPromiseContinuationCallback(promiseContinuationCallback, IntPtr.Zero) != JavaScriptErrorCode.NoError)
            {
                throw new InvalidOperationException("failed to setup callback for ES6 Promise");
            }
                

            ValueConverter = new JSValueConverter();
            JSValue_Undefined = JavaScriptValue.Undefined;
            JSValue_Null = JavaScriptValue.Null;
            JSValue_True = JavaScriptValue.True;
            JSValue_False = JavaScriptValue.False;

            RootObject = JSValue.CreateRoot(this);
            JavaScriptContext.Current = JavaScriptContext.Invalid;

        }

        /// <summary>
        /// Create a proxy for dotnet object for dotnet object instance, makes it accessible in javascript
        /// </summary>
        /// <typeparam name="T">dotnet object type</typeparam>
        /// <param name="source">dotnet object instance</param>
        /// <param name="proxyDelegateHandler">delegate handler,handles all function callback delegate to the dotnet object instance</param>
        /// <returns></returns>
        internal JavaScriptValue CreateProxy<T>(T source, out DelegateHandler proxyDelegateHandler) where T:class
        {
            
            var result = ProxyMapManager.ReigsterMap<T>(source, (p, callback) =>

               {
                   return With<JavaScriptValue>(() =>
                   {
                       return JavaScriptValue.CreateExternalObject(p, callback);
                   }
               );
               }
               ,out DelegateHandler handler
            );
            proxyDelegateHandler = handler;
            return result;
        }

        //internal JavaScriptValue CreateExternalArrayBuffer(JSExternalArrayBuffer source) 
        //{

        //    var result = ProxyMapManager.ReigsterMap<JSExternalArrayBuffer>(source, (p, callback) =>

        //    {
        //        return With<JavaScriptValue>(() =>
        //        {
        //            return JavaScriptValue.CreateExternalArrayBuffer(source.data, Convert.ToUInt32(source.memorySize), callback, IntPtr.Zero);
        //        }
        //    );
        //    }
        //       , out DelegateHandler tmp //do not pass back, arraybuffer should not have any callback
        //    );
        //    return result;
        //}

        internal JavaScriptValue CreateArrayBuffer(JSArrayBuffer source)
        {
            return With<JavaScriptValue>(() =>
            {
                switch (source.Source)
                {
                    case ArrayBufferSourceEnum.CreateByJavascript:
                        if (!source.JSSource.IsValid)
                        {
                            throw new InvalidOperationException("source array buffer is unvalid");
                        }
                        return source.JSSource;
                    case ArrayBufferSourceEnum.CreateInJavascript:
                        JavaScriptValue result = JavaScriptValue.CreateArrayBuffer((uint)source.ByteLength);
                        var data = JavaScriptValue.GetArrayBufferStorage(result, out uint bufferSize);
                        if (source.Init!=null)
                        {
                            using (SharedMemoryBuffer buffer = new SharedMemoryBuffer(data, bufferSize))
                            {
                                source.Init(buffer);
                            }
                        }
                        return result;
                    case ArrayBufferSourceEnum.CreateByDotnet:
                    case ArrayBufferSourceEnum.CreateByExternal:
                        return JavaScriptValue.CreateExternalArrayBuffer(source.Handle, (uint)source.ByteLength, null, IntPtr.Zero);//do not handle GC callback, user should control the varient life cycle
                    default:
                        throw new ArgumentOutOfRangeException("Invalid Source property in JSArryBuffer object");
                }
            }
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
