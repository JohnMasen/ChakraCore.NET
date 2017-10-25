using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ChakraCore.NET.Promise
{
    public abstract class AsyncResultBase:IAsyncResult
    {
        private ManualResetEvent mre = new ManualResetEvent(false);

        public WaitHandle AsyncWaitHandle => mre;

        public bool CompletedSynchronously { get; private set; } = false;

        public bool IsCompleted { get; private set; }

        public object AsyncState => throw new NotImplementedException();

        protected void Release()
        {
            CompletedSynchronously = true;
            IsCompleted = true;
            mre.Set();
        }
    }

    public class AsyncResult<T> : AsyncResultBase
    {
        public T Result { get; private set; }


        public void SetResult(T value)
        {
            Result = value;
            Release();
        }

    }

    public class AsyncResult:AsyncResultBase
    {
        public void SetComplete()
        {
            Release();
        }
    }
}
