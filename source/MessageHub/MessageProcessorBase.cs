using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MessageHub
{
    public abstract class MessageProcessorBase
    {
        protected BlockingCollection<Message> queue;
        private CancellationTokenSource source;
        protected MessageProcessorBase(int queueSize,CancellationToken parentToken)
        {
            queue = new BlockingCollection<Message>(queueSize);
            source = CancellationTokenSource.CreateLinkedTokenSource(parentToken);
        }

        protected MessageProcessorBase(int queueSize)
        {
            queue = new BlockingCollection<Message>(queueSize);
            source = new CancellationTokenSource();
        }



        protected abstract void OnMessage(Message msg);
    }
}
