using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public interface IObjectHolderService:IService
    {
        T Hold<T>(T obj);
    }
}
