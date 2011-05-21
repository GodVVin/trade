namespace Trade.Core.Stock.Indicators
{
    public class BarsClose : BarsSingleValues
    {
        public BarsClose(Bars bars)
            : base(bars)
        {
        }

        public override string Name
        {
            get { return "Close"; }
        }

        protected override double GetValue(int index)
        {
            return Bars[index].Close;
        }
    }
}