using System;
using System.Collections.Generic;
using System.Text;

namespace MessageHub
{
    public class Session
    {
        public Dictionary<string,object> Data { get; private set; }

        public bool SetData(string key, object value)
        {
            bool result = Data.ContainsKey(key);
            if (result)
            {
                Data[key] = value;
            }
            else
            {
                Data.Add(key, value);
            }
            return result;
        }

        public T GetData<T>(string key)where T:class
        {
            if (Data.ContainsKey(key))
            {
                return Data[key] as T;
            }
            else
            {
                return default(T);
            }
        }

        public T GetData<T>(string key,T defaultValue) where T : struct
        {
            if (Data.ContainsKey(key))
            {
                return (T)Data[key];
            }
            else
            {
                return defaultValue;
            }
        }

        public void RemoveData(string key)
        {
            if (Data.ContainsKey(key))
            {
                Data.Remove(key);
            }
        }
    }
}
