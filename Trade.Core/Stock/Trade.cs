namespace Trade.Core.Stock
{
    public sealed class Trade
    {
        public long TradeNo { get; set; }
        public long OrderNo { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return string.Format("[Trade {0} x {1}]", Quantity, Price);
        }
    }
}