using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public interface IRuntimeService:IService,IDisposable
    {
        /// <summary>
        /// Internal context, used to create/release javascript values when all user contexts are released
        /// </summary>
        IContextSwitchService InternalContextSwitchService { get; }

        /// <summary>
        /// Force runtime collect garbage
        /// </summary>
        void CollectGarbage();

        /// <summary>
        /// Stop running script without clean the context
        /// </summary>
        void TerminateRuningScript();
    }
}
