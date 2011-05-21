using System;

namespace Trade.Core.Stock
{
    public class StockValue
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }

        public StockValue()
        {
        }

        public StockValue(DateTime time, double value)
        {
            Time = time;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Time.ToShortTimeString(), Value);
        }
    }
}