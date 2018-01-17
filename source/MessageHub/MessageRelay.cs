using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MessageHub
{
    public abstract class MessageRelay:IMessageConsumer
    {
        public BlockingCollection<Message> Queue { get; private set; }

        public abstract void ProcessMessage(Message msg);
        

        public MessageRelay(int queueSize)
        {
            Queue = new BlockingCollection<Message>(queueSize);
        }

        public abstract void RelayTo(IMessageConsumer client);
    }
}
