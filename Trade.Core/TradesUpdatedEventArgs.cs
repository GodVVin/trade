using System;

namespace Trade.Core
{
    public class TradesUpdatedEventArgs : EventArgs
    {
        public DateTime UpdatedAt { get; private set; }
        public long TradeNo { get; private set; }
        public long OrderNo { get; private set; }
        public int Quantity { get; private set; }
        public double Price { get; private set; }
        public double Value { get; private set; }

        public TradesUpdatedEventArgs(DateTime updatedAt, long tradeNo, long orderNo, int quantity, double price, double value)
        {
            UpdatedAt = updatedAt;
            TradeNo = tradeNo;
            OrderNo = orderNo;
            Quantity = quantity;
            Price = price;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("[TUE {0}, order {1}, {2} x {3} ({4})]", TradeNo, OrderNo, Quantity, Price, Value);
        }
    }
}
