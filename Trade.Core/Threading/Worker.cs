using System;
using System.Threading;
using Trade.Core.Logging;

namespace Trade.Core.Threading
{
    public class Worker<T> where T : EventArgs
    {
        public event EventHandler<T> NewEvent;

        public Worker(string name)
        {
            _name = name;
            _thread = new Thread(DoWork);
        }

        public void Start()
        {
            lock (_locker)
            {
                if (_running) return;
                Log.Instance.Trace("Starting {0} thread", _name);
                _thread.IsBackground = true;
                _running = true;
                _thread.Start();
            }
        }

        public void Stop()
        {
            lock (_locker) _running = false;
            AddEvent(null);
            _thread.Join();
        }

        public void AddEvent(T evt)
        {
            if (!_running) Log.Instance.Warn("New event {0} on stopped worker {1}", evt, _name);
            _events.Enqueue(evt);
            bool result;
            lock (_locker)
                result = _newDataAdded.Set();
            if (!result) Log.Instance.Warn("AutoResetEvent.Set() returns false.");
        }

        private readonly string _name;

        private readonly Thread _thread;

        private readonly SyncQueue<T> _events = new SyncQueue<T>();

        private readonly object _locker = new object();

        private readonly AutoResetEvent _newDataAdded = new AutoResetEvent(false);

        private bool _running;

        private int _eventsSum;

        private T GetEvent()
        {
            while (_events.IsEmpty())
                _newDataAdded.WaitOne();

            return _events.Dequeue();
        }

        private void DoWork()
        {
            Log.Instance.Trace("Event thread {0} started", _name);

            while (_running)
            {
                var evt = GetEvent();
                if (evt != null)
                {
                    var queueLength = _events.Count;
                    if (queueLength > 0)
                        Log.Instance.Warn("Events queue {0} length: {1}", _name, queueLength);

                    if (NewEvent != null)
                        NewEvent(null, evt);

                    _eventsSum++;
                }
            }

            Log.Instance.Trace("Event thread {0} stopped", _name);
        }
    }
}
