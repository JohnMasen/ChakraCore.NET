using System;
using System.Collections.Generic;
using System.Text;
using ChakraCore.NET.API;

namespace ChakraCore.NET
{
    public class ContextService : ServiceBase, IContextService
    {
        private class moduleItem
        {
            public Action parse; // the action to parse the module 
            public List<Delegate> items = new List<Delegate>(); //holds the callback delegates to ensure they're not recycled until script is parsed
            public moduleItem(Action parseAction,params Delegate[] itemsToHold)
            {
                parse = parseAction;
                items.AddRange(itemsToHold);
            }
        }
        private Queue<moduleItem> moduleParseQueue = new Queue<moduleItem>();
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

        public void RunModule(string script,Func<string,string> loadModuleCallback)
        {
            contextSwitch.With(() => {
                var rootRecord = createModule(null, null, (name) =>
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return script;
                    }
                    else
                    {
                        return loadModuleCallback(name);
                    }
                });
                startModuleParseQueue();
                JavaScriptModuleRecord.RunModule(rootRecord);
                startModuleParseQueue();//for dynamic import during module run
            });
            
        }

        private void startModuleParseQueue()
        {
            while (moduleParseQueue.Count > 0)
            {
                moduleParseQueue.Dequeue().parse();
            }
        }

        private JavaScriptModuleRecord createModule(JavaScriptModuleRecord? parent, string name, Func<string, string> loadModuleCallback)
        {
            bool isCreateFromSourceCode = string.IsNullOrEmpty(name);
            
            //if module is cached, return cached value
            if (!isCreateFromSourceCode && moduleCache.ContainsKey(name))
            {
                return moduleCache[name];
            }


            var result = JavaScriptModuleRecord.Create(parent, name);
            #region init moudle callback delegates
            FetchImportedModuleDelegate fetchImported = (JavaScriptModuleRecord reference, JavaScriptValue scriptName, out JavaScriptModuleRecord output) =>
            {
                output= createModule(reference, scriptName.ToString(), loadModuleCallback);
                
                return JavaScriptErrorCode.NoError;
            };

            FetchImportedModuleFromScriptDelegate fetchImportedFromScript = (JavaScriptSourceContext sourceContext, JavaScriptValue scriptName, out JavaScriptModuleRecord output) =>
            {
                output = createModule(null, scriptName.ToString(), loadModuleCallback);
                return JavaScriptErrorCode.NoError;
            };

            NotifyModuleReadyCallbackDelegate notifyReady = (module, jsvalue) =>
            {
                if (jsvalue.IsValid)
                {
                    throw new InvalidOperationException($"Module load failed. message={jsvalue.ToString()}");
                }
                return JavaScriptErrorCode.NoError;
            };
            #endregion


            Action parseModule = () =>
             {
                 JavaScriptModuleRecord.ParseScript(result, loadModuleCallback(name));
                 System.Diagnostics.Debug.WriteLine($"module {name} Parsed");
             };

            JavaScriptModuleRecord.SetFetchModuleCallback(result, fetchImported);
            JavaScriptModuleRecord.SetFetchModuleScriptCallback(result, fetchImportedFromScript);
            JavaScriptModuleRecord.SetNotifyReady(result, notifyReady);

            moduleParseQueue.Enqueue(new moduleItem(
                parseModule,
                fetchImported,
                fetchImportedFromScript, 
                notifyReady));

            if (!isCreateFromSourceCode)
            {
                moduleCache.Add(name, result);//cache the module if it's not directly from RunModule function
            }
            
            
            System.Diagnostics.Debug.WriteLine($"{name} module created");
            return result;
        }

        //private JavaScriptErrorCode ModuleNotifyReady(JavaScriptModuleRecord module, JavaScriptValue value)
        //{
        //    if (value.IsValid)
        //    {
        //        throw new InvalidOperationException($"Module load failed. message={value}");
        //    }
        //    //System.Diagnostics.Debug.WriteLine("ModuleNotifyReady start");
        //    return JavaScriptErrorCode.NoError;
        //}
        //private JavaScriptErrorCode FetchImportedModule(JavaScriptModuleRecord reference, JavaScriptValue name, out JavaScriptModuleRecord result)
        //{
        //    System.Diagnostics.Debug.WriteLine($"FetchImportedModule [{name.ToString()}]");
        //    result = createModule(reference, name.ToString());
        //    return JavaScriptErrorCode.NoError;
        //}

        //private JavaScriptErrorCode FetchImportedModuleFromScript(JavaScriptSourceContext sourceContext, JavaScriptValue name, out JavaScriptModuleRecord result)
        //{
        //    System.Diagnostics.Debug.WriteLine($"FetchImportedModule from script [{name.ToString()}]");
        //    result = createModule(null, name.ToString());
        //    return JavaScriptErrorCode.NoError;
        //}

    }
}
