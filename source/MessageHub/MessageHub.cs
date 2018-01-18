using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageHub
{
    public class MessageHub:IMessageHub
    {
        BlockingCollection<Message> queue = new BlockingCollection<Message>();

        public MessageHub()
        {
            Task.Factory.StartNew(processQueue);
        }
        public void AddConsumer(IMessageConsumer consumer)
        {
            
        }

        public void SendMessage(Message msg)
        {

        }

        private void processQueue()
        {
            foreach (var item in queue.GetConsumingEnumerable())
            {

            }
        }
        
    }
}
