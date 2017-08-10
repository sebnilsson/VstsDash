using System;
using System.Collections.Concurrent;

namespace VstsDash
{
    public class KeyedLock : KeyedLock<string>
    {
    }

    public class KeyedLock<T>
    {
        private readonly ConcurrentDictionary<T, object> locks = new ConcurrentDictionary<T, object>();

        public object GetLock(T key)
        {
            return locks.GetOrAdd(key, _ => new object());
        }

        public void RemoveLock(T key)
        {
            object o;
            locks.TryRemove(key, out o);
        }

        public TResult RunWithLock<TResult>(T key, Func<TResult> body)
        {
            lock (locks.GetOrAdd(key, _ => new object()))
            {
                return body();
            }
        }

        public void RunWithLock(T key, Action body)
        {
            lock (locks.GetOrAdd(key, _ => new object()))
            {
                body();
            }
        }
    }
}