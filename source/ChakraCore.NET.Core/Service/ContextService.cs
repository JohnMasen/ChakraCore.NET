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
        private Exception moduleLoadException;
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
            moduleLoadException = null;
            JavaScriptModuleRecord rootRecord = contextSwitch.With(() =>
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
            throwIfExceptionInLoading();
            contextSwitch.With(() =>
            {
                JavaScriptModuleRecord.RunModule(rootRecord);
            });

        }

        private void throwIfExceptionInLoading()
        {
            var ex = moduleLoadException;
            if (ex != null)
            {
                moduleLoadException = null;
                throw ex;
            }
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
                        if (moduleLoadException==null)//if previous exception is not handled, empty the queue
                        {
                            contextSwitch.With(item.parse);
                        }
                        
                    }
                    catch(OperationCanceledException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        moduleLoadException = ex;
                        moduleReadyEvent.Set();
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

            IRuntimeDebuggingService debugService = CurrentNode.GetService<IRuntimeDebuggingService>();
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
                debugService.ScriptReady();
                return JavaScriptErrorCode.NoError;
            };
            #endregion


            Action parseModule = () =>
             {
                 string script = loadModuleCallback(name);
                 JavaScriptModuleRecord.ParseScript(result,script );
                 debugService.AddScriptSource(name,script);
                 System.Diagnostics.Debug.WriteLine($"module {name} Parsed");
             };

            JavaScriptModuleRecord.SetFetchModuleCallback(result, fetchImported);
            JavaScriptModuleRecord.SetFetchModuleScriptCallback(result, fetchImportedFromScript);
            JavaScriptModuleRecord.SetNotifyReady(result, notifyReady);
            if (!string.IsNullOrEmpty(name))
            {
                JavaScriptModuleRecord.SetHostUrl(result, name);
            }
            

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
