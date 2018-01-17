using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub
{
    public class Channel
    {
        public string Name { get; private set; }
        public Channel(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (obj is Channel)
            {
                return Name == (obj as Channel).Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
