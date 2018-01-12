using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace RunScript
{
    class ScriptConfig
    {
        private string filePath;
        [ConfigKey("Module",true)]
        public bool IsModule { get; set; }
        [ConfigKey("File")]
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
        [ConfigKey("Class")]
        public string ModuleClass { get; set; } = "app";
        [ConfigKey("EntryPoint")]
        public string ModuleEntryPoint { get; set; } = "main";
        [ConfigKey("RootFolder")]
        public string RootFolder { get; set; }
        [ConfigKey("PluginFolder")]
        public string PluginRootFolder { get; set; } = string.Empty;
        [ConfigKey("PluginConfig")]
        public string PluginConfig { get; set; }
        public static ScriptConfig Parse(string[] args)
        {
            ScriptConfig result = new ScriptConfig();
            foreach (var item in args)
            {
                var values = item.Split(':');
                switch (values.Length)
                {
                    case 1:
                        setConfigValueByKey(result, values[0], true);
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
                ConfigKeyAttribute configKey = item.GetCustomAttributes(typeof(ConfigKeyAttribute), false).FirstOrDefault() as ConfigKeyAttribute;
                if (string.Compare(configKey?.Key, propertyKey, true) == 0)
                {
                    if (configKey.IsBoolean && value.GetType()!=typeof(Boolean))
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
