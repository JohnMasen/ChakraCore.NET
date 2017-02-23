using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Helper
{
    public class TypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            return x.GetHashCode() - y.GetHashCode();
        }

        public int GetHashCode(Type obj)
        {
            return obj.GetHashCode();
        }
    }
}
