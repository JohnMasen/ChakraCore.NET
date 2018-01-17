using System;
using System.Collections.Concurrent;

namespace MessageHub
{
    public static class MessageHub
    {
        //private ConcurrentDictionary<string,BlockingCollection<Message>>
        public static void SendMessage(Message msg)
        {

        }

        public static void Subscribe(string channelName,Action<Message> callback)
        {

        }
    }
}
