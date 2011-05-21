using System;
using System.Collections.Generic;

namespace Trade.Core
{
    public interface IOrderbook
    {
        void Open();
        void Close();
        event EventHandler<OrderbookUpdatedEventArgs> Updated;
    }
}