using ChakraCore.NET.API;
using JSModuleTest.API;
using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET;

namespace JSModuleTest
{
    class ModuleLoader
    {
        private Queue<Action> moduleParseQueue = new Queue<Action>();
        public void Echo(string message)
        {
            Console.WriteLine(message);
        }
        public JavaScriptValue LoadModule(string fileName)
        {
            var rootModule=createModule(null, fileName);
            parseModuleQueue();
            //return JavaScriptValue.Invalid;
            var x= JavaScriptModuleRecord.RunModule(rootModule);
            parseModuleQueue();
            return x;
        }

        public void InitModuleCallback()
        {
            createModule(null, null);
        }

        private void parseModuleQueue()
        {
            while (moduleParseQueue.Count>0)
            {
                moduleParseQueue.Dequeue().Invoke();
            }
        }

        private JavaScriptModuleRecord createModule(JavaScriptModuleRecord? parent,string name)
        {
            var result= API.JavaScriptModuleRecord.Create(parent, name);
            JavaScriptModuleRecord.SetFetchModuleCallback(result, FetchImportedModule);
            JavaScriptModuleRecord.SetFetchModuleScriptCallback(result, FetchImportedModuleFromScript);
            //if (parent==null)//root module, set notify ready callback
            //{
                JavaScriptModuleRecord.SetNotifyReady(result,ModuleNotifyReady);
            //}
            if (name!=null)
            {
                moduleParseQueue.Enqueue(() =>
                {
                    JavaScriptModuleRecord.ParseScript(result, loadScript(name));
                    System.Diagnostics.Debug.WriteLine($"module {name} Parsed");
                });
            }
            
            System.Diagnostics.Debug.WriteLine($"{name} module created");
            return result;
        }
        private string loadScript(string name)
        {
            return System.IO.File.ReadAllText(name);
        }

        
        private JavaScriptErrorCode ModuleNotifyReady(JavaScriptModuleRecord module, JavaScriptValue value)
        {
            System.Diagnostics.Debug.WriteLine("ModuleNotifyReady");
            return JavaScriptErrorCode.NoError;
        }
        private JavaScriptErrorCode FetchImportedModule(JavaScriptModuleRecord reference, JavaScriptValue name, out JavaScriptModuleRecord result)
        {
            System.Diagnostics.Debug.WriteLine($"FetchImportedModule [{name.ToString()}]");
            result = createModule(reference, name.ToString());
            return JavaScriptErrorCode.NoError;
        }

        private JavaScriptErrorCode FetchImportedModuleFromScript(JavaScriptSourceContext sourceContext, JavaScriptValue source, out JavaScriptModuleRecord result)
        {
            System.Diagnostics.Debug.WriteLine("FetchImportedModuleFromScriptDelegate start");
            result = new JavaScriptModuleRecord();
            return JavaScriptErrorCode.NoError;
        }
    }

    public class abc<T> where T:class
    {

    }
}
