using System;

namespace Trade.Core
{
    public class OrderbookUpdatedEventArgs : EventArgs
    {
        public DateTime Updated { get; private set; }
        public OrderbookRow[] Rows { get; private set; }

        public OrderbookUpdatedEventArgs(DateTime updated, OrderbookRow[] rows)
        {
            Updated = updated;
            Rows = rows;
        }
    }
}
