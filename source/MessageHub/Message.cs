using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub
{
    public class Message
    {
        public string Body { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Title { get; set; }
        public Session Session { get; set; }
    }
}
