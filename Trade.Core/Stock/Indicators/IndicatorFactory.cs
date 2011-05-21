using Trade.Core.Logging;

namespace Trade.Core.Stock.Indicators
{
    public static class IndicatorFactory
    {
        public static IIndicator CreateIndicator(Bars bars, IndicatorInfo info)
        {
            Log.Instance.Trace("CreateIndicator {0}", info);
            IIndicator indicator;
            BarsSingleValues baseIndicator;
            switch (info.Base)
            {
                case IndicatorBase.Open:
                    baseIndicator = new BarsOpen(bars);
                    break;
                case IndicatorBase.Close:
                    baseIndicator = new BarsClose(bars);
                    break;
                case IndicatorBase.High:
                    baseIndicator = new BarsHigh(bars);
                    break;
                case IndicatorBase.Low:
                    baseIndicator = new BarsLow(bars);
                    break;
                default:
                    return null;
            }

            switch (info.Type)
            {
                case IndicatorType.EMA:
                    indicator = new Ema(baseIndicator, info.Period);
                    break;
                case IndicatorType.Simple:
                    indicator = new Simple(baseIndicator, info.Period);
                    break;
                default:
                    return null;
            }

            return indicator;
        }
    }
}