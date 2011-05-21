using System;

namespace Trade.Core.Stock
{
    public class Quote : StockValue
    {
        public Security Security;
        public int Volume;

        public Quote()
        {
        }

        [Obsolete]
        public Quote(DateTime time, double value)
        {
            Time = time;
            Value = value;
        }

        public static double operator -(Quote quote1, Quote quote2)
        {
            return quote1.Value - quote2.Value;
        }

        public override string ToString()
        {
            return string.Format("[{0} x {1} at price {2} {3}]", Volume, Security.ShortName, Value, Time);
        }
    }
}