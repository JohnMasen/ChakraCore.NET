using System;
using System.Collections.Generic;
using System.Text;

namespace ChakraCore.NET.Hosting
{
    public class ProtocolResolver<T>:Dictionary<string,Func<string,T>>
    {
        public struct ProtoclResolveResult
        {
            public string Protocol;
            public string Content;
        }
        public string DefaultProtocol { get; private set; }
        public char[] ProtocolKey { get; private set; }
        public ProtocolResolver(string defaultProtocol,string protoclKey="@")
        {
            DefaultProtocol = defaultProtocol;
            ProtocolKey = protoclKey.ToCharArray();
        }
        public T Process(string command)
        {
            if (command==null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var item = Resolve(command);
            if (ContainsKey(item.Protocol))
            {
                return this[item.Protocol](item.Content);
            }
            else
            {
                throw new MissingProtocolProcessorException(item.Protocol);
            }
        }
        public void Add(Func<string,T> value)
        {
            Add(DefaultProtocol, value);
        }
        public ProtoclResolveResult Resolve(string command)
        {
            var tmp = command.Split(ProtocolKey, 2);
            if (tmp.Length == 1)
            {
                return new ProtoclResolveResult() { Protocol = DefaultProtocol, Content = tmp[0] };
            }
            else
            {
                return new ProtoclResolveResult() { Protocol = tmp[0], Content = tmp[1] };
            }
        }
    }
    
}
