using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public interface IRuntimeService:IService,IDisposable
    {
        IContextSwitchService InternalContextSwitchService { get; }
        void CollectGarbage();
        void TerminateRuningScript();
    }
}
