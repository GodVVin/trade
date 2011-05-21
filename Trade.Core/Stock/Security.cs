using System;

namespace Trade.Core.Stock
{
    public sealed class Security : ICloneable
    {
        public string Name { get; set; }
        public string SecBoard { get; set; }
        public string SecCode { get; set; }
        public string ShortName { get; set; }
        public double Bid { get; set; }
        public double Ask { get; set; }
        public int BidDepth { get; set; }
        public int AskDepth { get; set; }
        public double LastDeal { get; set; }

        // Options
        public string OptionType { get; set; }
        public double Strike { get; set; }
        public string FutCode { get; set; }
        public double Volatility { get; set; }

        /// <summary>
        /// Минимальный шаг цены
        /// </summary>
        public double MinStep { get; set; }

        /// <summary>
        /// Стоимость шага цены
        /// </summary>
        public double StepPrice { get; set; }

        public DateTime Updated { get; set; }


        public double Spread
        {
            get { return Ask - Bid; }
        }

        public bool CanTrade
        {
            get { return (Bid != 0) && (Ask != 0) && (LastDeal != 0) && (BidDepth != 0) && (AskDepth != 0); }
        }

        public Security()
        {
        }

        [Obsolete]
        public Security(string secBoard, string secCode, double bid, double ask, int bidDepth, int offerDepth,
                        double last, DateTime updated)
        {
            SecBoard = secBoard;
            SecCode = secCode;
            Bid = bid;
            Ask = ask;
            BidDepth = bidDepth;
            AskDepth = offerDepth;
            LastDeal = last;
            Updated = updated;
        }

        public override string ToString()
        {
            return string.Format("'{0}' last: {1} {2}/{3}", ShortName, LastDeal, Bid, Ask);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public double RoundPrice(double price)
        {
            return Math.Round(price/MinStep)*MinStep;
        }
    }
}