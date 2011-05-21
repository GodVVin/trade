using System.Collections.Generic;

namespace Trade.Core.Threading
{
    public class SyncQueue<T>
    {
        #region Public fields

        public int Count
        {
            get { lock (_locker) return _data.Count; }
        }

        public T Dequeue()
        {
            lock (_locker) return _data.Dequeue();
        }

        public void Enqueue(T value)
        {
            lock (_locker) _data.Enqueue(value);
        }

        public bool IsEmpty()
        {
            lock (_locker) return _data.Count == 0;
        }

        #endregion

        #region Private fields

        private readonly Queue<T> _data = new Queue<T>();
        private readonly object _locker = new object();

        #endregion
    }
}