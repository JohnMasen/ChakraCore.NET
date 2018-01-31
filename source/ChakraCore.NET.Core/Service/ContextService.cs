using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private BlockingCollection<moduleItem> moduleParseQueue = new BlockingCollection<moduleItem>();
        private Dictionary<string, JavaScriptModuleRecord> moduleCache = new Dictionary<string, JavaScriptModuleRecord>();
        private AutoResetEvent moduleReadyEvent = new AutoResetEvent(false);
        public CancellationTokenSource ContextShutdownCTS { get; private set; }

        public ContextService(CancellationTokenSource shutdownCTS)
        {
            ContextShutdownCTS = shutdownCTS;
            startModuleParseQueue();
        }
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
            JavaScriptModuleRecord rootRecord=contextSwitch.With(() =>
            {
                return createModule(null, null, (name) =>
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
            });
                //startModuleParseQueue();
            moduleReadyEvent.WaitOne();
            contextSwitch.With(() =>
            {
                JavaScriptModuleRecord.RunModule(rootRecord);
                //startModuleParseQueue();//for dynamic import during module run
                //this pattern is not consistent to promise loop pattern, should change it?
            });
            
        }

        private void startModuleParseQueue()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var item = moduleParseQueue.Take(ContextShutdownCTS.Token);
                        contextSwitch.With(item.parse);
                    }
                    catch(OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    
                }
            },ContextShutdownCTS.Token);
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
                moduleReadyEvent.Set();
                if (jsvalue.IsValid)
                {
                    var valueService = CurrentNode.GetService<IJSValueService>();
                    valueService.ThrowIfErrorValue(jsvalue);
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

            moduleParseQueue.Add(new moduleItem(
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

    }
}
