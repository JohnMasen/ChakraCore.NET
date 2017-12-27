using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class ContextService : ServiceBase, IContextService
    {
        private Queue<Action> moduleParseQueue = new Queue<Action>();
        private Dictionary<string, JavaScriptModuleRecord> moduleCache = new Dictionary<string, JavaScriptModuleRecord>();
        public JavaScriptValue ParseScript(string script)
        {
            return contextSwitch.With<JavaScriptValue>(() =>
            {
                return JavaScriptContext.ParseScript(script);
            });
        }

        public string RunScript(string script)
        {
            return contextSwitch.With<string>(() =>
              {
                  var result= JavaScriptContext.RunScript(script);
                  return result.ConvertToString().ToString();
              });
        }

        public JavaScriptValue RunModule(string script)
        {
            var rootRecord = createModule(null, null, script);
            startModuleParseQueue();
            return JavaScriptModuleRecord.RunModule(rootRecord);
        }

        private void startModuleParseQueue()
        {
            while (moduleParseQueue.Count > 0)
            {
                moduleParseQueue.Dequeue().Invoke();
            }
        }

        private JavaScriptModuleRecord createModule(JavaScriptModuleRecord? parent, string name,string scriptContent=null)
        {
            bool isCreateFromSourceCode = !string.IsNullOrEmpty(scriptContent);
            
            //if scriptContent is passed, parse directly with out query cache
            if (!isCreateFromSourceCode && moduleCache.ContainsKey(name))
            {
                return moduleCache[name];
            }
            var result = API.JavaScriptModuleRecord.Create(parent, name);
            JavaScriptModuleRecord.SetFetchModuleCallback(result, FetchImportedModule);
            JavaScriptModuleRecord.SetFetchModuleScriptCallback(result, FetchImportedModuleFromScript);
            JavaScriptModuleRecord.SetNotifyReady(result, ModuleNotifyReady);
            if (isCreateFromSourceCode) 
            {
                //script is directly passed in parameter, possible root application
                moduleParseQueue.Enqueue(() =>
                {
                    JavaScriptModuleRecord.ParseScript(result, scriptContent);
                    System.Diagnostics.Debug.WriteLine($"module {name} Parsed");
                });
                moduleCache.Add(name, result);
            }
            else 
            {
                //script should be load from external source
                moduleParseQueue.Enqueue(() =>
                {
                    JavaScriptModuleRecord.ParseScript(result, serviceNode.GetService<ServiceInterface.IModuleLocatorService>().LoadModule(name));
                    System.Diagnostics.Debug.WriteLine($"module {name} Parsed");
                });
            }
            
            System.Diagnostics.Debug.WriteLine($"{name} module created");
            return result;
        }

        private JavaScriptErrorCode ModuleNotifyReady(JavaScriptModuleRecord module, JavaScriptValue value)
        {
            if (value.IsValid)
            {
                throw new InvalidOperationException($"Module load failed. message={value}");
            }
            //System.Diagnostics.Debug.WriteLine("ModuleNotifyReady start");
            return JavaScriptErrorCode.NoError;
        }
        private JavaScriptErrorCode FetchImportedModule(JavaScriptModuleRecord reference, JavaScriptValue name, out JavaScriptModuleRecord result)
        {
            System.Diagnostics.Debug.WriteLine($"FetchImportedModule [{name.ToString()}]");
            result = createModule(reference, name.ToString());
            return JavaScriptErrorCode.NoError;
        }

        private JavaScriptErrorCode FetchImportedModuleFromScript(JavaScriptSourceContext sourceContext, JavaScriptValue name, out JavaScriptModuleRecord result)
        {
            System.Diagnostics.Debug.WriteLine($"FetchImportedModule from script [{name.ToString()}]");
            result = createModule(null, name.ToString());
            return JavaScriptErrorCode.NoError;
        }

    }
}
