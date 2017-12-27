using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.ServiceInterface
{
    public interface IModuleLocatorService:IService
    {
        string LoadModule(string name);
    }
}
