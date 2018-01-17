using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageHub
{
    public class Listener
    {
        public BlockingCollection<Message> Queue { get; private set; }
        
        public string ChannelName { get; private set; }
        public Listener(string channelName,Action<Message> onMessageCallBack,CancellationToken token)
        {
            ChannelName = channelName;
            Queue = new BlockingCollection<Message>(1);
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var msg=Queue.Take(token);
                    onMessageCallBack(msg);
                }

            },token);
        }

    }
}
