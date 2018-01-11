using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChakraCore.NET
{
    public class ModuleLocator
    {
        private string rootFolder;
        public ModuleLocator(string rootFolder)
        {
            this.rootFolder = rootFolder??string.Empty;
            SearchPatterns = new List<string>
            {
                "{ModuleName}",
                "{ModuleName}.js",
                "{ModuleName}\\{ModuleName}",
                "{ModuleName}\\{ModuleName}.js",
                "{ModuleName}\\index.js"
            };
        }
        public List<string> SearchPatterns { get; private set; }

        public string LoadModule(string name)
        {
            foreach (var item in SearchPatterns)
            {
                FileInfo info = new FileInfo(getModuleFileName(item,name,rootFolder));
                if (info.Exists)
                {
                    try
                    {
                        using (var stream = info.OpenRead())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw new InvalidOperationException($"failed to read module [{name}]",ex);
                    }
                }
            }
            //no match, throw exception
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Cannot found module [{name}] at following location");
            foreach (var item in SearchPatterns)
            {
                sb.AppendLine(getModuleFileName(item, name,rootFolder));
            }
            throw new FileNotFoundException(sb.ToString());
        }
        private string getModuleFileName(string source,string moduleName,string folder)
        {
            return Path.Combine(folder, source.Replace("{ModuleName}", moduleName));
        }
    }

    
}
