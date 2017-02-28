using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public interface IService
    {
        IServiceNode CurrentNode { get; set; }
    }
}
