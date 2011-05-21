using System;

namespace Trade.Core.Stock.Indicators
{
    public class Ema : IIndicator
    {
        private StockData<StockValue> _data;

        private IIndicator _base;
        private int _period;
        private double _p;

        public Ema(IIndicator baseIndicator, int period)
        {
            if (baseIndicator == null)
                throw new ArgumentNullException("baseIndicator");
            _base = baseIndicator;
            if (_base.Updatable)
                _base.UpdateEvent += BaseUpdated;
            SetPeriod(period);
        }

        private void BaseUpdated(object sender, EventArgs e)
        {
            UpdateData();
            if (UpdateEvent != null)
                UpdateEvent(this, e);
        }

        private void SetPeriod(int period)
        {
            if (period < 1)
                throw new ArgumentOutOfRangeException("period");
            _period = period;
            _p = 2.0/(_period + 1);
            _data = new StockData<StockValue>();
            UpdateData();
        }

        private void UpdateData()
        {
            for (int i = 0; i < _base.Count; i++)
            {
                if (i > Count - 1)
                    _data.AddValue(new StockValue {Time = _base[i].Time});
                if (i == 0)
                    _data[i].Value = _base[i].Value;
                else
                {
                    double prevEma = _data[i - 1].Value;
                    _data[i].Value = prevEma + _p*(_base[i].Value - prevEma);
                }
            }
        }

        #region Implementation of IIndicator

        public int Count
        {
            get { return _data.Count; }
        }

        public StockValue this[int index]
        {
            get { return _data[index]; }
        }

        public StockValue Last
        {
            get { return _data[Count - 1]; }
        }

        public void Update()
        {
            UpdateData();
        }

        public string Name
        {
            get { return _base.Name + " EMA " + _period; }
        }

        public bool Updatable
        {
            get { return true; }
        }

        public event EventHandler<EventArgs> UpdateEvent;

        #endregion
    }
}