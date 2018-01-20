using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public static class ChakraContextHelper
    {
        public static void RunModule(this ChakraContext context,string script,string rootFolder="",params string[] addtionalSearchPattern)
        {
            ModuleLocator locator = new ModuleLocator(rootFolder);
            locator.SearchPatterns.AddRange(addtionalSearchPattern);
            context.RunModule(script, locator.LoadModule);
        }

        [Obsolete("Please use hosting namespace for replacement")]
        public static JSValue ProjectModuleClass(this ChakraContext context,string moduleName,string className,string rootFolder="",params string[] addtionalSearchPattern)
        {
            ModuleLocator locator = new ModuleLocator(rootFolder);
            locator.SearchPatterns.AddRange(addtionalSearchPattern);
            string projectTo = "__ModuleInstance__" +Guid.NewGuid().ToString().Replace('-','_');
            return context.ProjectModuleClass(projectTo, moduleName, className, locator.LoadModule);
        }
    }
}
