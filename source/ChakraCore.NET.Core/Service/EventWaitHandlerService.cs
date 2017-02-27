using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ChakraCore.NET.Core
{
    public class EventWaitHandlerService : ServiceBase, IEventWaitHandlerService
    {
        public EventWaitHandle Handler { get; private set; } = new AutoResetEvent(false);

        public void Set()
        {
            Handler.Set();
        }

        public void WaitOne()
        {
            Handler.WaitOne();
        }
    }
}
