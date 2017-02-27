using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ChakraCore.NET.Core
{
    public interface IEventWaitHandlerService:IService
    {
        EventWaitHandle Handler { get; }
        void WaitOne();
        void Set();
    }
}
