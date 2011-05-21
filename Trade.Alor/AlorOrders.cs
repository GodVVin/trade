using System;
using TEClientLib;
using Trade.Core;
using Trade.Core.Logging;
using Trade.Core.Orders;
using Trade.Core.Terminals;

namespace Trade.Alor
{
    public sealed class AlorOrders : AlorTable, IOrders
    {
        public AlorOrders(int slotDd, string account, string brokerRef)
            : base(slotDd, "ORDERS", false)
        {
            Log.Instance.Trace("AlorOrders.AlorOrders(slotId: {0}, account: {1}, brokerRef: {2})", slotDd, account, brokerRef);
            _account = account;
            _brokerRef = brokerRef;
        }

        #region Члены IOrders

        public event EventHandler<OrdersUpdatedEventArgs> Updated;

        public void Open()
        {
            Connect();
        }

        public void Close()
        {
            Disconnect();
        }

        public long AddOrder(string secBoard, string secCode, Operation operation,
            int quantity, double price,
            bool isMarketOrder,
            bool isStopOrder, double alertPrice)
        {
            if (DEBUG_ORDERS)
                Log.Instance.Trace("AddOrder {0}.{1} {2} {3}", secBoard, secCode, operation, price);
            var sfxOrder = CreateOrderObject();
            sfxOrder.SecBoard = secBoard;
            sfxOrder.SecCode = secCode;
            sfxOrder.BuySell = operation == Operation.Buy ? "B" : "S";
            sfxOrder.Quantity = quantity;
            sfxOrder.Price = price;
            sfxOrder.MktLimit = isMarketOrder ? "M" : "L";
            sfxOrder.OrderMode = isStopOrder ? "S" : "N";
            sfxOrder.AlertPrice = alertPrice;

            long orderNo = -1;
            string result;
            try
            {
                int rc = sfxOrder.Add(out orderNo, out result);
                result = result.Replace('\n', ' ');
                if (DEBUG_ORDERS)
                    Log.Instance.Info("AddOrder result: {0}, orderNo: {1} ({2})", rc, orderNo, result);
                if (rc != 0)
                {
                    Log.Instance.Error("Ошибка подачи заявки: {0}", result);
                    return -1;
                }
            }
            catch (Exception e)
            {
                Log.Instance.Error("SfxOrder.Add ERROR\n" + e.GetType() + "\n" + e.Message + "\n" + e.StackTrace + "\n" + e.Source);
            }
            return orderNo;
        }

        /// <summary>
        /// Запрашивает удаление заявки
        /// </summary>
        /// <param name="orderNo">Номер заявки</param>
        /// <param name="isStopOrder"></param>
        public CancelResult CancelOrder(long orderNo, bool isStopOrder)
        {
            Log.Instance.Info(String.Format("SfxOrder [{0}]", orderNo));
            SfxOrder sfxOrder = CreateOrderObject();
            sfxOrder.OrderNo = orderNo;
            sfxOrder.OrderMode = isStopOrder ? "S" : "N";

            string result;
            int rc = sfxOrder.Delete(out result);
            if (DEBUG_ORDERS)
                Log.Instance.Info("CancelOrder result: {0}, orderNo: {1} ({2})", rc, orderNo, result);
            if (rc == (int) SFE.SFE_OK) return CancelResult.Canceled;
            if (rc == (int) SFE.SFE_OBJECTNOTFOUND) return CancelResult.NotFound;
            return CancelResult.Error;
        }

        #endregion

        protected override void OnRowUpdated(RowUpdateType updateType, int rowId)
        {
            lock (_locker)
            {
                if (Updated != null)
                {
                    var orderNo = GetOrderNo(rowId);
                    var status = GetStatus(rowId);
                    var price = GetPrice(rowId);
                    var balance = GetBalance(rowId);
                    var e = new OrdersUpdatedEventArgs(DateTime.Now, orderNo, status, price, balance);
                    Updated(this, e);
                }
            }
        }

        private const bool DEBUG_ORDERS = true;

        private readonly string _account;
        private readonly string _brokerRef;

        private readonly object _locker = new object();

        private SfxOrder CreateOrderObject()
        {
            return new SfxOrder
            {
                SlotID = SlotId,
                Account = _account,
                BrokerRef = _brokerRef
            };
        }

        #region Methods for get values

        private long GetOrderNo(int rowId)
        {
            return (long)GetValue(rowId, "OrderNo");
        }

        private OrderStatus GetStatus(int rowId)
        {
            OrderStatus st;
            var status = (string)GetValue(rowId, "Status");
            switch (status)
            {
                case "A":
                    st = OrderStatus.Accepted;
                    break;
                case "O":
                    st = OrderStatus.Active;
                    break;
                case "M":
                    st = OrderStatus.Completed;
                    break;
                case "W":
                    st = OrderStatus.Canceled;
                    break;
                case "N":
                case "C":
                    st = OrderStatus.Rejected;
                    break;
                default:
                    st = OrderStatus.Unknown;
                    Log.Instance.Warn("Unknown order status: {0}", status);
                    break;
            }
            return st;
        }

        private int GetBalance(int rowId)
        {
            return (int)GetValue(rowId, "Balance");
        }

        private double GetPrice(int rowId)
        {
            return (double)GetValue(rowId, "Price");
        }

        #endregion
    }
}
