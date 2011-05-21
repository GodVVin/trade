namespace Trade.Core.Stock.Indicators
{
    public enum IndicatorType
    {
        EMA,
        Simple
    }

    public enum IndicatorBase
    {
        Open,
        Close,
        High,
        Low
    }

    public struct IndicatorInfo
    {
        public readonly IndicatorType Type;
        public readonly IndicatorBase Base;
        public readonly int Period;

        public IndicatorInfo(IndicatorType type, IndicatorBase indicatorBase, int period)
        {
            Type = type;
            Base = indicatorBase;
            Period = period >= 1 ? period : 1;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}({2})", Type, Base, Period);
        }
    }

    public interface IIndicator : IStockData<StockValue>
    {
    }
}