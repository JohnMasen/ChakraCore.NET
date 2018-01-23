using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RunScript
{
    class ScriptConfig
    {
        private string filePath;
        [ConfigKey("Module",true)][ConfigKey("M",true)]
        public bool IsModule { get; set; }
        [ConfigKey("File"),ConfigKey("F")]
        public string File
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
                FileInfo info = new FileInfo(filePath);
                if (string.IsNullOrEmpty(RootFolder))
                {
                    RootFolder = info.DirectoryName;
                }
                FileName = info.Name;
            }
        }

        public string FileName { get; private set; }
        [ConfigKey("Class"), ConfigKey("C")]
        public string ModuleClass { get; set; } = "app";
        [ConfigKey("EntryPoint"), ConfigKey("E")]
        public string ModuleEntryPoint { get; set; } = "main";
        public string RootFolder { get; set; }
        public string PluginRootFolder { get; set; } = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName + "\\Plugins";
        public static ScriptConfig Parse(string[] args)
        {
            ScriptConfig result = new ScriptConfig();
            foreach (var item in args)
            {
                var values = item.Split(':');
                switch (values.Length)
                {
                    case 1:
                        if (values[0].StartsWith('/'))
                        {
                            setConfigValueByKey(result, values[0], true);
                        }
                        else
                        {
                            result.File = values[0];
                        }
                        
                        break;
                    case 2:
                        setConfigValueByKey(result, values[0], values[1]);
                        break;
                    default:
                        throw new ArgumentException($"invalid parameter {item}");
                }

            }
            return result;
        }

        private static void setConfigValueByKey(ScriptConfig target, string propertyKey, object value)
        {
            var properties = target.GetType().GetProperties();
            foreach (var item in properties)
            {
                foreach (ConfigKeyAttribute configKey in item.GetCustomAttributes(typeof(ConfigKeyAttribute), false))
                {
                    if (string.Compare(configKey?.Key, propertyKey, true) == 0)
                    {
                        if (configKey.IsBoolean && value.GetType() != typeof(Boolean))
                        {
                            throw new ArgumentException($"{propertyKey} switch cannot assign value");
                        }
                        item.SetValue(target, value);
                        return;
                    }
                }
            }
        }
    }
}
