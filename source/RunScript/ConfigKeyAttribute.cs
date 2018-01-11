using System;
using System.Collections.Generic;
using System.Text;

namespace RunScript
{
    class ConfigKeyAttribute:Attribute
    {
        public string Key { get; private set; }
        public ConfigKeyAttribute(string key)
        {
            Key = "/"+key;
        }
    }
}
