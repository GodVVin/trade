namespace Trade.Core.Stock.Indicators
{
    public class BarsLow : BarsSingleValues
    {
        public BarsLow(Bars bars)
            : base(bars)
        {
        }

        public override string Name
        {
            get { return "Low"; }
        }

        protected override double GetValue(int index)
        {
            return Bars[index].Low;
        }
    }
}