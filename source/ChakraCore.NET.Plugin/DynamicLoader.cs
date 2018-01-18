using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Plugin
{
    public class DynamicLoader : IPluginLoader
    {
        private Func<INativePluginInstaller> getInstaller;
        private string name;
        public DynamicLoader(string name,Func<INativePluginInstaller> createInstallerCallback)
        {
            getInstaller = createInstallerCallback ?? throw new ArgumentNullException(nameof(createInstallerCallback));
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }
            this.name = name;
        }
        public INativePluginInstaller Load(string name)
        {
            if (name==this.name)
            {
                return getInstaller();
            }
            else
            {
                return null;
            }
        }
    }
}
