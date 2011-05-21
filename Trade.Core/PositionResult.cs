using System;
using System.Collections.Generic;

namespace Trade.Core
{
    public class PositionResult
    {
        public DateTime Opened { get; set; }
        public Operation Operation { get; set; }
        public IEnumerable<Stock.Trade> Trades { get; set; }
        public double Profit { get; set; }
    }
}
