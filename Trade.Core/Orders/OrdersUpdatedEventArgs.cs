using System;

namespace Trade.Core.Orders
{
    public class OrdersUpdatedEventArgs : EventArgs
    {
        public DateTime Updated { get; private set; }
        public long OrderNo { get; private set; }
        public OrderStatus Status { get; private set; }
        public double Price { get; private set; }
        public int Balance { get; private set; }

        public OrdersUpdatedEventArgs(DateTime updated, long orderNo, OrderStatus status, double price, int balance)
        {
            Updated = updated;
            OrderNo = orderNo;
            Status = status;
            Price = price;
            Balance = balance;
        }

        public override string ToString()
        {
            return string.Format("[OUE {0} {1}, price: {2}, balance: {3}]", OrderNo, Status, Price, Balance);
        }
    }
}