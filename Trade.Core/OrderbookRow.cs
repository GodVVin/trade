namespace Trade.Core
{
    public class OrderbookRow
    {
        public decimal Price;
        public int Volume;
        public Operation Operation;

        public override string ToString()
        {
            return string.Format("{0} {1} : {2}", Operation, Price, Volume);
        }
    }
}