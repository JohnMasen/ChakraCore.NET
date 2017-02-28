using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET
{
    public abstract class ServiceBase:IService
    {

        protected IServiceNode serviceNode;

        public IServiceNode CurrentNode
        {
            get { return serviceNode; }
            set
            {
                if (ReferenceEquals(serviceNode,value))
                {
                    return;
                }
                OnServiceNodeSwitch(serviceNode, value);
                serviceNode = value;
            }
        }


        protected IContextSwitchService contextSwitch => serviceNode.GetService<IContextSwitchService>();
        protected IJSValueConverterService converter => serviceNode.GetService<IJSValueConverterService>();
        protected virtual void OnServiceNodeSwitch(IServiceNode before, IServiceNode after)
        {
        }
    }
}
