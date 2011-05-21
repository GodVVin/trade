using System;

namespace Trade.Core.Orders
{
    public interface IOrders
    {
        void Open();
        void Close();
        event EventHandler<OrdersUpdatedEventArgs> Updated;

        long AddOrder(string secBoard, string secCode, Operation operation,
            int quantity, double price,
            bool isMarketOrder,
            bool isStopOrder, double alertPrice);
        CancelResult CancelOrder(long orderNo, bool isStopOrder);
    }
}