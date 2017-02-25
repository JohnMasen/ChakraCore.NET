using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public class TypeComparer : IComparer<Type>
    {
        public static TypeComparer Instance = new TypeComparer();
        private TypeComparer()
        {

        }
        public int Compare(Type x, Type y)
        {
            return x?.GetHashCode()??0 - y?.GetHashCode()??0;
        }

        public static bool AreSameGenericType<T1,T2>()
        {
            return typeof(T1) == typeof(T2);
        }

    }
}
