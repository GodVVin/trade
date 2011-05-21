namespace Trade.Core.Stock.Indicators
{
    public class BarsHigh : BarsSingleValues
    {
        public BarsHigh(Bars bars)
            : base(bars)
        {
        }

        public override string Name
        {
            get { return "High"; }
        }

        protected override double GetValue(int index)
        {
            return Bars[index].High;
        }
    }
}