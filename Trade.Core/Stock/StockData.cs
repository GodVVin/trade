using System;
using System.Collections.Generic;
using System.Text;

namespace Trade.Core.Stock
{
    public class StockData<T> : List<T>, IStockData<T>
    {
        public void AddValue(T value)
        {
            Add(value);
            if (UpdateEvent != null)
                UpdateEvent(this, EventArgs.Empty);
        }

        public string Name
        {
            get { return "stock data"; }
        }

        public bool Updatable
        {
            get { return true; }
        }

        public T Last
        {
            get { return this[Count - 1]; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Count; i++)
                sb.AppendFormat("{0}", this[i]);
            return sb.ToString();
        }

        public event EventHandler<EventArgs> UpdateEvent;
    }
}