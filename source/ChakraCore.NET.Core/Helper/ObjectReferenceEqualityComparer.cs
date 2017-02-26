using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public class ObjectReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
