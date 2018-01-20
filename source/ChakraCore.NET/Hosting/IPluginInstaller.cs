using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public interface IPluginInstaller
    {
        void Install(JSValue target);
    }
}
