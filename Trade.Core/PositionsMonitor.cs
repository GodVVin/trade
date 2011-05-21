using System;
using System.Collections.Generic;
using System.Linq;

namespace Trade.Core
{
    public class PositionsMonitor
    {
        public event Action PositionsUpdated;

        public double Profit { get; private set; }

        public IEnumerable<PositionResult> Positions { get { return _positionResults; } }

        /// <summary>
        /// Количество убыточных сделок
        /// </summary>
        public int LossCount { get; private set; }

        public void AddPosition(PositionResult positionResult)
        {
            _positionResults.Add(positionResult);
            CalculateProfit();
        }

        private readonly List<PositionResult> _positionResults = new List<PositionResult>();

        private void CalculateProfit()
        {
            Profit = _positionResults.Sum(res => res.Profit);

            LossCount = 0;
            for (var i = _positionResults.Count - 1; i >= 0; i--)
            {
                if (_positionResults[i].Profit >= 0) break;
                LossCount++;
            }

            var method = PositionsUpdated;
            if (method != null)
                method();
        }
    }
}
