using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public static class ServiceNodeHelper
    {
        public static void WithContext(this IServiceNode node, Action a)
        {
            node.GetService<IContextSwitchService>().With(a);
        }

        public static T WithContext<T>(this IServiceNode node, Func<T> f)
        {
            return node.GetService<IContextSwitchService>().With<T>(f);
        }
    }
}
