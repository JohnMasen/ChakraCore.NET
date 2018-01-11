using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    /// <summary>
    /// plugin interface, allows native feature install to javascript context
    /// </summary>
    public interface INativePlugin
    {
        /// <summary>
        /// install the user function to javascript context
        /// </summary>
        /// <param name="context">target context</param>
        void Install(ChakraContext context);
    }
}
