using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public class JSRequireLoader
    {
        public string RootPath { get; set; } = string.Empty;
        public string LoadLib(string name)
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory()+"\\"+ RootPath);
            
            string fileName = name + ".js";
            var files = info.GetFiles(fileName);
            if (files.Length==1)
            {
                return files[0].OpenText().ReadToEnd();
            }
            else
            {
                throw new System.IO.FileNotFoundException("cannot find rquired js file", fileName);
            }
        }

        public static void EnableRequire(ChakraContext context,string rootPath=null)
        {
            JSRequireLoader loader = new JSRequireLoader() { RootPath = rootPath };
            context.GlobalObject.Binding.SetFunction<string, string>("_loadLib", loader.LoadLib);
            context.RunScript(Properties.Resources.ResourceManager.GetString("JSRequire"));
        }
    }
}
