
namespace Trade.Core.Stock
{
    public class MarketDepthPair
    {
        public Quote Ask { get; private set; }
        public Quote Bid { get; private set; }

        public double SpreadSize
        {
            get { return Ask - Bid; }
        }
    }
}