using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub
{
    public interface IMessageConsumer
    {
        void ProcessMessage(Message msg);
    }
}
