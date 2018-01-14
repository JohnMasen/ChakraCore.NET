using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Plugin
{
    public class SingleTypeLoader<T> : IPluginLoader where T:INativePluginInstaller,new()
    {
        public string Name { get; private set; }
        public SingleTypeLoader(string name)
        {
            Name = name;
        }
        private T instance;
        public INativePluginInstaller Load(string name)
        {
            if (name!=Name)
            {
                return null;
            }
            if (instance==null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}
