using System;

namespace Trade.Core.Stock.Indicators
{
    public abstract class BarsSingleValues : IIndicator
    {
        protected Bars Bars;

        protected abstract double GetValue(int index);

        private StockData<StockValue> _data = new StockData<StockValue>();

        protected BarsSingleValues(Bars bars)
        {
            Bars = bars;
            Bars.UpdatedEvent += BarsUpdatedEvent;
        }


        private void BarsUpdatedEvent(object sender, EventArgs e)
        {
            UpdateData();
            if (UpdateEvent != null)
                UpdateEvent(this, e);
        }

        private void UpdateData()
        {
            if (Bars.Count == 0)
                return;
            if (Bars.Count > _data.Count)
            {
                for (int i = _data.Count; i < Bars.Count; ++i)
                {
                    _data.AddValue(new StockValue {Time = Bars[i].Time, Value = GetValue(i)});
                }
            }
            Last.Value = GetValue(Bars.Count - 1);
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
            get { return this[_data.Count - 1]; }
        }

        public void Update()
        {
            UpdateData();
        }

        public virtual string Name
        {
            get { return "BarsSingleValues"; }
        }

        public bool Updatable
        {
            get { return true; }
        }

        public event EventHandler<EventArgs> UpdateEvent;

        #endregion
    }
}