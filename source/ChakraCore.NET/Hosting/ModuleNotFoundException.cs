using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class ModuleNotFoundException:Exception
    {
        public ModuleNotFoundException(string moduleName):base($"No module loader for module [{moduleName}]")
        {
        }
    }
}
