using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public interface IContextSwitchService:IService
    {
        void With(Action a);
        T With<T>(Func<T> f);
        bool Enter();
        void Leave();
        bool IsCurrentContext { get; }
    }
}
