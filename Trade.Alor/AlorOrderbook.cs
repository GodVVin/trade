using System;
using System.Collections.Generic;
using Trade.Core;
using Trade.Core.Terminals;

namespace Trade.Alor
{
    public class AlorOrderbook : AlorTable, IOrderbook
    {
        public string SecBoard { get; set; }
        public string SecCode { get; set; }

        public AlorOrderbook(int slotId, string secBoard, string secCode)
            : base(slotId, secBoard, secCode)
        {
            SecBoard = secBoard;
            SecCode = secCode;
        }

        public void Open()
        {
            Connect();
        }

        public void Close()
        {
            Disconnect();
        }

        public event EventHandler<OrderbookUpdatedEventArgs> Updated;

        public int RowsCount
        {
            get { return DataCount; }
        }

        protected override void OnRowUpdated(RowUpdateType updateType, int rowId)
        {
            if (updateType != RowUpdateType.Refreshed) return;

            OrderbookRow[] rows;
            lock (_glassData)
            {
                _glassData.Clear();
                foreach (var row in Data)
                {
                    var operation = (string) row.Value[3] == "B" ? Operation.Buy : Operation.Sell;
                    var price = (decimal) (double) row.Value[4];
                    var volume = (int) row.Value[5];
                    _glassData.Add(new OrderbookRow
                                   {
                                       Operation = operation,
                                       Price = price,
                                       Volume = volume
                                   });
                }
                rows = _glassData.ToArray();
            }
            if (Updated != null)
                Updated(this, new OrderbookUpdatedEventArgs(DateTime.Now, rows));
        }

        private readonly List<OrderbookRow> _glassData = new List<OrderbookRow>();
    }
}
