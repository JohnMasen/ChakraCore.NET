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
            jsContext.AddRef();
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
                if (source.JSSource.IsValid)
                {
                    return source.JSSource;
                }
                switch (source.BufferSource)
                {
                    case SharedBufferSourceEnum.CreateByJavascript:
                        throw new InvalidOperationException("invalid source array buffer");//create by javascript should already have JavaScriptValue assigned
                    case SharedBufferSourceEnum.CreateInJavascript:
                        {
                            JavaScriptValue result = JavaScriptValue.CreateArrayBuffer((uint)source.Size);
                            source.SetJSSource(result, this);//hold the varient
                            var data = JavaScriptValue.GetArrayBufferStorage(result, out uint bufferSize);
                            source.InitWindow(data, false);
                            source.InitValue(source.Buffer);
                            return result;
                        }
                    case SharedBufferSourceEnum.CreateByDotnet:
                    case SharedBufferSourceEnum.CreateByExternal:
                        {
                            var result = JavaScriptValue.CreateExternalArrayBuffer(source.Buffer.Handle, (uint)source.Buffer.ByteLength, null, IntPtr.Zero);//do not handle GC callback, user should control the varient life cycle
                            source.SetJSSource(result,this);//hold the varient
                            return result;
                        }
                        
                    default:
                        throw new ArgumentOutOfRangeException("Invalid BufferSource property in JSArryBuffer object");
                }
            }
                );
        }



        internal JavaScriptValue CreateTypedArray(JSTypedArray source)
        {
            if (source.JSSource.IsValid)
            {
                return source.JSSource;
            }
            switch (source.BufferSource)
            {
                case SharedBufferSourceEnum.CreateByDotnet:
                    return With<JavaScriptValue>(
                        () =>
                        {
                            var result= JavaScriptValue.CreateTypedArray(source.ArrayType, source.JSSource, source.Position, source.UnitCount);
                            source.SetJSSource(result,this);
                            return result;
                        }
                        );
                case SharedBufferSourceEnum.CreateByJavascript:
                    throw new InvalidOperationException("invalid source typed array");//create by javascript should already have JavaScriptValue assigned
                case SharedBufferSourceEnum.CreateInJavascript:
                    return With<JavaScriptValue>(
                        () =>
                        {
                            var result= JavaScriptValue.CreateTypedArray(source.ArrayType, JavaScriptValue.Invalid , source.Position, source.UnitCount);
                            source.SetJSSource(result, this);//hold the objec
                            //get the internal storage
                            JavaScriptValue.GetTypedArrayStorage(result, out IntPtr data, out uint bufferLength, out JavaScriptTypedArrayType type, out int elementSize);
                            source.InitWindow(data, false);
                            source.InitValue?.Invoke(source.Buffer);
                            return result;
                        }
                        );
                case SharedBufferSourceEnum.CreateByExternal:
                    throw new ArgumentException("TypedArray does not support create from external");
                default:
                    throw new ArgumentOutOfRangeException("Invalid BufferSource property in JSTypedArray object");
            }
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
