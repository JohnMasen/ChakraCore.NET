using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Core
{
    public class ObjectHolderService :ServiceBase,IObjectHolderService
    {
        private List<object> objectList = new List<object>();
        public virtual T Hold<T>(T obj)
        {
            objectList.Add(obj);
            return obj;
        }
    }
}
