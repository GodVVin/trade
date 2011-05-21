using System;

namespace Trade.Core.Stock.Indicators
{
    internal class Simple : IIndicator
    {
        private StockData<StockValue> _data;

        private IIndicator _base;
        private int _period;
        private double _p;

        public Simple(IIndicator baseIndicator, int period)
        {
            if (baseIndicator == null)
                throw new ArgumentNullException("baseIndicator");
            _base = baseIndicator;
            _period = period;
            if (_base.Updatable)
                _base.UpdateEvent += BaseUpdated;
            //UpdateData();
        }

        private void BaseUpdated(object sender, EventArgs e)
        {
            //UpdateData();
            if (UpdateEvent != null)
                UpdateEvent(this, e);
        }

        private void UpdateData()
        {
            if (_data == null)
                _data = new StockData<StockValue>();
            for (var i = 0; i < _base.Count; i++)
            {
                if (i > Count - 1)
                    _data.AddValue(new StockValue {Time = _base[i].Time});
                _data[i].Value = _base[i].Value;
            }
        }

        #region Implementation of IIndicator

        public int Count
        {
            get { return _base.Count; }
        }

        public StockValue this[int index]
        {
            get { return _base[index]; }
        }

        public StockValue Last
        {
            get { return _base[Count - 1]; }
        }

        public void Update()
        {
            UpdateData();
        }

        public string Name
        {
            get { return _base.Name + " Simple"; }
        }

        public bool Updatable
        {
            get { return true; }
        }

        public event EventHandler<EventArgs> UpdateEvent;

        #endregion
    }
}