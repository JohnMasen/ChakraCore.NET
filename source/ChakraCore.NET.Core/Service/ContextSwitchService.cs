using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public class ContextSwitchService :ServiceBase, IContextSwitchService
    {
        //IJSValueConverter converter;
        ChakraContext context;
        public ContextSwitchService(ChakraContext context)
        {
            this.context = context;
        }
        //public override void SetupNode(IServiceNode service)
        //{
        //    base.SetupNode(service);
        //    if (converter == null)
        //    {
        //        converter = serviceNode.GetService<IJSValueConverter>();
        //    }
        //}

        public void With(Action a)
        {
            if(context.Enter())
            {
                a();
                context.Leave();
            }
            else
            {
                a();
            }
        }

        public T With<T>(Func<T> f)
        {
            T result;
            if (context.Enter())
            {
                result=f();
                context.Leave();
            }
            else
            {
                result = f();
            }
            return result;
        }
    }
}
