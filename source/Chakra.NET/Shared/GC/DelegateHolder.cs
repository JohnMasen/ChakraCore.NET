using ChakraHost.Hosting;
using System;
using System.Collections.Generic;

namespace Chakra.NET.GC
{
    public class DelegateHolder
    {
        DelegateHolderManager manager;
        List<JavaScriptNativeFunction> callbackList = new List<JavaScriptNativeFunction>();
        InternalHanlder clrHandler;
        bool isTaken = false;
        private bool isActive = true;

        public bool CanReleaseFromInternal { get; private set; }
        public bool CanReleaseFromExternal { get; private set; }
        public bool IsReleasedByInternal { get; private set; }
        public bool IsReleasedByExternal { get; private set; }

        internal DelegateHolder(DelegateHolderManager manager,bool allowInternalRelease, bool allowExternalRelease)
        {
            this.manager = manager;
            CanReleaseFromInternal = allowInternalRelease;
            CanReleaseFromExternal = allowExternalRelease;
            IsReleasedByInternal = !allowInternalRelease;
            IsReleasedByExternal = !allowExternalRelease;
            if (allowInternalRelease)
            {
                clrHandler = new InternalHanlder(this);
            }
            isActive = CanReleaseFromExternal || CanReleaseFromInternal;
        }
        
        public JavaScriptValue CreateCallback(JavaScriptNativeFunction callback)
        {
            if (isActive==false)//I'm root
            {
                throw new InvalidOperationException("cannot register callback at top level");
            }
            var result=JavaScriptValue.CreateFunction(callback);
            callbackList.Add(callback);
            return result;
        }

        public JavaScriptObjectFinalizeCallback GetExternalReleaseDelegate()
        {
            if (!CanReleaseFromExternal)
            {
                throw new InvalidOperationException("Handler does not support release by external");
            }
            if (isTaken)
            {
                throw new InvalidOperationException("External handler has been taken more than once, hanlder should only be collected once");
            }
            isTaken = true;
            return releseFromExternal;
        }

        public InternalHanlder GetInternalHandler()
        {
            if (CanReleaseFromInternal)
            {
                return clrHandler;
            }
            else
            {
                throw new InvalidOperationException("Holder does not support release from internal");
            }
        }
        private void releaseFromInternal()
        {
            IsReleasedByInternal = true;
            release();
        }
        private void releseFromExternal(IntPtr p)
        {
            IsReleasedByExternal = true;
            release();
        }
        private void release()
        {
            if (IsReleasedByExternal && IsReleasedByInternal)
            {
                callbackList.Clear();
                manager.ReleaseHolder(this);
            }
        }

        public class InternalHanlder:IDisposable
        {
            private DelegateHolder holder;
            public InternalHanlder(DelegateHolder parent)
            {
                holder = parent;
            }
            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                        holder.releaseFromInternal();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~InternalHanlder() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }

    }
}
