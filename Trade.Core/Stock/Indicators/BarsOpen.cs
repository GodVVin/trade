namespace Trade.Core.Stock.Indicators
{
    public class BarsOpen : BarsSingleValues
    {
        public BarsOpen(Bars bars)
            : base(bars)
        {
        }

        public override string Name
        {
            get { return "Open"; }
        }

        protected override double GetValue(int index)
        {
            return Bars[index].Open;
        }
    }
}