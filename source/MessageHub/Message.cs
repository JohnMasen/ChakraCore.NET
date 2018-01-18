using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub
{
    public class Message
    {
        public string Body { get; set; }
        public string From { get; set; }
        public Stack<string> To { get; private set; } = new Stack<string>();
        public string Type { get; set; }
        public Session Session { get; set; }
        public StringBuilder Route { get; private set; } = new StringBuilder();
        private Action resultCallback;
        public void Next()
        {
            if (To.Count==0)
            {
                resultCallback?.Invoke();
                return;
            }
            From = To.Peek();
            Route.Append($"/{From}");
        }
    }
}
