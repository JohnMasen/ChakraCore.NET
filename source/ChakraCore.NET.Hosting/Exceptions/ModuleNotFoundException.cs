using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class ModuleNotFoundException: HostingException
    {
        public ModuleNotFoundException(string moduleName,string protocol):base($"module [{moduleName}] not found, protocol={protocol}")
        {
        }
    }
}
