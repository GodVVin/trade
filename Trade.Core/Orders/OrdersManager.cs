using System;
using System.Linq;
using System.Collections.Generic;
using Trade.Core.Logging;

namespace Trade.Core.Orders
{
    public class OrdersManager
    {
        private bool _isVirtual;

        public bool IsVirtual
        {
            get { return _isVirtual; }
            set { _isVirtual = value; }
        }

        public OrdersManager(IOrders orders, ITrades trades, ISecurities securities)
        {
            if (orders == null)
                throw new ArgumentNullException("orders");
            if (trades == null)
                throw new ArgumentNullException("trades");
            _orders = orders;
            _orders.Updated += OrdersUpdated;
            _trades = trades;
            _trades.Updated += TradesUpdated;
        }

        public void AddOrder(Order order)
        {
            Log.Instance.Debug("Order: {0}", order);
            bool isMarket = order.Type == OrderType.Market;
            bool isStop = order.Type == OrderType.StopOrder;
            double price = GetPrice(order);
            var orderNo = _orders.AddOrder(order.Security.SecBoard, order.Security.SecCode, order.Operation,
                order.Quantity, price, isMarket, isStop, order.AlertPrice);
            if (orderNo == -1)
                order.IsError = true;
            else
            {
                order.OrderNo = orderNo;
                var state = new OrderState { Status = OrderStatus.NotReady, Timestamp = DateTime.Now };
                order.SetState(state);
                AddOrderToList(order);
            }
        }

        public void CheckOrders(DateTime now)
        {
            var ordersToCancel = new List<Order>();
            lock (_locker)
            {
                var activeLimitOrders = _allOrders.Where(
                    o => o.Status == OrderStatus.Active && o.Type == OrderType.Limit && !o.IsError && !o.Canceling);

                foreach (var order in activeLimitOrders)
                {
                    var dueOpening = now - order.OpenTime;
                    if (dueOpening >= order.LimitCancelTimeout)
                    {
                        ordersToCancel.Add(order);
                        Log.Instance.Trace("Cancel order {0}, due time {1}", order.OrderNo, dueOpening.TotalMilliseconds);
                    }
                }
            }

            //ordersToCancel.ForEach(CancelOrder);
            foreach (var order in ordersToCancel)
                CancelOrder(order);
        }

        public void CancelOrder(Order order)
        {
            if (FindOrder(order.OrderNo) != null)
            {
                var result = _orders.CancelOrder(order.OrderNo, order.Type == OrderType.StopOrder);
                if (result == CancelResult.Error)
                {
                    Log.Instance.Warn("Ошибка снятия заявки {0}", order.OrderNo);
                    order.IsError = true;
                    //RemoveOrderFromList(order);
                }
                else if (result == CancelResult.Canceled)
                {
                    //L.Instance.Trace("Cancel Canceled");
                    //RemoveOrderFromList(order);
                    order.Canceling = true;
                    AddOrderToCanceledList(order);
                }
                else
                {
                    Log.Instance.Warn("Cancel NotFound");
                    //RemoveOrderFromList(order);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        // Private fields
        ///////////////////////////////////////////////////////////////////////////

        private IOrders _orders;
        private ITrades _trades;

        private object _locker = new object();

        private List<Order> _allOrders = new List<Order>();
        private List<Order> _canceledOrders = new List<Order>();

        private int _virtualOrderNo = -100500;


        private void AddOrderToList(Order order)
        {
            lock (_locker) _allOrders.Add(order);
        }

        private void RemoveOrderFromList(Order order)
        {
            lock (_locker) _allOrders.Remove(order);
        }

        private Order FindOrder(long orderNo)
        {
            lock (_locker) return _allOrders.Find(o => o.OrderNo == orderNo);
        }

        private void AddOrderToCanceledList(Order order)
        {
            lock (_locker) _canceledOrders.Add(order);
        }

        private double GetPrice(Order order)
        {
            double price = order.Price;
            switch (order.Type)
            {
                case OrderType.Limit:
                    price = order.Operation == Operation.Buy ?
                        order.Security.Bid + order.SpreadPriceDelta :
                        order.Security.Ask - order.SpreadPriceDelta;
                    break;
                case OrderType.Market:
                    price = order.Operation == Operation.Buy ? order.Security.Ask : order.Security.Bid;
                    break;
            }
            return price;
        }

        private void OrdersUpdated(object sender, OrdersUpdatedEventArgs e)
        {
            //L.Instance.Debug("e: {0}", e);
            var status = e.Status;
            var price = e.Price;
            var balance = e.Balance;
            var order = FindOrder(e.OrderNo);
            if (order != null)
            {
                lock (_locker)
                {
                    var state = new OrderState { Status = status, Balance = balance, Timestamp = e.Updated };
                    order.SetState(state);
                    order.Price = price;
                    //L.Instance.Trace("order {0}", order);
                }
            }
        }

        private void TradesUpdated(object sender, TradesUpdatedEventArgs e)
        {
            //L.Instance.Trace("e: {0}", e);
            var order = FindOrder(e.OrderNo);
            if (order != null)
            {
                lock (_locker)
                {
                    var trade = new Core.Stock.Trade
                        {
                            TradeNo = e.TradeNo,
                            OrderNo = e.OrderNo,
                            Quantity = e.Quantity,
                            Price = e.Price,
                            Value = e.Value
                        };
                    order.AddTrade(trade);
                    //L.Instance.Trace("order {0}", order);
                }
            }
        }

        private static void LogOrders(List<Order> list)
        {
            foreach (var order in list)
                Log.Instance.Info("  {0}", order);
        }

    }
}
