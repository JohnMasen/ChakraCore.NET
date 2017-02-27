using ChakraCore.NET.Core.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public class ContextSwitchService :ServiceBase, IContextSwitchService
    {
        JavaScriptContext context;
        IEventWaitHandlerService syncService => serviceNode.GetService<IEventWaitHandlerService>();
        public ContextSwitchService(JavaScriptContext context)
        {
            this.context = context;
        }


        public bool IsCurrentContext => JavaScriptContext.Current==context;

        public bool Enter()
        {
            if (IsCurrentContext)
            {
                return false;
            }
            else
            {
                syncService.WaitOne();
                JavaScriptContext.Current = context;
                return true;
            }
        }

        public void Leave()
        {
            JavaScriptContext.Current = JavaScriptContext.Invalid;
            syncService.Set();
        }

        public void With(Action a)
        {
            if(Enter())
            {
                a();
                Leave();
            }
            else
            {
                a();
            }
        }

        public T With<T>(Func<T> f)
        {
            T result;
            if (Enter())
            {
                result=f();
                Leave();
            }
            else
            {
                result = f();
            }
            return result;
        }
    }
}
